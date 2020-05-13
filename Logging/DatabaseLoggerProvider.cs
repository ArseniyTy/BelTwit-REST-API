using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BelTwit_REST_API.Models;

namespace BelTwit_REST_API.Logging
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private BelTwitContext _context;
        public DatabaseLoggerProvider(BelTwitContext context)
        {
            _context = context;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(_context);
        }

        public void Dispose() { }
    }
}
