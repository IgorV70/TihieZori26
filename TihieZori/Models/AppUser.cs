using Microsoft.AspNetCore.Identity;

namespace TihieZori.Models
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ProfileImageUrl { get; set; }
    }
}