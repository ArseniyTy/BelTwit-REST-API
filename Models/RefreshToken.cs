using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BelTwit_REST_API.Models
{
    public class RefreshToken
    {
        [Required(ErrorMessage = "Token must have a value")]
        public Guid TokenValue { get; set; }

        [Required(ErrorMessage = "Token must have an expiration time")]
        public DateTime ExpiresAt { get; set; }


        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
