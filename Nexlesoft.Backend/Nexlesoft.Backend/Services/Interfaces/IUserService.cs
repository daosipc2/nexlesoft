using Nexlesoft.Backend.Dtos;
using Nexlesoft.Domain.Entities;

namespace Nexlesoft.Backend.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUser(UserDto userDto);
        Task<SigninResponseDto> Signin(string username, string password);

        Task<bool> SignOut(int userId);
        Task<List<UserDto>> Getall();
        Task<RefreshTokenResponseDto> RefreshToken(string refreshToken);
        Task<APIResponse> ValidateRefreshToken(string refreshToken);
    }
}
