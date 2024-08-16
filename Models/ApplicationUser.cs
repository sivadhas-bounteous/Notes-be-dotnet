using Microsoft.AspNetCore.Identity;

namespace Notes.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; } = string.Empty;
    }
}
