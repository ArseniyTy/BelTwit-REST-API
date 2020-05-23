using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public enum RateState
    {
        Dislike=-1,
        None,
        Like        
    }
    public class UserRateState
    {
        public RateState RateState { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }


        public Guid TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}
