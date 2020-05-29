using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class Log
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string LogLevel { get; set; }
        public string Path { get; set; }
        public string Action { get; set; }
    }
}
