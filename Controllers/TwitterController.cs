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

            //_db.Entry(user).Collection(p => p.Tweets).Load();
            var tweets = _db.Users
                .Include(p => p.Tweets)
                .Where(p => p.Id == user.Id)
                .Select(p => p.Tweets)
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

            var comment = jwtWithComment.Object.Comment;
            if (comment == null || comment.Length==0)
                return BadRequest("Comment must include at least one symbol");

            var commentList = tweet.Comments;
            commentList.Add(comment);
            tweet.Comments = commentList; //для обновленія бд


            _db.Tweets.Update(tweet);
            _db.SaveChanges();

            return Ok(tweet);
        }


        //можно сколько хочешь лайкать от одного чувака + ещё надо, чтобы отменить лайк 
        //+ чтобы нельзя лайк і дізлайк одновременно

        //ПРАПАНОВА: стварыць прамежутачую табліцу UserReaction дзе апісваецца пастваіў лайк
        //ці дізлайк + камент к твіту
        //связь паміж User і Tweet праз яе - many-to-many
        [HttpPut("like-tweet")]
        public ActionResult WriteCommentToTweet([FromBody]JwtWtihObject<Guid> jwtWithTweetId)
        {
            JWT token;
            try
            {
                token = new JWT(jwtWithTweetId.JWT);
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
                tweetId = jwtWithTweetId.Object;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var tweet = _db.Tweets
                .FirstOrDefault(p => p.Id == tweetId);
            if (tweet == null)
                return NotFound("User doesn't have tweet with such Id");

            tweet.Likes += 1;
            _db.Tweets.Update(tweet);
            _db.SaveChanges();

            return Ok(tweet);
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