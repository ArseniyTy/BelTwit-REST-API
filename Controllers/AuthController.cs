using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BelTwit_REST_API.Models;
using BelTwit_REST_API.Tokens.JWT_token;
using BelTwit_REST_API.Additional;
using BelTwit_REST_API.Tokens;

namespace BelTwit_REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BelTwitContext _db;

        public AuthController(BelTwitContext context)
        {
            _db = context;
        }

        [HttpPost("create")]
        public ActionResult CreateUser([FromBody]User user)
        {
            var userFromDb = _db.Users.FirstOrDefault(u => u.Login == user.Login);
            if (userFromDb != null)
                return BadRequest("The user with such a login currently exists");


            user.Id = new Guid();
            user.PasswordSalt = SecurityService.GetSalt();
            user.Password = SecurityService.GetHash(user.Password, user.PasswordSalt);

            _db.Users.Add(user);
            _db.SaveChanges();
            return new ObjectResult(user);
        }

        [HttpPut("update")]
        public ActionResult UpdateUser([FromBody]Tuple<User, User> users)
        {
            var oldUser = users.Item1;
            var newUser = users.Item2;


            var userFromDb = _db.Users.
                FirstOrDefault(u => u.Login == oldUser.Login);
            if (userFromDb == null)
                return NotFound("There is no such a user");
            if (userFromDb.Password != SecurityService.GetHash(oldUser.Password, userFromDb.PasswordSalt))
                return new ForbidResult("Password is incorrect");


            if (newUser.Login != null)
                userFromDb.Login = newUser.Login;
            if (newUser.Password != null)
                userFromDb.Password = newUser.Password;

            _db.SaveChanges();
            return new ObjectResult(userFromDb);
        }

        [HttpDelete("delete")]
        public ActionResult DeleteUser([FromBody]User user)
        {
            var userFromDb = _db.Users.
                FirstOrDefault(u => u.Login == user.Login);
            if (userFromDb == null)
                return NotFound("There is no such a user");
            if (userFromDb.Password != SecurityService.GetHash(user.Password, userFromDb.PasswordSalt))
                return new ForbidResult("Password is incorrect");


            _db.Users.Remove(userFromDb);
            _db.SaveChanges();
            return Ok();
        }


        /*
         * 
         * 
         * Создать логер для нынешніх действій (метаніт + проект Деніса)
         * Потом уже ідті создавать і связывать twit
         * 
         * 
         * 
         */


        [HttpPost("authentificate")]
        public ActionResult AuthentificateUser([FromBody]User user)
        {
            var userFromDb = _db.Users.
                FirstOrDefault(u => u.Login == user.Login);
            if (userFromDb == null)
                return NotFound("There is no such a user");
            if (userFromDb.Password != SecurityService.GetHash(user.Password,userFromDb.PasswordSalt))
                return new ForbidResult("Password is incorrect");


            var token = new AccessRefreshToken(userFromDb).ParseToJSON();
            return Ok(token);
        }

        [HttpGet("authorize")]
        public ActionResult AuthorizeUser([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }


            if (token.IsTokenExpired())
            {
                return BadRequest("Token expired");
            }

            return Ok(token);
            //return Ok(token.GetBase64Encoding());
        }
        

        //можно передавать любой старый access, і он будет обновляться
        //но это і не важно. Тут должна гарантіроваться лішь авторізованность
        //і в целом хакер не сможет получіть доступ к update-tokens
        [HttpPost("update-tokens")]
        public ActionResult RefreshTokens([FromBody]AccessRefreshTokenJSON tokenJSON)
        {
            AccessRefreshToken token;
            try
            {
                token = new AccessRefreshToken(tokenJSON);
                token.UpdateTokens();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(token.ParseToJSON());
        }
    }
}