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

//UPDATE_TOKENS !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

/*TwitterController:
 * 
 * Comment model (User(+connection),Content,Likes,Dislikes)
 * 
 * POST(Id tweet-a) - write comment
 * POST(Id) - put like tweet
 * POST(Id) - put dislike tweet
 * 
 * 
 * * 
 *\kak lenta novostey/
 * GET - get all tweets of your subscriptions sorted by data  + доп параметр, сколько первых выбрать
 * 
 * 
 * 
 * POST - retweet (с указаніем retweeted from)
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
                return BadRequest(ex.Message);
            }

            var tweet = _db.Tweets
                .Include(p => p.TweetComments)
                .FirstOrDefault(p => p.Id == idGuid);
            if (tweet == null)
                return BadRequest("Such tweet doen't exist");
            return Ok(tweet);
        }

        [HttpGet("getByLogin/{login}")]
        public ActionResult GetTweetsByLogin(string login)
        {
            var user = _db.Users
                .FirstOrDefault(p => p.Login == login);
            if (user == null)
                return BadRequest("There is no such a user");

            var tweets = _db.Tweets
                .Include(p => p.TweetComments)
                .Where(p => p.UserId == user.Id)
                .ToList();

            return Ok(tweets);
        }

        [HttpPost]
        public ActionResult AddTweet([FromBody]JwtWtihObject<Tweet> jwtWithTweet)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithTweet.JWT);
            }
            catch (Exception ex) //token expired
            {
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");





            var tweetJSON = jwtWithTweet.Object;
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
                return BadRequest(ex.Message);
            }

            _db.Tweets.Add(tweet);
            _db.SaveChanges();

            return Ok(tweet);
        }





        [HttpPost("comment-tweet")]
        public ActionResult WriteCommentToTweet([FromBody]JwtWtihObject<TweetIdWithCommentJSON> jwtWithComment)
        {
            //пока что выполняет функцію того, что только авторізованные пользователі могут коменты
            //писать, но в будущем нужна для прикрутки авторства коменту
            JWT token;
            try
            {
                token = new JWT(jwtWithComment.JWT);
            }
            catch (Exception ex) //token expired
            {
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");




            Guid tweetId;
            try
            {
                tweetId = jwtWithComment.Object.TweetId;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var tweet = _db.Tweets
                .FirstOrDefault(p => p.Id == tweetId);
            if (tweet == null)
                return NotFound("User doesn't have tweet with such Id");

            var commentCont = jwtWithComment.Object.Comment;
            if (commentCont == null || commentCont.Length==0)
                return BadRequest("Comment must include at least one symbol");

            var comment = new Comment
            {
                Id = new Guid(),
                Content = commentCont,
                TweetId = tweet.Id,
                UserId = user.Id
            };
            _db.Comments.Add(comment);
            _db.SaveChanges();

            return Ok(tweet);
        }



        [HttpPut("rate-tweet")]
        public ActionResult RateTweet([FromBody]JwtWtihObject<Tuple<Guid, RateState>> jwtWithInfo)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithInfo.JWT);
            }
            catch (Exception ex) //token expired
            {
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");




            Guid tweetId;
            try
            {
                tweetId = jwtWithInfo.Object.Item1;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var tweet = _db.Tweets
                .FirstOrDefault(p => p.Id == tweetId);
            if (tweet == null)
                return NotFound("User doesn't have tweet with such Id");


            int stateFromJSON = (int)jwtWithInfo.Object.Item2;
            if (stateFromJSON != -1 && stateFromJSON != 0 && stateFromJSON != 1)
                return BadRequest("There is no such rate state");


            _db.Entry(tweet).Collection(p => p.TweetRateStates).Load();
            var stateFromDb = tweet.TweetRateStates
                .FirstOrDefault(p => p.UserId == user.Id);
            if (stateFromDb != null)
            {
                if ((int)stateFromDb.RateState == stateFromJSON)
                    return BadRequest("You have already rated this tweet the same way");



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
                tweet.Likes += stateFromJSON; //еслі дізлайк то +=1
            }

            if ((RateState)stateFromJSON == RateState.Dislike)
                tweet.Dislikes++;
            else if ((RateState)stateFromJSON == RateState.Like)
                tweet.Likes++;

            _db.Tweets.Update(tweet);
            _db.SaveChanges();

            return Ok();
        }



        [HttpDelete]
        public ActionResult DeleteTweet([FromBody]JwtWtihObject<string> jwtWithTweet)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithTweet.JWT);
            }
            catch (Exception ex) //token expired
            {
                return BadRequest(ex.Message);
            }
            var user = _db.Users
                .Include(p => p.Tweets)
                .FirstOrDefault(p => p.Id == token.PAYLOAD.Sub);
            if (user == null)
                return NotFound("Your jwt doesn't match any user!");




            Guid tweetId;
            try
            {
                tweetId = new Guid(jwtWithTweet.Object);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            var tweet = user.Tweets
                .FirstOrDefault(p => p.Id == tweetId);
            if (tweet == null)
                return NotFound("You haven't tweet with such Id");

            _db.Tweets.Remove(tweet);
            _db.SaveChanges();

            return Ok(tweet);
        }
    }
}