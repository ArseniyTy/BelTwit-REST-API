using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class Reaction
    {
        public bool IsLike { get; set; }
        public bool IsDislike { get; set; }
        public bool IsRetweeted { get; set; }
        public string Comment { get; set; }


        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}
