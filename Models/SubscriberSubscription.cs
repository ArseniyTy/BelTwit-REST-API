using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class SubscriberSubscription
    {
        public Guid WhoSubscribeId { get; set; }
        public virtual User WhoSubscribe { get; set; }


        public Guid OnWhomSubscribeId { get; set; }
        public virtual User OnWhomSubscribe { get; set; }
    }
}
