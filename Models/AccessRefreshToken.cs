﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class AccessRefreshToken
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
