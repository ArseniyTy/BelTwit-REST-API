using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class Comment
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }


        public Guid TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}
