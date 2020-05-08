using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BelTwit_REST_API.Models.JWT_token;
using BelTwit_REST_API.Additional;

namespace BelTwit_REST_API.Models
{
    public class AccessRefreshToken
    {
        public RefreshToken RefreshToken { get; set; }
        public JWT AccessToken { get; set; }


        public AccessRefreshToken(AccessRefreshTokenJSON tokenJSON) : 
            this(tokenJSON.AccessToken, tokenJSON.RefreshToken) { }

        public AccessRefreshToken(string accessToken, string refreshToken)
        {            
            using(var _db = new BelTwitContext(Helper.BelTwitDbOptions))
            {
                RefreshToken = _db.RefreshTokens.
                    First(p => p.TokenValue == new Guid(refreshToken));

                AccessToken = new JWT(accessToken);
            }
            
        }
        public AccessRefreshToken(RefreshToken refresh, JWT access)
        {
            RefreshToken = refresh;
            AccessToken = access;
        }

        public bool IsTokenExpired()
        {
            if (RefreshToken.ExpiresAt > DateTime.Now)
                return false;

            return true;
        }
    }
}
