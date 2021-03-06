using System.ComponentModel.DataAnnotations;

namespace DATN.Models
{
    public class RegisterModel
    {

        public string FullName{ get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Location is required")]
        public string? ShippingAddress{ get; set; }

       
    }
}
