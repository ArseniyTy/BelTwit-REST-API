using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using BelTwit_REST_API.Additional;

namespace BelTwit_REST_API.Models.JWT
{
    public class Header : IEncoding
    {
        public string Alg { get; set; }
        public string Typ { get; set; }

        public Header()
        {
            Alg = "HS256";
            Typ = "JWT";
        }

        public Header(string encodedHeader)
        {
            Alg = "HS256";
            Typ = "JWT";



            byte[] headerBytes = Convert.FromBase64String(encodedHeader);
            string headerStr = Encoding.UTF8.GetString(headerBytes);

            //чётные элементы и есть значения свойств (см ToString)
            string[] props = headerStr.Split(',', ':').
                Where((item, index) => index % 2 != 0).
                ToArray();
            try
            {
                string alg = props[0];
                string typ = props[1];
                if (Alg != alg || Typ != typ)
                    throw new Exception("Header: Algorithm value or Type value are incorrect");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override string ToString()
        {
            return String.Format("alg:{0},typ:{1}", Alg, Typ);
        }

        public string GetBase64Encoding()
        {
            string headerStr = this.ToString();
            byte[] headerBytes = Encoding.UTF8.GetBytes(headerStr);
            return Convert.ToBase64String(headerBytes);
        }
    }
}
