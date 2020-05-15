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
 * POST(jwt) - add tweet
 * DELETE(jwt) - remove tweet
 * PUT(jwt) - change tweet
 * GET(jwt) - get one tweet
 * GET(jwt) - get all tweets
 * 
 * GET(login) - get tweets of the other person 
 * POST(Id tweet-a) - write comment
 * POST(Id) - put like tweet
 * POST(Id) - put dislike tweet
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

            var tweetJSON = jwtWithTweet.Object;
            //в ідеале clone ілі конструктор
            var tweet = new Tweet
            {
                Id = new Guid(),
                Content = tweetJSON.Content,
                Comments = tweetJSON.Comments,
                Likes = tweetJSON.Likes,
                Dislikes = tweetJSON.Dislikes,
                UserIdRetweetedFrom = tweetJSON.UserIdRetweetedFrom,

                UserId = token.PAYLOAD.Sub
            };
            _db.Tweets.Add(tweet);
            _db.SaveChanges();

            return Ok(tweet);
        }
    }
}