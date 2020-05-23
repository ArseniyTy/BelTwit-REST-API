using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.ModelsJSON
{
    public class TweetIdWithObject<T>
    {
        public Guid TweetId { get; set; }
        public T WithTweetObject { get; set; }
    }
}
