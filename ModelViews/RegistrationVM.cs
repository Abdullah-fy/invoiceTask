using System.ComponentModel.DataAnnotations;

namespace itRoot.ModelViews
{
    public class RegistrationVM
    {
        [Required]
        [StringLength(20)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Full name must contain letters only.")]
        public string FullName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Phone]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Please complete the reCAPTCHA")]
        public string RecaptchaToken { get; set; }
    }
}
