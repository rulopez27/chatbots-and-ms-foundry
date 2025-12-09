using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Roboto.Models;

namespace Roboto.Repository.Tests
{
    [TestFixture]
    public class CalendarEventRepositoryTests
    {
        private RobotoCalendarSchedulerDbContext _context;
        private CalendarEventRepository _calendarEventRepository;
        private User _testUser;

        [SetUp]
        public void Setup()
        {
            // Create in-memory database options
            var options = new DbContextOptionsBuilder<RobotoCalendarSchedulerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
                .Options;

            _context = new RobotoCalendarSchedulerDbContext(options);
            _calendarEventRepository = new CalendarEventRepository(_context);

            // Create a test user for calendar events
            _testUser = new User("testuser", "test@example.com", "hash", "salt");
            _context.Users.Add(_testUser);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _calendarEventRepository.Dispose();
        }

        #region GetEventByIdAsync Tests

        [Test]
        public async Task GetEventByIdAsync_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            var calendarEvent = new CalendarEvent(_testUser.Id, "Meeting", DateTime.Now, 1.0, false, false, "Team meeting");
            _context.CalendarEvents.Add(calendarEvent);
            await _context.SaveChangesAsync();

            // Act
            var result = await _calendarEventRepository.GetEventByIdAsync(calendarEvent.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(calendarEvent.Id, result.Id);
            Assert.AreEqual("Meeting", result.Title);
        }

        [Test]
        public void GetEventByIdAsync_ShouldThrowException_WhenEventDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => 
                await _calendarEventRepository.GetEventByIdAsync(9999));
        }

        #endregion

        #region AddEventAsync Tests

        [Test]
        public async Task AddEventAsync_ShouldAddEvent_WhenEventIsValid()
        {
            // Arrange
            var calendarEvent = new CalendarEvent(_testUser.Id, "New Event", DateTime.Now, 2.0, false, false, "Event details");

            // Act
            await _calendarEventRepository.AddEventAsync(calendarEvent);

            // Assert
            var savedEvent = await _context.CalendarEvents.FirstOrDefaultAsync(e => e.Title == "New Event");
            Assert.IsNotNull(savedEvent);
            Assert.AreEqual("New Event", savedEvent.Title);
            Assert.AreEqual(_testUser.Id, savedEvent.UserId);
            Assert.AreEqual(2.0, savedEvent.Duration);
        }

        [Test]
        public async Task AddEventAsync_ShouldCalculateEndDateTime_WhenEventIsAdded()
        {
            // Arrange
            var startTime = new DateTime(2025, 12, 8, 10, 0, 0);
            var calendarEvent = new CalendarEvent(_testUser.Id, "Event", startTime, 3.0, false, false, "Details");

            // Act
            await _calendarEventRepository.AddEventAsync(calendarEvent);

            // Assert
            var savedEvent = await _context.CalendarEvents.FirstOrDefaultAsync(e => e.Title == "Event");
            Assert.IsNotNull(savedEvent);
            Assert.AreEqual(startTime.AddHours(3), savedEvent.EndDateTime);
        }

        [Test]
        public async Task AddEventAsync_ShouldGenerateId_WhenEventIsAdded()
        {
            // Arrange
            var calendarEvent = new CalendarEvent(_testUser.Id, "Event", DateTime.Now, 1.0, false, false, "Details");

            // Act
            await _calendarEventRepository.AddEventAsync(calendarEvent);

            // Assert
            Assert.Greater(calendarEvent.Id, 0);
        }

        #endregion

        #region GetCalendarConflictsAsync Tests

        [Test]
        public async Task GetCalendarConflictsAsync_ShouldReturnConflictingEvents_WhenEventsOverlap()
        {
            // Arrange
            var startTime1 = new DateTime(2025, 12, 8, 10, 0, 0);
            var event1 = new CalendarEvent(_testUser.Id, "Event 1", startTime1, 2.0, false, false, "Details");
            
            var startTime2 = new DateTime(2025, 12, 8, 11, 0, 0);
            var event2 = new CalendarEvent(_testUser.Id, "Event 2", startTime2, 2.0, false, false, "Details");

            _context.CalendarEvents.AddRange(event1, event2);
            await _context.SaveChangesAsync();

            // Act - check for conflicts with a new event from 11:30 to 12:30
            var checkStart = new DateTime(2025, 12, 8, 11, 30, 0);
            var checkEnd = new DateTime(2025, 12, 8, 12, 30, 0);
            var conflicts = await _calendarEventRepository.GetCalendarConflictsAsync(_testUser.Id, checkStart, checkEnd);

            // Assert
            Assert.AreEqual(2, conflicts.Count); // Both events overlap with the check window
        }

        [Test]
        public async Task GetCalendarConflictsAsync_ShouldReturnEmpty_WhenNoConflicts()
        {
            // Arrange
            var startTime = new DateTime(2025, 12, 8, 10, 0, 0);
            var event1 = new CalendarEvent(_testUser.Id, "Event 1", startTime, 1.0, false, false, "Details");
            
            _context.CalendarEvents.Add(event1);
            await _context.SaveChangesAsync();

            // Act - check for conflicts with a non-overlapping time
            var checkStart = new DateTime(2025, 12, 8, 14, 0, 0);
            var checkEnd = new DateTime(2025, 12, 8, 15, 0, 0);
            var conflicts = await _calendarEventRepository.GetCalendarConflictsAsync(_testUser.Id, checkStart, checkEnd);

            // Assert
            Assert.AreEqual(0, conflicts.Count);
        }

        [Test]
        public async Task GetCalendarConflictsAsync_ShouldOnlyReturnEventsForSpecificUser()
        {
            // Arrange
            var otherUser = new User("otheruser", "other@example.com", "hash", "salt");
            _context.Users.Add(otherUser);
            await _context.SaveChangesAsync();

            var startTime = new DateTime(2025, 12, 8, 10, 0, 0);
            var event1 = new CalendarEvent(_testUser.Id, "User 1 Event", startTime, 2.0, false, false, "Details");
            var event2 = new CalendarEvent(otherUser.Id, "User 2 Event", startTime, 2.0, false, false, "Details");
            
            _context.CalendarEvents.AddRange(event1, event2);
            await _context.SaveChangesAsync();

            // Act
            var checkStart = new DateTime(2025, 12, 8, 9, 0, 0);
            var checkEnd = new DateTime(2025, 12, 8, 13, 0, 0);
            var conflicts = await _calendarEventRepository.GetCalendarConflictsAsync(_testUser.Id, checkStart, checkEnd);

            // Assert
            Assert.AreEqual(1, conflicts.Count);
            Assert.AreEqual("User 1 Event", conflicts[0].Title);
        }

        #endregion

        #region GetEventsByDateAsync Tests

        [Test]
        public async Task GetEventsByDateAsync_ShouldReturnEventsOnSpecificDate()
        {
            // Arrange
            var targetDate = new DateTime(2025, 12, 8);
            var event1 = new CalendarEvent(_testUser.Id, "Morning Event", targetDate.AddHours(9), 1.0, false, false, "Details");
            var event2 = new CalendarEvent(_testUser.Id, "Afternoon Event", targetDate.AddHours(14), 2.0, false, false, "Details");
            var event3 = new CalendarEvent(_testUser.Id, "Other Day Event", targetDate.AddDays(1).AddHours(10), 1.0, false, false, "Details");
            
            _context.CalendarEvents.AddRange(event1, event2, event3);
            await _context.SaveChangesAsync();

            // Act
            var events = await _calendarEventRepository.GetEventsByDateAsync(_testUser.Id, targetDate);

            // Assert
            Assert.AreEqual(2, events.Count);
            Assert.IsTrue(events.All(e => e.StartDateTime.Date == targetDate.Date));
        }

        [Test]
        public async Task GetEventsByDateAsync_ShouldReturnEmpty_WhenNoEventsOnDate()
        {
            // Arrange
            var eventDate = new DateTime(2025, 12, 8);
            var checkDate = new DateTime(2025, 12, 10);
            var event1 = new CalendarEvent(_testUser.Id, "Event", eventDate.AddHours(9), 1.0, false, false, "Details");
            
            _context.CalendarEvents.Add(event1);
            await _context.SaveChangesAsync();

            // Act
            var events = await _calendarEventRepository.GetEventsByDateAsync(_testUser.Id, checkDate);

            // Assert
            Assert.AreEqual(0, events.Count);
        }

        #endregion

        #region UpdateEventAsync Tests

        [Test]
        public async Task UpdateEventAsync_ShouldUpdateEvent_WhenEventExists()
        {
            // Arrange
            var calendarEvent = new CalendarEvent(_testUser.Id, "Original Title", DateTime.Now, 1.0, false, false, "Original Details");
            _context.CalendarEvents.Add(calendarEvent);
            await _context.SaveChangesAsync();

            // Act
            calendarEvent.Title = "Updated Title";
            calendarEvent.Duration = 2.0;
            await _calendarEventRepository.UpdateEventAsync(calendarEvent);

            // Assert
            var updatedEvent = await _context.CalendarEvents.FindAsync(calendarEvent.Id);
            Assert.IsNotNull(updatedEvent);
            Assert.AreEqual("Updated Title", updatedEvent.Title);
            Assert.AreEqual(2.0, updatedEvent.Duration);
        }

        [Test]
        public async Task UpdateEventAsync_ShouldRecalculateEndDateTime_WhenDurationChanges()
        {
            // Arrange
            var startTime = new DateTime(2025, 12, 8, 10, 0, 0);
            var calendarEvent = new CalendarEvent(_testUser.Id, "Event", startTime, 1.0, false, false, "Details");
            _context.CalendarEvents.Add(calendarEvent);
            await _context.SaveChangesAsync();

            var originalEndTime = calendarEvent.EndDateTime;

            // Act
            calendarEvent.Duration = 3.0;
            await _calendarEventRepository.UpdateEventAsync(calendarEvent);

            // Assert
            var updatedEvent = await _context.CalendarEvents.FindAsync(calendarEvent.Id);
            Assert.IsNotNull(updatedEvent);
            Assert.AreNotEqual(originalEndTime, updatedEvent.EndDateTime);
            Assert.AreEqual(startTime.AddHours(3), updatedEvent.EndDateTime);
        }

        #endregion

        #region DeleteEventAsync Tests

        [Test]
        public async Task DeleteEventAsync_ShouldDeleteEvent_WhenEventExists()
        {
            // Arrange
            var calendarEvent = new CalendarEvent(_testUser.Id, "Event to Delete", DateTime.Now, 1.0, false, false, "Details");
            _context.CalendarEvents.Add(calendarEvent);
            await _context.SaveChangesAsync();
            var eventId = calendarEvent.Id;

            // Act
            await _calendarEventRepository.DeleteEventAsync(eventId);

            // Assert
            var deletedEvent = await _context.CalendarEvents.FindAsync(eventId);
            Assert.IsNull(deletedEvent);
        }

        [Test]
        public void DeleteEventAsync_ShouldThrowException_WhenEventDoesNotExist()
        {
            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => 
                await _calendarEventRepository.DeleteEventAsync(9999));
        }

        #endregion

        #region GetEventsInDateRangeAsync Tests

        [Test]
        public async Task GetEventsInDateRangeAsync_ShouldReturnEventsInRange()
        {
            // Arrange
            var startDate = new DateTime(2025, 12, 1);
            var endDate = new DateTime(2025, 12, 31);
            
            var event1 = new CalendarEvent(_testUser.Id, "Dec Event 1", new DateTime(2025, 12, 5, 10, 0, 0), 1.0, false, false, "Details");
            var event2 = new CalendarEvent(_testUser.Id, "Dec Event 2", new DateTime(2025, 12, 15, 14, 0, 0), 2.0, false, false, "Details");
            var event3 = new CalendarEvent(_testUser.Id, "Nov Event", new DateTime(2025, 11, 25, 10, 0, 0), 1.0, false, false, "Details");
            
            _context.CalendarEvents.AddRange(event1, event2, event3);
            await _context.SaveChangesAsync();

            // Act
            var events = await _calendarEventRepository.GetEventsInDateRangeAsync(_testUser.Id, startDate, endDate);

            // Assert
            Assert.AreEqual(2, events.Count);
            Assert.IsTrue(events.All(e => e.StartDateTime >= startDate && e.EndDateTime <= endDate));
        }

        [Test]
        public async Task GetEventsInDateRangeAsync_ShouldReturnEmpty_WhenNoEventsInRange()
        {
            // Arrange
            var startDate = new DateTime(2025, 12, 1);
            var endDate = new DateTime(2025, 12, 31);
            
            var event1 = new CalendarEvent(_testUser.Id, "Jan Event", new DateTime(2026, 1, 5, 10, 0, 0), 1.0, false, false, "Details");
            
            _context.CalendarEvents.Add(event1);
            await _context.SaveChangesAsync();

            // Act
            var events = await _calendarEventRepository.GetEventsInDateRangeAsync(_testUser.Id, startDate, endDate);

            // Assert
            Assert.AreEqual(0, events.Count);
        }

        #endregion

        #region GetEventsByUserIdAsync Tests

        [Test]
        public async Task GetEventsByUserIdAsync_ShouldReturnAllEventsForUser()
        {
            // Arrange
            var event1 = new CalendarEvent(_testUser.Id, "Event 1", DateTime.Now, 1.0, false, false, "Details");
            var event2 = new CalendarEvent(_testUser.Id, "Event 2", DateTime.Now.AddDays(1), 2.0, false, false, "Details");
            var event3 = new CalendarEvent(_testUser.Id, "Event 3", DateTime.Now.AddDays(2), 1.5, false, false, "Details");
            
            _context.CalendarEvents.AddRange(event1, event2, event3);
            await _context.SaveChangesAsync();

            // Act
            var events = await _calendarEventRepository.GetEventsByUserIdAsync(_testUser.Id);

            // Assert
            Assert.AreEqual(3, events.Count);
            Assert.IsTrue(events.All(e => e.UserId == _testUser.Id));
        }

        [Test]
        public async Task GetEventsByUserIdAsync_ShouldReturnEmpty_WhenUserHasNoEvents()
        {
            // Arrange
            var otherUser = new User("otheruser", "other@example.com", "hash", "salt");
            _context.Users.Add(otherUser);
            await _context.SaveChangesAsync();

            // Act
            var events = await _calendarEventRepository.GetEventsByUserIdAsync(otherUser.Id);

            // Assert
            Assert.AreEqual(0, events.Count);
        }

        [Test]
        public async Task GetEventsByUserIdAsync_ShouldOnlyReturnEventsForSpecificUser()
        {
            // Arrange
            var otherUser = new User("otheruser", "other@example.com", "hash", "salt");
            _context.Users.Add(otherUser);
            await _context.SaveChangesAsync();

            var event1 = new CalendarEvent(_testUser.Id, "User 1 Event", DateTime.Now, 1.0, false, false, "Details");
            var event2 = new CalendarEvent(otherUser.Id, "User 2 Event", DateTime.Now, 1.0, false, false, "Details");
            
            _context.CalendarEvents.AddRange(event1, event2);
            await _context.SaveChangesAsync();

            // Act
            var events = await _calendarEventRepository.GetEventsByUserIdAsync(_testUser.Id);

            // Assert
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("User 1 Event", events[0].Title);
        }

        #endregion
    }
}