using System.ComponentModel.DataAnnotations;

namespace ClassFileBackEnd.Mapper
{
    public class AccountSignupDTO
    {
        private string? username;
        private string? password;
        private string? password2;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 5)]
        public string? Username { get { return username; } set { username = value; } }
        [Required(ErrorMessage = "Password is required")]
        [StringLength(30, ErrorMessage = "Must be between 5 and 30 characters", MinimumLength = 5)]
        public string? Password { get { return password; } set { password = value; } }
        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(30, ErrorMessage = "Must be between 5 and 30 characters", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password is not match")]
        public string? Password2 { get { return password2; } set { password2 = value; } }
    }
}
