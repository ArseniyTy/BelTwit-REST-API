using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class TweetIdWithCommentJSON
    {
        public Guid TweetId { get; set; }
        public string Comment { get; set; }
    }
}
