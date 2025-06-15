using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nexlesoft.Application.Interfaces;
using Nexlesoft.Backend.Dtos;
using Nexlesoft.Backend.Services.Interfaces;
using Nexlesoft.Domain.Entities;
using Nexlesoft.Domain.Exceptions;
using Nexlesoft.Infrastructure.Data;
using Nexlesoft.Infrastructure.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Nexlesoft.Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenRepository _tokenRepository;

        private readonly IConfiguration _configuration;

        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, ITokenRepository tokenRepository, IConfiguration configuration, IMapper mapper)
        {
            this._userRepository = userRepository;
            this._tokenRepository = tokenRepository;

            // _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<User> CreateUser(UserDto userDto)
        {
            var existed = await _userRepository.CheckUerExisted(userDto.Email);
            if (existed)
            {
                throw new UserAlreadyExistsException("User already exists.");
            }

            var user = _mapper.Map<User>(userDto);
            // update CreatedAt and UpdatedAt 
            user.CreatedAt = user.UpdatedAt = DateTime.UtcNow;

            //var result = await _userManager.CreateAsync(user, userDto.Password);
            var hashedPassword = PasswordHasher.HashPassword(userDto.Password);
            user.Hash = hashedPassword;
            try
            {
                // add user to DB
                user = await _userRepository.AddAsync(user);
            }
            catch (Exception ex)
            {
                // log error

                return null;
            }

            //if (!result.Succeeded) throw new Exception(string.Join("\r\n ", result.Errors.Select(e => e.Description)));
            //return result.Succeeded;

            return user;
        }


        public async Task<SigninResponseDto> Signin(string username, string password)
        {
            SigninResponseDto result;


            var user = await _userRepository.GetByEmailAsync(username);
            if (user == null)
                throw new Exception("User not found.");

            var hashedPassword = user.Hash;

            if (!PasswordHasher.VerifyPassword(password, hashedPassword))
            {
                throw new Exception("Invalid username or password.");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            if (user.Tokens.Count == 0)
            {
                // create new token  if not existed.
                var tokenEntity = new Token
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresIn = DateTime.UtcNow.AddDays(30).ToString()
                };
                await _tokenRepository.AddAsync(tokenEntity);
            }
            else
            {
                // update token
                await _tokenRepository.UpdateRefreshToken(user.Tokens.FirstOrDefault().Id, refreshToken);
            }

            result = new SigninResponseDto
            {
                user = _mapper.Map<UserDto>(user),
                token = token,
                refreshToken = refreshToken,
            };

            return result;
        }
        public async Task<bool> SignOut(string refreshToken)
        {
            await _tokenRepository.DeleteRefreshToken(refreshToken);

            return true;
        }
        public async Task<RefreshTokenResponseDto> RefreshToken(string refreshToken)
        {
            var tokenEntity = await _tokenRepository.GetByRefreshToken(refreshToken);           

            var user = await _userRepository.GetByIdAsync(tokenEntity.UserId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            var newToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            await _tokenRepository.UpdateRefreshToken(tokenEntity.Id, newRefreshToken);

            return new RefreshTokenResponseDto
            {                
                token = newToken,
                refreshToken = newRefreshToken
            };
        }
        public async Task<APIResponse> ValidateRefreshToken(string refreshToken)
        {
            APIResponse response = new APIResponse();
            response.Status = true;
            var tokenEntity = await _tokenRepository.GetByRefreshToken(refreshToken);
            if (tokenEntity == null)
            {
                response.Status = false;
                response.Message = "Refresh token is not found.";
            }
            else if (tokenEntity != null && string.IsNullOrEmpty(tokenEntity.ExpiresIn))
            {
                // validate refreshToken expiration
                DateTime oDT = DateTime.MinValue;
                DateTime.TryParse(tokenEntity.ExpiresIn, out oDT);
                if (DateTime.UtcNow < oDT)
                {
                    response.Status = false;
                    response.Message = "Refresh token is expired.";
                }
            }

            return response;
        }

        public async Task<List<UserDto>> Getall()
        {
            throw new NotImplementedException();
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            int expiredHrs;

            if (int.TryParse(_configuration["Jwt:ExpiredHrs"], out expiredHrs))
                expiredHrs = 1;
            var nowUtc = DateTimeOffset.UtcNow;
            var expiration = nowUtc.AddHours(expiredHrs); // is a new token which will expire in one hour
            var exp = expiration.ToUnixTimeSeconds();
            var now = nowUtc.ToUnixTimeSeconds();

            var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiredHrs),
            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            string refreshtoken = Convert.ToBase64String(randomNumber);
            return refreshtoken;
        }


    }
}
