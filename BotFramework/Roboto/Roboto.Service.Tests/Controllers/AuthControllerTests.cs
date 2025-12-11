using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using Roboto.Models;
using Roboto.Dtos;
using Roboto.Repository;
using Roboto.Service.Auth;
using Roboto.Service.Controllers;
using Roboto.Service.Extensions;
using Roboto.Service.Services;

namespace Roboto.Service.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private AuthController _authController;
        private Mock<IAuthService> _mockAuthService;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>();

            _authController = new AuthController(
                _mockAuthService.Object
            );
        }

        #region Register Tests

        [Test]
        public async Task Register_ValidUser_ReturnsCreated()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "password123",
                FirstName = "John",
                LastName = "Doe"
            };

            var user = new User
            {
                Id = 1,
                Username = "newuser",
                Email = "newuser@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockAuthService.Setup(s => s.RegisterUserAsync(registerDto))
                .ReturnsAsync((true, null, user));

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<CreatedResult>(result);
            _mockAuthService.Verify(s => s.RegisterUserAsync(registerDto), Times.Once);
        }

        [Test]
        public async Task Register_ExistingUsername_ReturnsConflict()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "existinguser",
                Email = "new@example.com",
                Password = "password123"
            };

            _mockAuthService.Setup(s => s.RegisterUserAsync(registerDto))
                .ReturnsAsync((false, "Username or email already in use", null));

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<ConflictObjectResult>(result);
            _mockAuthService.Verify(s => s.RegisterUserAsync(registerDto), Times.Once);
        }

        [Test]
        public async Task Register_ExistingEmail_ReturnsConflict()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "existing@example.com",
                Password = "password123"
            };

            _mockAuthService.Setup(s => s.RegisterUserAsync(registerDto))
                .ReturnsAsync((false, "Username or email already in use", null));

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<ConflictObjectResult>(result);
        }

        #endregion

        #region Login Tests

        [Test]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UsernameOrEmail = "testuser",
                Password = "password123"
            };

            _mockAuthService.Setup(s => s.AuthenticateUserAsync(loginDto))
                .ReturnsAsync((true, "mock-jwt-token"));

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        [Test]
        public async Task Login_UserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UsernameOrEmail = "nonexistent",
                Password = "password123"
            };

            _mockAuthService.Setup(s => s.AuthenticateUserAsync(loginDto))
                .ReturnsAsync((false, null));

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UsernameOrEmail = "testuser",
                Password = "wrongpassword"
            };

            _mockAuthService.Setup(s => s.AuthenticateUserAsync(loginDto))
                .ReturnsAsync((false, null));

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
        }

        [Test]
        public async Task Login_WithEmail_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UsernameOrEmail = "test@example.com",
                Password = "password123"
            };

            _mockAuthService.Setup(s => s.AuthenticateUserAsync(loginDto))
                .ReturnsAsync((true, "mock-jwt-token"));

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion
    }
}