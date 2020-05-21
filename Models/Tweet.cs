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


        ////storing in Database
        //public string CommentsDb { get; set; }
        ////not storing but easier to work with
        //[NotMapped]
        //public List<string> Comments
        //{
        //    get 
        //    {
        //        if (CommentsDb == null)
        //            return new List<string>();
        //        return CommentsDb.Split(';').ToList();
        //    }
        //    set 
        //    {
        //        if (value == null)
        //            CommentsDb = null;
        //        else
        //            CommentsDb = string.Join(";", value); 
        //    }
        //}


        public Guid UserIdRetweetedFrom { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }


        public virtual IList<Comment> TweetComments { get; set; }   //твіты
        public virtual IList<UserLikeState> TweetLikeStates { get; set; }   


        public Tweet() 
        {
            TweetComments = new List<Comment>();
            TweetLikeStates = new List<UserLikeState>();
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

            //Comments = tweetToCopy.Comments;
            Likes = tweetToCopy.Likes;
            Dislikes = tweetToCopy.Dislikes;
            UserIdRetweetedFrom = tweetToCopy.UserIdRetweetedFrom;
        }
    }
}
