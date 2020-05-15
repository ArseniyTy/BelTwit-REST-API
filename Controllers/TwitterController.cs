using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BelTwit_REST_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


/*TwitterController:
*/


namespace BelTwit_REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwitterController : ControllerBase
    {
        private readonly BelTwitContext _db;
        private ILogger _logger;

        public TwitterController(BelTwitContext context, ILoggerFactory loggerFactory)
        {
            _db = context;
            _logger = loggerFactory.CreateLogger("DatabaseLogger");
        }

        
    }
}