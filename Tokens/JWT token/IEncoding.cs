using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Tokens.JWT_token
{
    interface IEncoding
    {
        string GetBase64Encoding();
    }
}
