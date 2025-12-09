using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using Roboto.Models;
using Roboto.Models.Dto;
using Roboto.Repository;
using Roboto.Service.Auth;
using Roboto.Service.Controllers;
using Roboto.Service.Services;

namespace Roboto.Service.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private AuthController _authController;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IPasswordHasher> _mockPasswordHasher;
        private Mock<IJwtService> _mockJwtService;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ILinkService> _mockLinkService;
        private Mock<LinkGenerator> _mockLinkGenerator;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockJwtService = new Mock<IJwtService>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLinkService = new Mock<ILinkService>();
            _mockLinkGenerator = new Mock<LinkGenerator>();

            _authController = new AuthController(
                _mockUserRepository.Object,
                _mockPasswordHasher.Object,
                _mockJwtService.Object,
                _mockMapper.Object,
                _mockHttpContextAccessor.Object,
                _mockLinkService.Object,
                _mockLinkGenerator.Object
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

            _mockUserRepository.Setup(r => r.UsernameOrEmailExistsAsync(registerDto.Username, registerDto.Email))
                .ReturnsAsync(false);
            _mockPasswordHasher.Setup(p => p.HashPassword(registerDto.Password))
                .Returns(("hashedPassword", "salt"));
            _mockUserRepository.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<CreatedResult>(result);
            _mockUserRepository.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
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

            _mockUserRepository.Setup(r => r.UsernameOrEmailExistsAsync(registerDto.Username, registerDto.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<ConflictObjectResult>(result);
            _mockUserRepository.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
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

            _mockUserRepository.Setup(r => r.UsernameOrEmailExistsAsync(registerDto.Username, registerDto.Email))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<ConflictObjectResult>(result);
        }

        [Test]
        public async Task Register_EmptyUsername_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "",
                Email = "test@example.com",
                Password = "password123"
            };

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Register_EmptyPassword_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = ""
            };

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Register_EmptyEmail_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Email = "",
                Password = "password123"
            };

            // Act
            var result = await _authController.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
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

            var user = new User("testuser", "test@example.com", "hashedPassword", "salt")
            {
                Id = 1
            };

            _mockUserRepository.Setup(r => r.GetUserByUsernameOrEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(p => p.VerifyPassword(loginDto.Password, user.PasswordHash, user.Salt))
                .Returns(true);
            _mockJwtService.Setup(j => j.GenerateToken(user))
                .Returns("mock-jwt-token");

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

            _mockUserRepository.Setup(r => r.GetUserByUsernameOrEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync((User)null);

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

            var user = new User("testuser", "test@example.com", "hashedPassword", "salt")
            {
                Id = 1
            };

            _mockUserRepository.Setup(r => r.GetUserByUsernameOrEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(p => p.VerifyPassword(loginDto.Password, user.PasswordHash, user.Salt))
                .Returns(false);

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<UnauthorizedResult>(result);
            _mockJwtService.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
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

            var user = new User("testuser", "test@example.com", "hashedPassword", "salt")
            {
                Id = 1
            };

            _mockUserRepository.Setup(r => r.GetUserByUsernameOrEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(user);
            _mockPasswordHasher.Setup(p => p.VerifyPassword(loginDto.Password, user.PasswordHash, user.Salt))
                .Returns(true);
            _mockJwtService.Setup(j => j.GenerateToken(user))
                .Returns("mock-jwt-token");

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion
    }
}