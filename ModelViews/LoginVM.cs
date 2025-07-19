using System.ComponentModel.DataAnnotations;

namespace itRoot.ModelViews
{
    public class LoginVM
    {
        public string username{ get; set; }
        public string password { get; set; }

        [Required(ErrorMessage = "Please complete the reCAPTCHA")]
        public string RecaptchaToken { get; set; }
    }
}
