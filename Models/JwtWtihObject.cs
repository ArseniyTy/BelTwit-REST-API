using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class JwtWtihObject<T>
    {
        public string JWT { get; set; }
        public T Object { get; set; }
    }
}
