using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    [DataContract]
    public class Comment
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public string Content { get; set; }


        [DataMember]
        public Guid? UserId { get; set; }
        public User User { get; set; }


        public Guid TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}
