using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.ModelsJSON
{
    public class AccessRefreshTokenJSON
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
