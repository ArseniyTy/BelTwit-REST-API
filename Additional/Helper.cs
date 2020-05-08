using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BelTwit_REST_API.Models;

namespace BelTwit_REST_API.Additional
{
    public static class Helper
    {
        public static DbContextOptions<BelTwitContext> BelTwitDbOptions { get; set; }
    }
}
