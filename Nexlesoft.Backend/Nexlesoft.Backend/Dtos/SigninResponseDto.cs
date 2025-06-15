using System.Text.Json.Serialization;

namespace Nexlesoft.Backend.Dtos
{
    public class SigninResponseDto 
    {
        public UserDto user { get; set; }
        public string token { get; set; }
        public string refreshToken { get; set; }

    }

    public class RefreshTokenResponseDto
    {
        public string token { get; set; }
        public string refreshToken { get; set; }
    }
}
