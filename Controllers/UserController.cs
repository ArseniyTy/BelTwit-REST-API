using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BelTwit_REST_API.Additional;
using BelTwit_REST_API.Models;
using BelTwit_REST_API.ModelsJSON;
using BelTwit_REST_API.Tokens.JWT_token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BelTwit_REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BelTwitContext _db;
        private ILogger _logger;

        public UserController(BelTwitContext context, ILoggerFactory loggerFactory)
        {
            _db = context;
            _logger = loggerFactory.CreateLogger("DatabaseLogger");
        }



        #region Subscribe/Subscriptions/Subscribers
        [HttpOptions("get-subscribers")]
        public ActionResult GetSubscribersOptions()
        {
            Response.Headers.Add("Allow", "GET, HEAD");
            return Ok();
        }
        [HttpHead("get-subscribers")]
        public ActionResult GetSubscribersHead([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GET]api/auth/get-subscribers" + ex.Message);
                return BadRequest();
            }

            var user = _db.Users.FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound();


            _db.Entry(user).Collection(p => p.Subscribers).Load();
            //выбірает всех, кто есть в бд
            var subscribers = user.Subscribers
                .Where(p => _db.Users.FirstOrDefault(i => i.Id == p.WhoSubscribeId) != null)
                .Select(p => _db.Users.FirstOrDefault(i => i.Id == p.WhoSubscribeId))
                .ToList();

            return Ok();
        }
        [HttpGet("get-subscribers")]
        public ActionResult GetSubscribers([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GET]api/auth/get-subscribers" + ex.Message);
                return BadRequest(ex.Message);
            }

            var user = _db.Users.FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("No user that matches this JWT");


            _db.Entry(user).Collection(p => p.Subscribers).Load();
            //выбірает всех, кто есть в бд
            var subscribers = user.Subscribers
                .Where(p => _db.Users.FirstOrDefault(i => i.Id == p.WhoSubscribeId) != null)
                .Select(p => _db.Users.FirstOrDefault(i => i.Id == p.WhoSubscribeId))
                .ToList();

            return Ok(subscribers);
        }









        [HttpOptions("subscribe")]
        public ActionResult SubscribeOptions()
        {
            Response.Headers.Add("Allow", "GET, HEAD, POST, DELETE");
            return Ok();
        }
        [HttpHead("subscribe")]
        public ActionResult GetSubscriptionsHead([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GET]api/auth/get-subscriptions" + ex.Message);
                return BadRequest();
            }

            var user = _db.Users.FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound();


            _db.Entry(user).Collection(p => p.Subscriptions).Load();
            //выбірает всех, кто есть в бд
            var subscriptions = user.Subscriptions
                .Where(p => _db.Users.FirstOrDefault(i => i.Id == p.OnWhomSubscribeId) != null)
                .Select(p => _db.Users.FirstOrDefault(i => i.Id == p.OnWhomSubscribeId))
                .ToList();

            return Ok();
        }
        [HttpGet("subscribe")]
        public ActionResult GetSubscriptions([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GET]api/auth/get-subscriptions" + ex.Message);
                return BadRequest(ex.Message);
            }

            var user = _db.Users.FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("No user that matches this JWT");


            _db.Entry(user).Collection(p => p.Subscriptions).Load();
            //выбірает всех, кто есть в бд
            var subscriptions = user.Subscriptions
                .Where(p => _db.Users.FirstOrDefault(i => i.Id == p.OnWhomSubscribeId) != null)
                .Select(p => _db.Users.FirstOrDefault(i => i.Id == p.OnWhomSubscribeId))
                .ToList();

            return Ok(subscriptions);
        }
        [HttpPost("subscribe")]
        public ActionResult Subscribe([FromBody]JwtWtihObject<string> subInfo)
        {
            JWT token;
            try
            {
                token = new JWT(subInfo.JWT);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[POST]api/auth/subscribe" + ex.Message);
                return BadRequest(ex.Message);
            }

            var userWhoSubscribe = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (userWhoSubscribe == null)
                return NotFound("No user that matches this JWT");

            var userToSubscribe = _db.Users
                .FirstOrDefault(p => p.Login == subInfo.WithJWTObject);
            if (userToSubscribe == null)
                return NotFound("No user that matches you entered login");
            if (userToSubscribe == userWhoSubscribe)
                return BadRequest("You can't subscribe on yourself");

            var subSub = new SubscriberSubscription
            {
                WhoSubscribeId = userWhoSubscribe.Id,
                OnWhomSubscribeId = userToSubscribe.Id
            };
            _db.SubscriberSubscriptions.Add(subSub);
            _db.SaveChanges();


            _logger.LogInformation($"[POST]api/auth/subscribe;" +
                                    $"User [{userWhoSubscribe.Login}] subscribed to [{userToSubscribe.Id}]");
            return Ok(subSub);
        }

        [HttpDelete("subscribe")]
        public ActionResult Unsubscribe([FromBody]JwtWtihObject<string> subInfo)
        {
            JWT token;
            try
            {
                token = new JWT(subInfo.JWT);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DELETE]api/auth/unsubscribe" + ex.Message);
                return BadRequest(ex.Message);
            }

            var userWhoSubscribe = _db.Users.FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (userWhoSubscribe == null)
                return NotFound("No user that matches this JWT");

            var userToSubscribe = _db.Users.FirstOrDefault(p => p.Login == subInfo.WithJWTObject);
            if (userToSubscribe == null)
                return NotFound("No user in database that matches you entered login");

            var subSub = _db.SubscriberSubscriptions
                .FirstOrDefault(p => p.WhoSubscribeId == userWhoSubscribe.Id
                                  && p.OnWhomSubscribeId == userToSubscribe.Id);
            if (subSub == null)
            {
                string ex = "You are not subscribed to this user";
                _logger.LogError($"[DELETE]api/auth/unsubscribe" + ex);
                return BadRequest(ex);
            }

            _db.SubscriberSubscriptions.Remove(subSub);
            _db.SaveChanges();


            _logger.LogInformation($"[DELETE]api/auth/unsubscribe;" +
                                   $"User [{userWhoSubscribe.Login}] unsubscribed from [{userToSubscribe.Id}]");
            return Ok(subSub);
        }
        #endregion








        [HttpOptions]
        public ActionResult UserControllerOptions()
        {
            Response.Headers.Add("Allow", "GET, HEAD, POST, PUT, DELETE");
            return Ok();
        }
        #region UserController (GetAll, Create, Modify, Delete, Delete(admin))
        [HttpHead]
        public ActionResult GetUsersHead()
        {
            return Ok();
        }
        [HttpGet]
        public ActionResult GetUsers()
        {
            var users = _db.Users.ToList();
            return Ok(users);
        }

        [HttpPost]
        public ActionResult CreateUser([FromBody]User user)
        {
            var userFromDb = _db.Users.FirstOrDefault(u => u.Login == user.Login);
            if (userFromDb != null)
            {
                string ex = "The user with such a login currently exists";
                _logger.LogError($"[POST]api/auth/create" + ex);
                return BadRequest(ex);
            }


            user.Id = new Guid();
            user.PasswordSalt = SecurityService.GetSalt();
            user.Password = SecurityService.GetHash(user.Password, user.PasswordSalt);

            _db.Users.Add(user);
            _db.SaveChanges();



            _logger.LogInformation($"[POST]api/auth/create;" +
                                   $"User [{user.Login}] was created");
            return Ok(user);
        }

        [HttpPut]
        public ActionResult UpdateUser([FromBody]Tuple<User, User> users)
        {
            var oldUser = users.Item1;
            var newUser = users.Item2;


            var userFromDb = _db.Users.
                FirstOrDefault(u => u.Login == oldUser.Login);
            if (userFromDb == null)
                return NotFound("There is no such a user");
            if (userFromDb.Password != SecurityService.GetHash(oldUser.Password, userFromDb.PasswordSalt))
            {
                string ex = "Password is incorrect";
                _logger.LogError($"[PUT]api/auth/update" + ex);
                return StatusCode(403, ex);
            }


            if (newUser.Login != null)
                userFromDb.Login = newUser.Login;
            if (newUser.Password != null)
                userFromDb.Password = SecurityService.GetHash(newUser.Password, userFromDb.PasswordSalt);
            _db.SaveChanges();


            _logger.LogInformation($"[PUT]api/auth/update;" +
                                   $"User [{userFromDb.Login}] was modified");
            return Ok(userFromDb);
        }

        [HttpDelete]
        public ActionResult DeleteUser([FromBody]User user)
        {
            var userFromDb = _db.Users
                .Include(p => p.Subscribers)
                .Include(p => p.Subscriptions)
                .Include(p => p.Tweets)
                .Include(p => p.TweetRateStates)
                .FirstOrDefault(u => u.Login == user.Login);
            if (userFromDb == null)
                return NotFound("There is no such a user");
            if (userFromDb.Password != SecurityService.GetHash(user.Password, userFromDb.PasswordSalt))
            {
                string ex = "Password is incorrect";
                _logger.LogError($"[DELETE]api/auth/delete" + ex);
                return StatusCode(403, ex);
            }

            #region РучнаяЧистка(т.к. в контексте DeleteBehaviour.Restrict, а по другому і нельзя)
            _db.SubscriberSubscriptions.RemoveRange(userFromDb.Subscriptions);
            _db.SubscriberSubscriptions.RemoveRange(userFromDb.Subscribers);
            _db.UserRateStates.RemoveRange(userFromDb.TweetRateStates);
            foreach (var tweet in userFromDb.Tweets)
            {
                var tweetComments = _db.Comments
                    .Where(p => p.TweetId == tweet.Id).ToList();
                _db.Comments.RemoveRange(tweetComments);

                var tweetRates = _db.UserRateStates
                    .Where(p => p.TweetId == tweet.Id).ToList();
                _db.UserRateStates.RemoveRange(tweetRates);
            }
            #endregion
            _db.Users.Remove(userFromDb);
            _db.SaveChanges();


            _logger.LogInformation($"[DELETE]api/auth/delete;" +
                                   $"User [{userFromDb.Login}] was deleted");
            return Ok();
        }


        [HttpOptions("admin-delete")]
        public ActionResult DeleteUserAdminOptions()
        {
            Response.Headers.Add("Allow", "DELETE");
            return Ok();
        }
        [HttpDelete("admin-delete")]
        public ActionResult DeleteUserAdmin([FromBody]JwtWtihObject<string> jwtWithUserLogin)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithUserLogin.JWT);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DELETE]api/auth/admin-delete" + ex);
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");
            if (!user.IsAdmin)
            {
                string ex = "This method is only for administrators";
                _logger.LogError($"[DELETE]api/auth/admin-delete" + ex);
                return StatusCode(403, ex);
            }



            var userToDelete = _db.Users
                    .Include(p => p.Subscribers)
                    .Include(p => p.Subscriptions)
                    .Include(p => p.Tweets)
                    .Include(p => p.TweetRateStates)
                    .FirstOrDefault(p => p.Login == jwtWithUserLogin.WithJWTObject);
            if (userToDelete == null)
                return NotFound("There is no user with such Id");
            if (userToDelete == user)
            {
                string ex = "You can't delete this account with this method";
                _logger.LogError($"[DELETE]api/auth/admin-delete" + ex);
                return StatusCode(403, ex);
            }

            #region РучнаяЧистка(т.к. в контексте DeleteBehaviour.Restrict, а по другому і нельзя)
            _db.SubscriberSubscriptions.RemoveRange(userToDelete.Subscriptions);
            _db.SubscriberSubscriptions.RemoveRange(userToDelete.Subscribers);
            _db.UserRateStates.RemoveRange(userToDelete.TweetRateStates);
            foreach (var tweet in userToDelete.Tweets)
            {
                var tweetComments = _db.Comments
                    .Where(p => p.TweetId == tweet.Id).ToList();
                _db.Comments.RemoveRange(tweetComments);

                var tweetRates = _db.UserRateStates
                    .Where(p => p.TweetId == tweet.Id).ToList();
                _db.UserRateStates.RemoveRange(tweetRates);
            }
            #endregion
            _db.Users.Remove(userToDelete);
            _db.SaveChanges();

            _logger.LogInformation($"[DELETE]api/auth/admin-delete;" +
                                   $"User [{userToDelete.Login}] was deleted by admin [{user.Login}]");
            return Ok();
        }
        #endregion
    }
}