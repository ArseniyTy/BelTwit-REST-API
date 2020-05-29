﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BelTwit_REST_API.Models;
using BelTwit_REST_API.Tokens.JWT_token;
using BelTwit_REST_API.Additional;
using BelTwit_REST_API.Tokens;
using Microsoft.Extensions.Logging;
using BelTwit_REST_API.Logging;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BelTwit_REST_API.ModelsJSON;

/*General:
 * Разделение на роли: у админа должна быть возможность редактировать все поля
 * http://localhost:port main get request -> инфа о сервисе (документация) - лучше head
 * 
 * твиттер
 - у юзера есть возможность добавлять твит
 - подписываться на других юзеров
 - получать все твиты на подписаных юзеров
 - ретвит
 - комменты и лайки к твитам
 */


/*User:                   
* Role=admin(system enum)  - get all ...
*/



//https://metanit.com/sharp/aspnet5/15.5.php!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!



namespace BelTwit_REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BelTwitContext _db;
        private ILogger _logger;

        public AuthController(BelTwitContext context, ILoggerFactory loggerFactory)
        {
            _db = context;
            _logger = loggerFactory.CreateLogger("DatabaseLogger");

           // _logger.LogInformation("LOGGER");
        }


        [HttpGet("get-subscriptions")]
        public ActionResult GetSubscriptions([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch (Exception ex) //token expired
            {
                return BadRequest(ex.Message);
            }

            var user = _db.Users.FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return BadRequest("No user that matches this JWT");


            _db.Entry(user).Collection(p => p.Subscriptions).Load();
            //выбірает всех, кто есть в бд
            var subscriptions = user.Subscriptions
                .Where(p => _db.Users.FirstOrDefault(i => i.Id == p.OnWhomSubscribeId)!=null)
                .Select(p => _db.Users.FirstOrDefault(i => i.Id == p.OnWhomSubscribeId))
                .ToList();

            return Ok(subscriptions);
        }

        [HttpGet("get-subscribers")]
        public ActionResult GetSubscribers([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch (Exception ex) //token expired
            {
                return BadRequest(ex.Message);
            }

            var user = _db.Users.FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return BadRequest("No user that matches this JWT");


            _db.Entry(user).Collection(p => p.Subscribers).Load();
            //выбірает всех, кто есть в бд
            var subscribers = user.Subscribers
                .Where(p => _db.Users.FirstOrDefault(i => i.Id == p.WhoSubscribeId) != null)
                .Select(p => _db.Users.FirstOrDefault(i => i.Id == p.WhoSubscribeId))
                .ToList();

            return Ok(subscribers);
        }

        [HttpPost("subscribe")]
        public ActionResult Subscribe([FromBody]JwtWtihObject<string> subInfo)
        {
            JWT token;
            try
            {
                token = new JWT(subInfo.JWT);
            }
            catch (Exception ex) //token expired
            {
                return BadRequest(ex.Message);
            }

            var userWhoSubscribe = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (userWhoSubscribe == null)
                return BadRequest("No user that matches this JWT");

            var userToSubscribe = _db.Users
                .FirstOrDefault(p => p.Login == subInfo.WithJWTObject);
            if (userToSubscribe == null)
                return BadRequest("No user that matches you entered login");

            var subSub = new SubscriberSubscription
            {
                WhoSubscribeId = userWhoSubscribe.Id,
                OnWhomSubscribeId = userToSubscribe.Id
            };
            _db.SubscriberSubscriptions.Add(subSub);
            _db.SaveChanges();

            return Ok(subSub);
        }

        [HttpDelete("unsubscribe")]
        public ActionResult Unsubscribe([FromBody]JwtWtihObject<string> subInfo)
        {
            JWT token;
            try
            {
                token = new JWT(subInfo.JWT);
            }
            catch(Exception ex) //token expired
            {
                return BadRequest(ex.Message);
            }

            var userWhoSubscribe = _db.Users.FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (userWhoSubscribe == null)
                return BadRequest("No user that matches this JWT");

            var userToSubscribe = _db.Users.FirstOrDefault(p => p.Login == subInfo.WithJWTObject);
            if (userToSubscribe == null)
                return BadRequest("No user in database that matches you entered login");

            var subSub = _db.SubscriberSubscriptions
                .FirstOrDefault(p => p.WhoSubscribeId == userWhoSubscribe.Id
                                  && p.OnWhomSubscribeId == userToSubscribe.Id);
            if (subSub == null)
                return BadRequest("You are not subscribed to this user");

            _db.SubscriberSubscriptions.Remove(subSub);
            _db.SaveChanges();
            return Ok(subSub);
        }









        //в будущем только для admin + возможность получіть одного передавая модельку
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            var users = _db.Users.ToList();
            return users;
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
                //ne proveryem srok godnosti
                token = new AccessRefreshToken(tokenJSON, CheckForExpiration: false);
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