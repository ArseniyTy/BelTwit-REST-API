using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class TweetIdWithRateJSON
    {
        public Guid TweetId { get; set; }
        public RateState RateState { get; set; }
    }
}
