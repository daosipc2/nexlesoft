using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Nexlesoft.Backend.Controllers;
using Nexlesoft.Backend.Dtos;
using Nexlesoft.Backend.Services.Interfaces;
using Nexlesoft.Domain.Entities;
using Nexlesoft.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nexlesoft.Backend.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<ILogger<UsersController>> _loggerMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _loggerMock = new Mock<ILogger<UsersController>>();
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_loggerMock.Object, _userServiceMock.Object);
            _controller.ModelState.Clear(); // Reset ModelState cho mỗi test
        }

        #region Signup Tests

        [Fact]
        public async Task Signup_InvalidModelState_Returns400()
        {
            // Arrange
            var signupDto = new SignupDto { Email = "invalid-email", Password = "short" };
            _controller.ModelState.AddModelError("Email", "Invalid email address.");
            _controller.ModelState.AddModelError("Password", "Password must be between 8 and 20 characters.");

            // Act
            var result = await _controller.Signup(signupDto);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var response = Assert.IsType<APIResponse>(badRequestResult.Value);
            Assert.False(response.Status);
            Assert.Equal("Validation failed.", response.Message);
            var errors = Assert.IsType<List<string>>(response.Result);
            Assert.Contains("Invalid email address.", errors);
            Assert.Contains("Password must be between 8 and 20 characters.", errors);
        }

        [Fact]
        public async Task Signup_UserAlreadyExists_Returns400()
        {
            // Arrange
            var signupDto = new SignupDto { Email = "tuan1@tuan.com", Password = "Password123!", FirstName = "Tuan", LastName = "Nguyen" };
            _userServiceMock.Setup(x => x.CreateUser(signupDto)).ThrowsAsync(new UserAlreadyExistsException("User already exists."));
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Signup(signupDto);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var response = Assert.IsType<APIResponse>(badRequestResult.Value);
            Assert.False(response.Status);
            Assert.Equal("User already exists.", response.Message);
        }

        [Fact]
        public async Task Signup_Success_Returns201()
        {
            // Arrange
            var signupDto = new SignupDto { Email = "tuan1@tuan.com", Password = "Password123!", FirstName = "Tuan", LastName = "Nguyen" };
            var user = new User { Id = 177, Email = "tuan1@tuan.com", FirstName = "Tuan", LastName = "Nguyen" };
            _userServiceMock.Setup(x => x.CreateUser(signupDto)).ReturnsAsync(user);
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Signup(signupDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, statusResult.StatusCode);
            var response = Assert.IsType<APIResponse>(statusResult.Value);
            Assert.True(response.Status);
            var userResult = Assert.IsType<dynamic>(response.Result);
            Assert.Equal(177, userResult.id);
            Assert.Equal("Tuan", userResult.firstName);
            Assert.Equal("Nguyen", userResult.lastName);
            Assert.Equal("tuan1@tuan.com", userResult.email);
            Assert.Equal("Tuan Nguyen", userResult.displayName);
        }

        [Fact]
        public async Task Signup_UnknownError_Returns500()
        {
            // Arrange
            var signupDto = new SignupDto { Email = "tuan1@tuan.com", Password = "Password123!", FirstName = "Tuan", LastName = "Nguyen" };
            _userServiceMock.Setup(x => x.CreateUser(signupDto)).ThrowsAsync(new Exception("Unknown error"));
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Signup(signupDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var response = Assert.IsType<APIResponse>(statusResult.Value);
            Assert.False(response.Status);
            Assert.Equal("Unknown Error.", response.Message);
        }

        #endregion

        #region Sigin Tests

        [Fact]
        public async Task Sigin_InvalidModelState_Returns400()
        {
            // Arrange
            var signinDto = new SigninDto { Email = "invalid-email", Password = "short" };
            _controller.ModelState.AddModelError("Email", "Invalid email address.");
            _controller.ModelState.AddModelError("Password", "Password must be between 8 and 20 characters.");

            // Act
            var result = await _controller.Sigin(signinDto);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var response = Assert.IsType<APIResponse>(badRequestResult.Value);
            Assert.False(response.Status);
            Assert.Equal("Validation failed.", response.Message);
            var errors = Assert.IsType<List<string>>(response.Result);
            Assert.Contains("Invalid email address.", errors);
            Assert.Contains("Password must be between 8 and 20 characters.", errors);
        }

        [Fact]
        public async Task Sigin_Success_Returns201()
        {
            // Arrange
            var signinDto = new SigninDto { Email = "tuan1@tuan.com", Password = "Password123!" };
            var signinResponse = new SigninResponseDto { token = "dummy-token", refreshToken = "dummy-refresh-token" };
            _userServiceMock.Setup(x => x.Signin(signinDto.Email, signinDto.Password)).ReturnsAsync(signinResponse);
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Sigin(signinDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, statusResult.StatusCode);
            var response = Assert.IsType<APIResponse>(statusResult.Value);
            Assert.True(response.Status);
            var resultDto = Assert.IsType<SigninResponseDto>(response.Result);
            Assert.Equal("dummy-token", resultDto.token);
            Assert.Equal("dummy-refresh-token", resultDto.refreshToken);
        }

        [Fact]
        public async Task Sigin_UnknownError_Returns500()
        {
            // Arrange
            var signinDto = new SigninDto { Email = "tuan1@tuan.com", Password = "Password123!" };
            _userServiceMock.Setup(x => x.Signin(signinDto.Email, signinDto.Password)).ThrowsAsync(new Exception("Unknown error"));
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Sigin(signinDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var response = Assert.IsType<APIResponse>(statusResult.Value);
            Assert.False(response.Status);
        }

        #endregion

        #region RefreshToken Tests

        [Fact]
        public async Task RefreshToken_InvalidModelState_Returns400()
        {
            // Arrange
            var refreshDto = new RefreshTokenDto { RefreshToken = "" };
            _controller.ModelState.AddModelError("RefreshToken", "Refresh token is required.");

            // Act
            var result = await _controller.RefreshToken(refreshDto);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var response = Assert.IsType<APIResponse>(badRequestResult.Value);
            Assert.False(response.Status);
            Assert.Equal("Refresh token is required.", response.Message);
            var errors = Assert.IsType<List<string>>(response.Result);
            Assert.Contains("Refresh token is required.", errors);
        }

        [Fact]
        public async Task RefreshToken_InvalidRefreshToken_Returns400()
        {
            // Arrange
            var refreshDto = new RefreshTokenDto { RefreshToken = "invalid-token" };
            var validateResponse = new APIResponse { Status = false, Message = "Invalid refresh token" };
            _userServiceMock.Setup(x => x.ValidateRefreshToken(refreshDto.RefreshToken)).ReturnsAsync(validateResponse);
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.RefreshToken(refreshDto);

            // Assert
            var badRequestResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            var response = Assert.IsType<APIResponse>(badRequestResult.Value);
            Assert.False(response.Status);
            Assert.Equal("Invalid refresh token", response.Message);
        }

        [Fact]
        public async Task RefreshToken_Success_Returns201()
        {
            // Arrange
            var refreshDto = new RefreshTokenDto { RefreshToken = "valid-token" };
            var validateResponse = new APIResponse { Status = true };
            var refreshResponse = new RefreshTokenResponseDto { token = "new-access-token", refreshToken = "new-refresh-token" };
            _userServiceMock.Setup(x => x.ValidateRefreshToken(refreshDto.RefreshToken)).ReturnsAsync(validateResponse);
            _userServiceMock.Setup(x => x.RefreshToken(refreshDto.RefreshToken)).ReturnsAsync(refreshResponse);
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.RefreshToken(refreshDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, statusResult.StatusCode);
            var response = Assert.IsType<APIResponse>(statusResult.Value);
            Assert.True(response.Status);
            var resultDto = Assert.IsType<RefreshTokenResponseDto>(response.Result);
            Assert.Equal("new-access-token", resultDto.token);
            Assert.Equal("new-refresh-token", resultDto.refreshToken);
        }

        [Fact]
        public async Task RefreshToken_UnknownError_Returns500()
        {
            // Arrange
            var refreshDto = new RefreshTokenDto { RefreshToken = "valid-token" };
            _userServiceMock.Setup(x => x.ValidateRefreshToken(refreshDto.RefreshToken)).ThrowsAsync(new Exception("Unknown error"));
            _controller.ModelState.Clear();

            // Act
            var result = await _controller.RefreshToken(refreshDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var response = Assert.IsType<APIResponse>(statusResult.Value);
            Assert.False(response.Status);
            Assert.Equal("An internal error occurred.", response.Message);
        }

        #endregion

        #region Signout Tests

        [Fact]
        public async Task Signout_InvalidUserIdClaim_Returns401()
        {
            // Arrange
            var claims = new[] { new System.Security.Claims.Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "") };
            var identity = new System.Security.Claims.ClaimsIdentity(claims);
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new System.Security.Claims.ClaimsPrincipal(identity) } };

            // Act
            var result = await _controller.Signout();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }       

        [Fact]
        public async Task Signout_UnknownError_Returns500()
        {
            // Arrange
            var claims = new[] { new System.Security.Claims.Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "177") };
            var identity = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims));
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = identity } };
            _userServiceMock.Setup(x => x.SignOut(177)).ThrowsAsync(new Exception("Unknown error"));

            // Act
            var result = await _controller.Signout();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var response = Assert.IsType<APIResponse>(statusResult.Value);
            Assert.False(response.Status);
            Assert.Equal("Unknown Error.", response.Message);
        }

        #endregion
    }   
}