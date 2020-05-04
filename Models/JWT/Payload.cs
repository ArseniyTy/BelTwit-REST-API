using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using BelTwit_REST_API.Additional;

namespace BelTwit_REST_API.Models.JWT
{
    public class Payload : IEncoding
    {
        public Guid Sub { get; set; }
        public string Name { get; set; }
        //"admin": true

        public Payload(Guid subject, string name)
        {
            Sub = subject;
            Name = name;
        }

        public Payload(string encodedPayload)
        {
            byte[] payloadBytes = Convert.FromBase64String(encodedPayload);
            string payloadStr = Encoding.UTF8.GetString(payloadBytes);

            //чётные элементы и есть значения свойств (см ToString)
            string[] props = payloadStr.Split(',', ':').
                Where((item, index) => index % 2 != 0).
                ToArray();
            try
            {
                Sub = new Guid(props[0]);
                Name = props[1];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override string ToString()
        {
            return String.Format("sub:{0},name:{1}", Sub, Name);
        }

        public string GetBase64Encoding()
        {
            string payloadStr = this.ToString();
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payloadStr);
            return Convert.ToBase64String(payloadBytes);
        }
    }
}
