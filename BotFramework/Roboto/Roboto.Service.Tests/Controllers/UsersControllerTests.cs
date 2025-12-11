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
using Roboto.Service.Controllers;
using Roboto.Service.Extensions;

namespace Roboto.Service.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UsersController _controller;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ILinkService> _mockLinkService;
        private Mock<LinkGenerator> _mockLinkGenerator;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLinkService = new Mock<ILinkService>();
            _mockLinkGenerator = new Mock<LinkGenerator>();

            _controller = new UsersController(
                _mockUserRepository.Object,
                _mockMapper.Object,
                _mockHttpContextAccessor.Object,
                _mockLinkService.Object,
                _mockLinkGenerator.Object
            );
        }

        #region GetUserById Tests

        [Test]
        public async Task GetUserById_ExistingUser_ReturnsOkWithUserDto()
        {
            // Arrange
            var userId = 1;
            var user = new User("testuser", "test@example.com", "hash", "salt")
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe"
            };

            var userDto = new UserDto
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map(user, It.IsAny<UserDto>()))
                .Callback<User, UserDto>((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.Username = src.Username;
                    dest.Email = src.Email;
                    dest.FirstName = src.FirstName;
                    dest.LastName = src.LastName;
                });

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<UserDto>(okResult.Value);
            _mockUserRepository.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetUserById_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            var userId = 999;
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            _mockUserRepository.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
        }

        [Test]
        public async Task GetUserById_ValidId_CallsMapperWithCorrectParameters()
        {
            // Arrange
            var userId = 1;
            var user = new User("testuser", "test@example.com", "hash", "salt")
            {
                Id = userId
            };

            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            await _controller.GetUserById(userId);

            // Assert
            _mockMapper.Verify(m => m.Map(user, It.IsAny<UserDto>()), Times.Once);
        }

        #endregion
    }
}