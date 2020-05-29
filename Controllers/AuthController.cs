using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BelTwit_REST_API.Models;
using BelTwit_REST_API.Tokens.JWT_token;
using BelTwit_REST_API.Additional;
using BelTwit_REST_API.Tokens;
using Microsoft.Extensions.Logging;
using BelTwit_REST_API.Logging;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BelTwit_REST_API.ModelsJSON;
using Microsoft.AspNetCore.Authorization;

/*General:
 * Разделение на роли: у админа должна быть возможность редактировать все поля
 * http://localhost:port main get request -> инфа о сервисе (документация) - лучше head
 * 
 * твиттер
 - у юзера есть возможность добавлять твит
 - подписываться на других юзеров
 - получать все твиты на подписаных юзеров
 - ретвит
 - комменты и лайки к твитам
 */



namespace BelTwit_REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BelTwitContext _db;
        private ILogger _logger;

        public AuthController(BelTwitContext context, ILoggerFactory loggerFactory)
        {
            _db = context;
            _logger = loggerFactory.CreateLogger("DatabaseLogger");
        }


        
        [HttpOptions]
        public ActionResult AuthControllerOptions()
        {
            Response.Headers.Add("Allow", "GET, POST");
            return Ok();
        }
        [HttpPost]
        public ActionResult AuthentificateUser([FromBody]User user)
        {
            var userFromDb = _db.Users.
                FirstOrDefault(u => u.Login == user.Login);
            if (userFromDb == null)
                return NotFound("There is no such a user");
            if (userFromDb.Password != SecurityService.GetHash(user.Password,userFromDb.PasswordSalt))
            {
                string ex = "Password is incorrect";
                _logger.LogError($"[POST]api/auth/authentificate" + ex);
                return Forbid(ex);
            }


            var token = new AccessRefreshToken(userFromDb).ParseToJSON();

            _logger.LogInformation($"[DELETE]api/auth/admin-delete;" +
                                   $"User [{userFromDb.Login}] was authentificated");
            return Ok(token);
        }


        [HttpHead]
        public ActionResult AuthorizeUserHead([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GET]api/auth/authorize" + ex);
                return BadRequest();
            }

            return Ok();
        }
        [HttpGet]
        public ActionResult AuthorizeUser([FromBody]string accessToken)
        {
            JWT token;
            try
            {
                token = new JWT(accessToken);
            }
            catch(Exception ex)
            {
                _logger.LogError($"[GET]api/auth/authorize" + ex);
                return BadRequest(ex.Message);
            }

            return Ok(token);
            //return Ok(token.GetBase64Encoding());
        }



        [HttpOptions("update-tokens")]
        public ActionResult RefreshTokensOptions()
        {
            Response.Headers.Add("Allow", "POST");
            return Ok();
        }
        //можно передавать любой старый access, і он будет обновляться
        //но это і не важно. Тут должна гарантіроваться лішь авторізованность
        //і в целом хакер не сможет получіть доступ к update-tokens
        [HttpPost("update-tokens")]
        public ActionResult RefreshTokens([FromBody]AccessRefreshTokenJSON tokenJSON)
        {
            AccessRefreshToken token;
            try
            {
                //ne proveryem srok godnosti
                token = new AccessRefreshToken(tokenJSON, CheckForExpiration: false);
                token.UpdateTokens();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[POST]api/auth/update-tokens" + ex);
                return BadRequest(ex.Message);
            }

            return Ok(token.ParseToJSON());
        }
    }
}