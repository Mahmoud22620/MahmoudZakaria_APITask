using System;
using Microsoft.AspNetCore.Identity;

namespace MahmoudZakaria_APITask.Models
{
    public class User : IdentityUser
    {
        public DateTime? LastLoginTime { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
