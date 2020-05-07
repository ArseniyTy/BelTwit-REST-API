using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BelTwit_REST_API.Models.JWT_token;

namespace BelTwit_REST_API.Models
{
    public class AccessRefreshToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        private RefreshToken _RefreshToken { get; set; }
        private JWT _AccessToken { get; set; }

        public AccessRefreshToken(RefreshToken refresh, JWT access)
        {
            _RefreshToken = refresh;
            _AccessToken = access;

            RefreshToken = _RefreshToken.TokenValue.ToString();
            AccessToken = _AccessToken.GetBase64Encoding();
        }
        public bool IsTokenExpired()
        {
            if (_RefreshToken.ExpiresAt > DateTime.Now)
                return true;

            return false;
        }
    }
}
