using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.ModelsJSON
{
    public class JwtWtihObject<T>
    {
        public string JWT { get; set; }
        public T WithJWTObject { get; set; }
    }
}
