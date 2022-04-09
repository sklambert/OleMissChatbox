using System.ComponentModel.DataAnnotations;

namespace OleMissChatbox.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
