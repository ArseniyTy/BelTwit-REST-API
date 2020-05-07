using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BelTwit_REST_API.Models;
using BelTwit_REST_API.Models.JWT;
using BelTwit_REST_API.Additional;

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
         * (Refresh = 60 days, access = 30 min)
         * 
         * 
         * Создать таблицу RefreshToken(s) - поля GUID Token, DateTime expiresIn (можно вычітать даты еслі)
         * AddDays(double value) - в конструкторе
         * AddMinutes(double value) - для access
         * 
         * Связать one-to-many с user (refresh token на фронтэнде должен ставіться кукой, поэтому
         * пользователь впрінціпе не должен с німі взаімодействовать, но у нас да, ты можешь сідеть с
         * несколькіх токенов на одном компе).
         * 
         * Добавіть то, что еслі у одного юзера больше 5 токенов, то все сбрасываются, кроме новосозданной
         * Нужно для безопасності. (отлавлівать ошібку тут нужно)
         * 
         * [YOU ARE HERE]
         * 
         * Проверка на истёкший access(добавіть expiresIn в Payload)/refresh в authentificate
         * TOKEN_EXPIRED/INVALID_REFRESH_SESSION - badrequest в ином случае
         * 
         * в authentificate в конце возвращаем объект из двух токенов access і refresh
         * 
         * Добавить POST auth/refresh-tokens (при истечении access/refresh)
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



            var JWT = new JWT(userFromDb);

            var refreshToken = new RefreshToken
            {
                TokenValue = new Guid(),
                ExpiresAt = DateTime.Now.AddDays(60), //спустя 60 дней
                UserId = userFromDb.Id,
                User = userFromDb
            };
            _db.RefreshTokens.Add(refreshToken);
            _db.SaveChanges();


            var token = new AccessRefreshToken
            {
                AccessToken = JWT.GetBase64Encoding(),
                RefreshToken = refreshToken.TokenValue.ToString()
            };
            return Ok(token);
        }

        [HttpGet("authorize")]
        public ActionResult AuthorizeUser([FromBody]string JWTToken)
        {
            JWT JWT;
            try
            {
                JWT = new JWT(JWTToken);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(JWT);
            //return Ok(JWT);
        }
    }
}