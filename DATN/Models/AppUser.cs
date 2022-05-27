using Microsoft.AspNetCore.Identity;

namespace DATN.Models
{
    public class AppUser : IdentityUser 
    {
        public string? ShippingAddress { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
