using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class Tweet
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }


        public Guid UserIdRetweetedFrom { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }


        public virtual IList<Comment> TweetComments { get; set; }   
        public virtual IList<UserRateState> TweetRateStates { get; set; }   


        public Tweet() 
        {
            TweetComments = new List<Comment>();
            TweetRateStates = new List<UserRateState>();
        }
        public Tweet(Tweet tweetToCopy) : base()
        {
            if (tweetToCopy == null)
                throw new Exception("Tweet to copy from is null!");
            
            if(tweetToCopy.Content==null || tweetToCopy.Content.Length==0)
                throw new Exception("Tweet tweet must have a content");
            if (tweetToCopy.UserId == null)
                throw new Exception("Tweet tweet must have an owner");

            Id = new Guid();
            Content = tweetToCopy.Content;
            UserId = tweetToCopy.UserId;

            Likes = tweetToCopy.Likes;
            Dislikes = tweetToCopy.Dislikes;
            UserIdRetweetedFrom = tweetToCopy.UserIdRetweetedFrom;
        }
    }
}
