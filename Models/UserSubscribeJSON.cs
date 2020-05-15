using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class UserSubscribeJSON
    {
        public string JWT { get; set; }
        public string OtherUserLogin { get; set; }
    }
}
