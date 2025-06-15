using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nexlesoft.Backend.Dtos;
using Nexlesoft.Backend.Services.Interfaces;
using Nexlesoft.Domain.Entities;
using Nexlesoft.Domain.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using static System.Net.WebRequestMethods;

namespace Nexlesoft.Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserService _userService;
        public UsersController(ILogger<UsersController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupDto userRegister)
        {
            APIResponse response = new APIResponse();

            if (!ModelState.IsValid)
            {
                response.Status = false;
                response.Message = "Validation failed.";
                response.Result = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return StatusCode(400, response); // 400
            }

            try
            {
                var user = await this._userService.CreateUser(userRegister);
                if (user != null)
                {
                    response.Status = true;
                    var responUser = new
                    {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        displayName = user.FirstName + " " + user.LastName
                    };

                    response.Result = responUser;
                }
                else
                {
                    response.Status = false;
                }
            }
            catch (UserAlreadyExistsException ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                return BadRequest(response); // 400
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Unknown Error.";
                // return staus code 500 
                return StatusCode(500, response);
            }

            // return status code 201 
            return StatusCode(201, response);
        }

        [HttpPost("sigin")]
        public async Task<IActionResult> Sigin(SigninDto signinDto)
        {
            APIResponse response = new APIResponse();

            if (!ModelState.IsValid)
            {
                response.Status = false;
                response.Message = "Validation failed.";
                response.Result = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(response); // 400
            }

            try
            {
                var responseDto = await this._userService.Signin(signinDto.Email, signinDto.Password);
                response.Status = true;
                response.Result = responseDto;
            }
            catch (Exception ex)
            {
                response.Status = false;
                // return staus code 500 
                return StatusCode(500, response);
            }


            return StatusCode(201, response);
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshDto)
        {
            var response = new APIResponse();

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(refreshDto.RefreshToken))
            {
                response.Status = false;
                response.Message = "Refresh token is required.";
                response.Result = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(response); // 400
            }

            try
            {
                var validateResponse = await _userService.ValidateRefreshToken(refreshDto.RefreshToken);
                if (!validateResponse.Status)
                {
                    return BadRequest(validateResponse); // 400
                }

                var result = await _userService.RefreshToken(refreshDto.RefreshToken);
                response.Status = true;
                response.Result = result;
                return StatusCode(201, response); // 201
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "An internal error occurred.";
                return StatusCode(500, response);
            }
        }

        [Authorize]
        [HttpPost("signout")]
        public async Task<IActionResult> Signout()
        {
            APIResponse response = new APIResponse();            

            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            // var emailClaim = User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(); // 401

            await _userService.SignOut(userId);

            response.Status = true;
            return StatusCode(201, response);
        }
    }
}
