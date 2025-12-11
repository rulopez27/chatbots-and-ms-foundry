using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Roboto.Dtos;
using Roboto.Service.Controllers;
using Roboto.Service.Services;

namespace Roboto.Service.Tests.Controllers
{
    [TestFixture]
    public class UserCalendarControllerTests
    {
        private UserCalendarController _controller;
        private Mock<ICalendarEventService> _mockCalendarEventService;

        [SetUp]
        public void Setup()
        {
            _mockCalendarEventService = new Mock<ICalendarEventService>();
            _controller = new UserCalendarController(_mockCalendarEventService.Object);
        }

        #region GetUserEvents Tests

        [Test]
        public async Task GetUserEvents_ValidUserId_ReturnsOkWithEvents()
        {
            // Arrange
            var userId = 1;
            var events = new List<CalendarEventDto>
            {
                new CalendarEventDto { Id = 1, Title = "Event 1", UserId = userId },
                new CalendarEventDto { Id = 2, Title = "Event 2", UserId = userId }
            };

            _mockCalendarEventService.Setup(s => s.GetEventsByUserIdAsync(userId))
                .ReturnsAsync(events);

            // Act
            var result = await _controller.GetUserEvents(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedEvents = okResult.Value as IEnumerable<CalendarEventDto>;
            Assert.IsNotNull(returnedEvents);
            Assert.AreEqual(2, returnedEvents.Count());
        }

        [Test]
        public async Task GetUserEvents_NoEvents_ReturnsEmptyList()
        {
            // Arrange
            var userId = 1;
            _mockCalendarEventService.Setup(s => s.GetEventsByUserIdAsync(userId))
                .ReturnsAsync(new List<CalendarEventDto>());

            // Act
            var result = await _controller.GetUserEvents(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var returnedEvents = okResult.Value as IEnumerable<CalendarEventDto>;
            Assert.IsNotNull(returnedEvents);
            Assert.AreEqual(0, returnedEvents.Count());
        }

        #endregion

        #region GetEventsInDateRange Tests

        [Test]
        public async Task GetEventsInDateRange_ValidRange_ReturnsEvents()
        {
            // Arrange
            var userId = 1;
            var startDate = new DateTime(2025, 12, 1);
            var endDate = new DateTime(2025, 12, 31);
            var events = new List<CalendarEventDto>
            {
                new CalendarEventDto { Id = 1, Title = "Event 1", UserId = userId, StartDateTime = new DateTime(2025, 12, 5) }
            };

            _mockCalendarEventService.Setup(s => s.GetEventsInDateRangeAsync(userId, startDate, endDate))
                .ReturnsAsync(events);

            // Act
            var result = await _controller.GetEventsInDateRange(userId, startDate, endDate);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            _mockCalendarEventService.Verify(s => s.GetEventsInDateRangeAsync(userId, startDate, endDate), Times.Once);
        }

        #endregion

        #region GetCalendarConflicts Tests

        [Test]
        public async Task GetCalendarConflicts_ValidRange_ReturnsConflicts()
        {
            // Arrange
            var userId = 1;
            var startDate = new DateTime(2025, 12, 8, 10, 0, 0);
            var endDate = new DateTime(2025, 12, 8, 12, 0, 0);
            var conflicts = new List<CalendarEventDto>
            {
                new CalendarEventDto { Id = 1, Title = "Conflicting Event", UserId = userId }
            };

            _mockCalendarEventService.Setup(s => s.GetCalendarConflictsAsync(userId, startDate, endDate))
                .ReturnsAsync(conflicts);

            // Act
            var result = await _controller.GetCalendarConflicts(userId, startDate, endDate);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            _mockCalendarEventService.Verify(s => s.GetCalendarConflictsAsync(userId, startDate, endDate), Times.Once);
        }

        [Test]
        public async Task GetCalendarConflicts_NoConflicts_ReturnsEmptyList()
        {
            // Arrange
            var userId = 1;
            var startDate = new DateTime(2025, 12, 8, 10, 0, 0);
            var endDate = new DateTime(2025, 12, 8, 12, 0, 0);

            _mockCalendarEventService.Setup(s => s.GetCalendarConflictsAsync(userId, startDate, endDate))
                .ReturnsAsync(new List<CalendarEventDto>());

            // Act
            var result = await _controller.GetCalendarConflicts(userId, startDate, endDate);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var returnedConflicts = okResult.Value as IEnumerable<CalendarEventDto>;
            Assert.IsNotNull(returnedConflicts);
            Assert.AreEqual(0, returnedConflicts.Count());
        }

        #endregion
    }
}
