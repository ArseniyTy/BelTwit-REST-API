using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace BelTwit_REST_API.Tokens.JWT_token
{
    public class Payload : IEncoding
    {
        public Guid Sub { get; set; }   //user id
        public string Name { get; set; }    //user login
        public bool Admin { get; set; }     //if the user is admin
        public DateTime Exp { get; set; }   //expiration time

        public Payload(Guid subject, string name, bool admin)
        {
            Sub = subject;
            Name = name;
            Admin = admin;
            Exp = DateTime.Now.AddMinutes(30); //AddSeconds(20) - for testing
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
                Admin = bool.Parse(props[2]);
                Exp = DateTime.Parse(props[3].Replace('|', ':')); //заменяем |, которые мы установили в ToSting() для того, чтобы проісходіл нормальный Split здесь
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override string ToString()
        {
            return String.Format("sub:{0},name:{1},admin:{2},exp:{3}", Sub, Name, Admin, Exp.ToString().Replace(':','|'));
        }

        public string GetBase64Encoding()
        {
            string payloadStr = this.ToString();
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payloadStr);
            return Convert.ToBase64String(payloadBytes);
        }
    }
}
