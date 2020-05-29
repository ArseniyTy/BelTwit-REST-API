using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BelTwit_REST_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BelTwit_REST_API.Tokens.JWT_token;
using Microsoft.EntityFrameworkCore;
using BelTwit_REST_API.ModelsJSON;

/*TwitterController: 
 * 
 * 4)Dobavit head + options metodi
 * 
*/


namespace BelTwit_REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwitterController : ControllerBase
    {
        private readonly BelTwitContext _db;
        private ILogger _logger;

        public TwitterController(BelTwitContext context, ILoggerFactory loggerFactory)
        {
            _db = context;
            _logger = loggerFactory.CreateLogger("DatabaseLogger");
        }


        //HEAD is the same as GET but does not has a respond body
        //Good practice, because we can check what code will be returned, before
        //GETting big data, or to check if the resource exist
        [HttpHead("getById/{id}")]
        public ActionResult GetTweetByIdHead(string id)
        {
            Guid idGuid;
            try
            {
                idGuid = new Guid(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GET]api/twitter/getById/id;" + ex.Message);
                return BadRequest();
            }

            var tweet = _db.Tweets
                .Include(p => p.TweetComments)
                .FirstOrDefault(p => p.Id == idGuid);
            if (tweet == null)
                return NotFound();

            return Ok();
        }

        [HttpGet("getById/{id}")]
        public ActionResult GetTweetById(string id)
        {
            Guid idGuid;
            try
            {
                idGuid = new Guid(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GET]api/twitter/getById/id;" + ex.Message);
                return BadRequest(ex.Message);
            }

            var tweet = _db.Tweets
                .Include(p => p.TweetComments)
                .FirstOrDefault(p => p.Id == idGuid);
            if (tweet == null)
                return NotFound("There is no such a tweet");

            return Ok(tweet);
        }


        [HttpHead("getByLogin/{login}")]
        public ActionResult GetTweetsByLoginHead(string login)
        {
            var user = _db.Users
                .FirstOrDefault(p => p.Login == login);
            if (user == null)
                return NotFound();

            var tweets = _db.Tweets
                .Include(p => p.TweetComments)
                .Where(p => p.UserId == user.Id)
                .ToList();

            return Ok();
        }
        [HttpGet("getByLogin/{login}")]
        public ActionResult GetTweetsByLogin(string login)
        {
            var user = _db.Users
                .FirstOrDefault(p => p.Login == login);
            if (user == null)
                return NotFound("There is no such a user");

            var tweets = _db.Tweets
                .Include(p => p.TweetComments)
                .Where(p => p.UserId == user.Id)
                .ToList();

            return Ok(tweets);
        }


        [HttpHead]
        public ActionResult GetMySubscriptionsTweetsHead([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GET]api/twitter;" + ex.Message);
                return BadRequest();
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound();


            _db.Entry(user).Collection(p => p.Subscriptions).Load();
            var subscriptions = user.Subscriptions
                .Where(p => _db.Users.FirstOrDefault(i => i.Id == p.OnWhomSubscribeId) != null)
                .Select(p => _db.Users.FirstOrDefault(i => i.Id == p.OnWhomSubscribeId))
                .ToList();

            var alltweets = new List<Tweet>();
            foreach (var sub in subscriptions)
            {
                _db.Entry(sub).Collection(p => p.Tweets).Load();
                var subTweets = sub.Tweets;
                alltweets.AddRange(subTweets);
            }

            return Ok();
        }
        [HttpGet]
        public ActionResult GetMySubscriptionsTweets([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GET]api/twitter;" + ex.Message);
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");


            _db.Entry(user).Collection(p => p.Subscriptions).Load();
            var subscriptions = user.Subscriptions
                .Where(p => _db.Users.FirstOrDefault(i => i.Id == p.OnWhomSubscribeId) != null)
                .Select(p => _db.Users.FirstOrDefault(i => i.Id == p.OnWhomSubscribeId))
                .ToList();

            var alltweets = new List<Tweet>();
            foreach (var sub in subscriptions)
            {
                _db.Entry(sub).Collection(p => p.Tweets).Load();
                var subTweets = sub.Tweets;
                alltweets.AddRange(subTweets);
            }

            return Ok(alltweets);
        }


        [HttpPost]
        public ActionResult AddTweet([FromBody]JwtWtihObject<Tweet> jwtWithTweet)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithTweet.JWT);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[POST]api/twitter;" + ex.Message);
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");





            var tweetJSON = jwtWithTweet.WithJWTObject;
            if (tweetJSON == null)
                return BadRequest("No tweet object in request");
            tweetJSON.UserId = token.PAYLOAD.Sub;

            Tweet tweet;
            try
            {
                tweet = new Tweet(tweetJSON);
            }
            catch(Exception ex)
            {
                _logger.LogError($"[POST]api/twitter;" + ex.Message);
                return BadRequest(ex.Message);
            }

            _db.Tweets.Add(tweet);
            _db.SaveChanges();


            _logger.LogInformation($"[POST]api/twitter;" +
                                    $"User [{user.Login}] added tweet [{tweet.Id}]");
            return Ok(tweet);
        }

        [HttpDelete]
        public ActionResult DeleteTweet([FromBody]JwtWtihObject<Guid> jwtWithTweetId)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithTweetId.JWT);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DELETE]api/twitter;" + ex.Message);
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");




            Guid tweetId;
            try
            {
                tweetId = jwtWithTweetId.WithJWTObject;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DELETE]api/twitter;" + ex.Message);
                return BadRequest(ex.Message);
            }


            Tweet tweet;
            if(user.IsAdmin)
            {
                tweet = _db.Tweets
                    .FirstOrDefault(p => p.Id == tweetId);
                if (tweet == null)
                    return NotFound("There is no tweet with such Id");
            }
            else
            {
                _db.Entry(user).Collection(p => p.Tweets).Load();
                tweet = user.Tweets
                    .FirstOrDefault(p => p.Id == tweetId);
                if (tweet == null)
                    return NotFound("You haven't tweet with such Id");
            }

            _db.Entry(tweet).Collection(p => p.TweetComments).Load();
            _db.Entry(tweet).Collection(p => p.TweetRateStates).Load();         

            _db.Comments.RemoveRange(tweet.TweetComments); //чістка вручную, т.к. в контексте DeleteBehaviour.Restrict (а по другому і нельзя)
            _db.UserRateStates.RemoveRange(tweet.TweetRateStates);
            _db.Tweets.Remove(tweet);
            _db.SaveChanges();



            _logger.LogInformation($"[DELETE]api/twitter;" +
                                    $"User [{user.Login}] deleted tweet [{tweet.Id}]");
            return Ok(tweet);
        }


        [HttpPost("retweet")]
        public ActionResult Retweet([FromBody]JwtWtihObject<Guid> jwtWithTweetId)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithTweetId.JWT);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[POST]api/twitter/retweet;" + ex.Message);
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");





            Guid tweetId;
            try
            {
                tweetId = jwtWithTweetId.WithJWTObject;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[POST]api/twitter/retweet;" + ex.Message);
                return BadRequest(ex.Message);
            }


            var tweet = _db.Tweets
                .FirstOrDefault(p => p.Id == tweetId);
            if (tweet == null)
                return NotFound("There is no tweet with such Id");

            var retweet = new Tweet(tweet);
            retweet.UserIdRetweetedFrom = retweet.UserId;
            retweet.UserId = user.Id;
            _db.Tweets.Add(retweet);
            _db.SaveChanges();


            _logger.LogInformation($"[POST]api/twitter/retweet;" +
                                    $"User [{user.Login}] retweeted tweet [{tweet.Id}]. So, tweet [{retweet.Id}] is created");
            return Ok(retweet);
        }



        [HttpPost("comment-tweet")]
        public ActionResult WriteCommentToTweet([FromBody]JwtWtihObject<TweetIdWithObject<string>> jwtWithComment)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithComment.JWT);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[POST]api/twitter/comment-tweet;" + ex.Message);
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");




            Guid tweetId;
            try
            {
                tweetId = jwtWithComment.WithJWTObject.TweetId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[POST]api/twitter/comment-tweet;" + ex.Message);
                return BadRequest(ex.Message);
            }
            var tweet = _db.Tweets
                .FirstOrDefault(p => p.Id == tweetId);
            if (tweet == null)
                return NotFound("User doesn't have tweet with such Id");

            var commentCont = jwtWithComment.WithJWTObject.WithTweetObject;
            if (commentCont == null || commentCont.Length==0)
            {
                string ex = "Comment must include at least one symbol";
                _logger.LogError($"[POST]api/twitter/comment-tweet;" + ex);
                return BadRequest(ex);
            }

            var comment = new Comment
            {
                Id = new Guid(),
                Content = commentCont,
                TweetId = tweet.Id,
                UserId = user.Id
            };
            _db.Comments.Add(comment);
            _db.SaveChanges();


            _logger.LogInformation($"[POST]api/twitter/comment-tweet;" +
                                    $"User [{user.Login}] commented tweet [{tweet.Id}]. So, comment [{comment.Id}] is created");
            return Ok(comment);
        }


        [HttpDelete("comment-tweet")]
        public ActionResult DeleteCommentToTweet([FromBody]JwtWtihObject<Guid> jwtWithCommentId)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithCommentId.JWT);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DELETE]api/twitter/comment-tweet;" + ex.Message);
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");




            Guid commentId;
            try
            {
                commentId = jwtWithCommentId.WithJWTObject;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DELETE]api/twitter/comment-tweet;" + ex.Message);
                return BadRequest(ex.Message);
            }


            Comment comment;
            if (user.IsAdmin)
            {
                comment = _db.Comments
                    .FirstOrDefault(p => p.Id == commentId);
                if (comment == null)
                    return NotFound("There is no comment with such Id");
            }
            else
            {
                _db.Entry(user).Collection(p => p.TweetComments).Load();
                comment = user.TweetComments
                    .FirstOrDefault(p => p.Id == commentId);
                if (comment == null)
                    return NotFound("You haven't comment with such Id");
            }
            
            _db.Comments.Remove(comment);
            _db.SaveChanges();


            _logger.LogInformation($"[DELETE]api/twitter/comment-tweet;" +
                                    $"User [{user.Login}] deleted comment [{comment.Id}]");
            return Ok(comment);
        }



        [HttpPut("rate-tweet")]
        public ActionResult RateTweet([FromBody]JwtWtihObject<TweetIdWithObject<RateState>> jwtWithInfo)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithInfo.JWT);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PUT]api/twitter/rate-tweet;" + ex.Message);
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");




            Guid tweetId;
            try
            {
                tweetId = jwtWithInfo.WithJWTObject.TweetId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[PUT]api/twitter/rate-tweet;" + ex.Message);
                return BadRequest(ex.Message);
            }
            var tweet = _db.Tweets
                .FirstOrDefault(p => p.Id == tweetId);
            if (tweet == null)
                return NotFound("User doesn't have tweet with such Id");


            int stateFromJSON = (int)jwtWithInfo.WithJWTObject.WithTweetObject;
            if (stateFromJSON != -1 && stateFromJSON != 0 && stateFromJSON != 1)
            {
                string ex = "There is no such rate state";
                _logger.LogError($"[PUT]api/twitter/rate-tweet;" + ex);
                return BadRequest(ex);
            }


            _db.Entry(tweet).Collection(p => p.TweetRateStates).Load();
            var stateFromDb = tweet.TweetRateStates
                .FirstOrDefault(p => p.UserId == user.Id);
            if (stateFromDb != null)
            {
                if ((int)stateFromDb.RateState == stateFromJSON)
                {
                    string ex = "You have already rated this tweet the same way";
                    _logger.LogError($"[PUT]api/twitter/rate-tweet;" + ex);
                    return BadRequest(ex);
                }



                if (stateFromDb.RateState == RateState.Dislike)
                    tweet.Dislikes--;
                else if (stateFromDb.RateState == RateState.Like)
                    tweet.Likes--;

                

                stateFromDb.RateState = (RateState)stateFromJSON;
            }
            else
            {
                var newState = new UserRateState
                {
                    RateState = (RateState)stateFromJSON,
                    TweetId = tweet.Id,
                    UserId = user.Id
                };
                _db.UserRateStates.Add(newState);
            }

            if ((RateState)stateFromJSON == RateState.Dislike)
                tweet.Dislikes++;
            else if ((RateState)stateFromJSON == RateState.Like)
                tweet.Likes++;

            _db.Tweets.Update(tweet);
            _db.SaveChanges();


            _logger.LogInformation($"[PUT]api/twitter/rate-tweet;" +
                        $"User [{user.Login}] rated tweet [{tweet.Id}]");
            return Ok();
        }
    }
}