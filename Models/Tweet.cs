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
        public string CommentsDb { get; set; }
        //not storing but easier to work with
        [NotMapped]
        public string[] Comments
        {
            get { return CommentsDb.Split(';'); }
            set { CommentsDb = string.Join(";", value); }
        }



        public Guid UserIdRetweetedFrom { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
