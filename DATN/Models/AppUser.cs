using Microsoft.AspNetCore.Identity;

namespace DATN.Models
{
    public class AppUser : IdentityUser 
    {
        public string FullName { get; set; }
        public string? AccoutType { get; set; }
        public string? Avatar { get; set; }
        public string? ShippingAddress { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<Cart>? Carts { get; set; }
        public List<WishList>? WishLists { get; set; }

        public bool? IsLocked { get; set; }
    }
}
