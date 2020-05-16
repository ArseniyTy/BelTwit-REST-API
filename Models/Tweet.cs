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


        //storing in Database
        private string _commentsDb { get; set; }
        //not storing but easier to work with
        [NotMapped]
        public string[] Comments
        {
            get 
            {
                if (_commentsDb == null)
                    return null;
                return _commentsDb.Split(';');
            }
            set 
            {
                if (value == null)
                    _commentsDb = null;
                else
                    _commentsDb = string.Join(";", value); 
            }
        }



        public Guid UserIdRetweetedFrom { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }


        public Tweet() { }
        public Tweet(Tweet tweetToCopy)
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

            Comments = tweetToCopy.Comments;
            Likes = tweetToCopy.Likes;
            Dislikes = tweetToCopy.Dislikes;
            UserIdRetweetedFrom = tweetToCopy.UserIdRetweetedFrom;
        }
    }
}
