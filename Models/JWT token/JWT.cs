using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using BelTwit_REST_API.Additional;

namespace BelTwit_REST_API.Models.JWT_token
{
    public class JWT : IEncoding
    {
        public Header HEADER { get; set; }
        public Payload PAYLOAD { get; set; }
        public Signature SIGNATURE { get; set; }


        public JWT(User user)
        {
            HEADER = new Header();
            PAYLOAD = new Payload(user.Id, user.Login);
            SIGNATURE = new Signature(HEADER, PAYLOAD);
        }

        public JWT(string encodedJWT)
        {
            string[] props = encodedJWT.Split('.');
            try
            {
                HEADER = new Header(props[0]);
                PAYLOAD = new Payload(props[1]);
                SIGNATURE = new Signature(props[2]);

                var SignatureCorrect = new Signature(HEADER, PAYLOAD);
                if(SIGNATURE.Token!= SignatureCorrect.Token)
                {
                    throw new Exception("Signatures do not match!!!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string GetBase64Encoding()
        {
            return $"{HEADER.GetBase64Encoding()}." +
                    $"{PAYLOAD.GetBase64Encoding()}." +
                    $"{SIGNATURE.GetBase64Encoding()}";
        }


        public bool IsTokenExpired()
        {
            if (PAYLOAD.Exp > DateTime.Now)
                return false;

            return true;
        }
    }
}
