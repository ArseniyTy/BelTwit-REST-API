using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BelTwit_REST_API.Tokens.JWT_token;
using BelTwit_REST_API.Additional;
using BelTwit_REST_API.Models;

namespace BelTwit_REST_API.Tokens
{
    public class AccessRefreshToken
    {
        public RefreshToken RefreshToken { get; set; }
        public JWT AccessToken { get; set; }


        //ссылается на ніжній
        public AccessRefreshToken(AccessRefreshTokenJSON tokenJSON) : 
            this(tokenJSON.AccessToken, tokenJSON.RefreshToken) { }

        public AccessRefreshToken(string accessToken, string refreshToken)
        {            
            using(var _db = new BelTwitContext(Helper.BelTwitDbOptions))
            {
                RefreshToken = _db.RefreshTokens.
                    FirstOrDefault(p => p.TokenValue == new Guid(refreshToken));
                if (RefreshToken == null)
                    throw new Exception("Refesh token doesn't exist in database");

                AccessToken = new JWT(accessToken);
            }
            
        }
        public AccessRefreshToken(User user)
        {
            AccessToken = new JWT(user);

            using (var _db = new BelTwitContext(Helper.BelTwitDbOptions))
            {
                //еслі больше 5 токенов, то удаляем (безопасность!)
                var userRefreshTokens = _db.RefreshTokens.
                    Where(p => p.UserId == user.Id)
                    .ToList();
                if (userRefreshTokens.Count > 5)
                    _db.RefreshTokens.RemoveRange(userRefreshTokens);



                RefreshToken = new RefreshToken
                {
                    TokenValue = new Guid(),
                    ExpiresAt = DateTime.Now.AddDays(60), //AddSeconds(20) - for testing 
                    UserId = user.Id
                };
                _db.RefreshTokens.Add(RefreshToken);
                _db.SaveChanges();
            }
        }

        public AccessRefreshTokenJSON ParseToJSON()
        {
            return new AccessRefreshTokenJSON
            {
                AccessToken = AccessToken.GetBase64Encoding(),
                RefreshToken = RefreshToken.TokenValue.ToString()
            };
        }

        public bool IsTokenExpired()
        {
            //Refresh token не может истечь раньше access, поэтому тут только одна проверка
            return AccessToken.IsTokenExpired();
        }

        public void UpdateTokens()
        {
            //подгружается User от нужного Refresh (нужно для обновленія токенов)
            //лібо для удаления из БД при expire Refresh-а
            using (var _db = new BelTwitContext(Helper.BelTwitDbOptions))
            {
                var refreshFromDb = _db.RefreshTokens.
                    FirstOrDefault(p => p.TokenValue == RefreshToken.TokenValue);
                if (refreshFromDb == null)
                    throw new Exception("Refresh token doesn't exist in database anymore");
                RefreshToken = refreshFromDb;

                _db.Entry(RefreshToken).Reference(p => p.User).Load();
                if (RefreshToken.User == null)
                    throw new Exception("User of this token doesn't exist in database anymore");
            }


            if (RefreshToken.ExpiresAt < DateTime.Now)
            {
                //удаляем із бд его
                using (var _db = new BelTwitContext(Helper.BelTwitDbOptions))
                {
                    _db.RefreshTokens.Remove(RefreshToken);
                    _db.SaveChanges();
                }
                throw new Exception("Refresh token expired. Authentificate again!");
            }


            //при обновлении Access обновляется і Refresh
            //expiration у Refresh нужен только лішь для случаю отсутсвія в сеті 60 дней (тогда нужно вводіть пароль)
            if (AccessToken.IsTokenExpired())
            {
                AccessToken = new JWT(RefreshToken.User);


                using (var _db = new BelTwitContext(Helper.BelTwitDbOptions))
                {
                    _db.RefreshTokens.Remove(RefreshToken);
                    var refreshToken = new RefreshToken
                    {
                        TokenValue = new Guid(),
                        ExpiresAt = DateTime.Now.AddDays(60),
                        UserId = RefreshToken.UserId,
                        User = RefreshToken.User
                    };
                    _db.RefreshTokens.Add(refreshToken);
                    _db.SaveChanges();

                    RefreshToken = refreshToken;
                }
            }
        }
    }
}
