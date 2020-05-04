using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using BelTwit_REST_API.Additional;

namespace BelTwit_REST_API.Models.JWT
{
    public class Signature : IEncoding
    {
        private const string SecretKey = "MySecret";
        public string Token { get; set; }

        public Signature(Header header, Payload payload)
        {
            string data = $"{header.GetBase64Encoding()}.{payload.GetBase64Encoding()}";
            Token = SecurityService.GetHash(data, SecretKey);
        }

        public Signature(string encodedSignature)
        {
            byte[] tokenBytes = Convert.FromBase64String(encodedSignature);
            string tokenStr = Encoding.UTF8.GetString(tokenBytes);

            Token = tokenStr;
        }


        public override string ToString()
        {
            return Token;
        }
        public string GetBase64Encoding()
        {
            string tokenStr = this.ToString();
            byte[] tokenBytes = Encoding.UTF8.GetBytes(tokenStr);
            return Convert.ToBase64String(tokenBytes);
        }
    }
}
