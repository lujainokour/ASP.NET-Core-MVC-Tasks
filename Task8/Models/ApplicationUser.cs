using Microsoft.AspNetCore.Identity;

namespace UniversityCafeteriaSupply8.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
    }
}