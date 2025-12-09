using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Roboto.Models;

namespace Roboto.Repository.Tests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private RobotoCalendarSchedulerDbContext _context;
        private UserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            // Create in-memory database options
            var options = new DbContextOptionsBuilder<RobotoCalendarSchedulerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
                .Options;

            _context = new RobotoCalendarSchedulerDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region UsernameOrEmailExistsAsync Tests

        [Test]
        public async Task UsernameOrEmailExistsAsync_ShouldReturnTrue_WhenUsernameExists()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.UsernameOrEmailExistsAsync("testuser", "other@example.com");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task UsernameOrEmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.UsernameOrEmailExistsAsync("otheruser", "test@example.com");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task UsernameOrEmailExistsAsync_ShouldReturnTrue_WhenBothExist()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.UsernameOrEmailExistsAsync("testuser", "test@example.com");

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task UsernameOrEmailExistsAsync_ShouldReturnFalse_WhenNeitherExists()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.UsernameOrEmailExistsAsync("newuser", "new@example.com");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task UsernameOrEmailExistsAsync_ShouldReturnFalse_WhenDatabaseIsEmpty()
        {
            // Act
            var result = await _userRepository.UsernameOrEmailExistsAsync("testuser", "test@example.com");

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region AddUserAsync Tests

        [Test]
        public async Task AddUserAsync_ShouldAddUser_WhenUserIsValid()
        {
            // Arrange
            var user = new User("newuser", "newuser@example.com", "hashedpassword", "salt")
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            await _userRepository.AddUserAsync(user);

            // Assert
            var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
            Assert.IsNotNull(savedUser);
            Assert.AreEqual("newuser", savedUser.Username);
            Assert.AreEqual("newuser@example.com", savedUser.Email);
            Assert.AreEqual("John", savedUser.FirstName);
            Assert.AreEqual("Doe", savedUser.LastName);
        }

        [Test]
        public async Task AddUserAsync_ShouldGenerateId_WhenUserIsAdded()
        {
            // Arrange
            var user = new User("newuser", "newuser@example.com", "hashedpassword", "salt");

            // Act
            await _userRepository.AddUserAsync(user);

            // Assert
            Assert.Greater(user.Id, 0);
        }

        [Test]
        public async Task AddUserAsync_ShouldAddMultipleUsers_WhenCalledMultipleTimes()
        {
            // Arrange
            var user1 = new User("user1", "user1@example.com", "hash1", "salt1");
            var user2 = new User("user2", "user2@example.com", "hash2", "salt2");

            // Act
            await _userRepository.AddUserAsync(user1);
            await _userRepository.AddUserAsync(user2);

            // Assert
            var userCount = await _context.Users.CountAsync();
            Assert.AreEqual(2, userCount);
        }

        #endregion

        #region GetUserByUsernameOrEmailAsync Tests

        [Test]
        public async Task GetUserByUsernameOrEmailAsync_ShouldReturnUser_WhenUsernameMatches()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByUsernameOrEmailAsync("testuser");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("testuser", result.Username);
            Assert.AreEqual("test@example.com", result.Email);
        }

        [Test]
        public async Task GetUserByUsernameOrEmailAsync_ShouldReturnUser_WhenEmailMatches()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByUsernameOrEmailAsync("test@example.com");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("testuser", result.Username);
            Assert.AreEqual("test@example.com", result.Email);
        }

        [Test]
        public async Task GetUserByUsernameOrEmailAsync_ShouldReturnNull_WhenNoMatch()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByUsernameOrEmailAsync("nonexistent");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetUserByUsernameOrEmailAsync_ShouldReturnNull_WhenDatabaseIsEmpty()
        {
            // Act
            var result = await _userRepository.GetUserByUsernameOrEmailAsync("testuser");

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetUserByIdAsync Tests

        [Test]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenIdExists()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdAsync(user.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual("testuser", result.Username);
        }

        [Test]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdAsync(9999);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenDatabaseIsEmpty()
        {
            // Act
            var result = await _userRepository.GetUserByIdAsync(1);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetUserByIdWithEventsAsync Tests

        [Test]
        public async Task GetUserByIdWithEventsAsync_ShouldReturnUser_WhenIdExists()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdWithEventsAsync(user.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual("testuser", result.Username);
        }

        [Test]
        public async Task GetUserByIdWithEventsAsync_ShouldIncludeEvents_WhenUserHasEvents()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var event1 = new CalendarEvent
            {
                Title = "Event 1",
                Details = "Description 1",
                StartDateTime = DateTime.Now,
                Duration = 1.0,
                EndDateTime = DateTime.Now.AddHours(1),
                UserId = user.Id
            };
            var event2 = new CalendarEvent
            {
                Title = "Event 2",
                Details = "Description 2",
                StartDateTime = DateTime.Now.AddDays(1),
                Duration = 1.0,
                EndDateTime = DateTime.Now.AddDays(1).AddHours(1),
                UserId = user.Id
            };

            _context.CalendarEvents.Add(event1);
            _context.CalendarEvents.Add(event2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdWithEventsAsync(user.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.CalendarEvents);
            Assert.AreEqual(2, result.CalendarEvents.Count);
        }

        [Test]
        public async Task GetUserByIdWithEventsAsync_ShouldReturnUserWithEmptyEvents_WhenUserHasNoEvents()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdWithEventsAsync(user.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.CalendarEvents);
            Assert.AreEqual(0, result.CalendarEvents.Count);
        }

        [Test]
        public async Task GetUserByIdWithEventsAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", "hashedpassword", "salt");
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByIdWithEventsAsync(9999);

            // Assert
            Assert.IsNull(result);
        }

        #endregion
    }
}
