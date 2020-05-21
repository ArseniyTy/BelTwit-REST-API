using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public enum LikeState
    {
        Dislike=-1,
        None,
        Like        
    }
    public class UserLikeState
    {
        public LikeState LikeState { get; set; }
        public Guid UserId { get; set; }


        public Guid TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}
