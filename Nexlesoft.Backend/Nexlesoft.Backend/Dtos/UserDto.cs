using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Nexlesoft.Backend.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^.{8,20}$", ErrorMessage = "Password must be between 8 and 20 characters.")]
        public string Password { get; set; }
    }

    public class SignupDto : UserDto
    {
        [JsonIgnore]
        public new int Id { get; set; }
    }

    public class SigninDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^.{8,20}$", ErrorMessage = "Password must be between 8 and 20 characters.")]
        public string Password { get; set; }
    }
}
