using System;
using System.Collections.Generic;
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

namespace Roboto.Service.Tests.Controllers
{
    [TestFixture]
    public class CalendarEventsControllerTests
    {
        private CalendarEventsController _controller;
        private Mock<ICalendarEventRepository> _mockRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ILinkService> _mockLinkService;
        private Mock<LinkGenerator> _mockLinkGenerator;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ICalendarEventRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLinkService = new Mock<ILinkService>();
            _mockLinkGenerator = new Mock<LinkGenerator>();

            _controller = new CalendarEventsController(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockHttpContextAccessor.Object,
                _mockLinkService.Object,
                _mockLinkGenerator.Object
            );
        }

        #region Post Tests

        [Test]
        public async Task Post_ValidEvent_ReturnsCreated()
        {
            // Arrange
            var createDto = new CalendarEventCreateDto
            {
                UserId = 1,
                Title = "Meeting",
                StartDateTime = DateTime.Now,
                Duration = 1.0,
                IsAllDay = false,
                BlockCalendar = true,
                Details = "Team meeting"
            };

            var calendarEvent = new CalendarEvent(1, "Meeting", DateTime.Now, 1.0, false, true, "Team meeting");
            var returnDto = new CalendarEventDto { Id = 1, Title = "Meeting" };

            _mockMapper.Setup(m => m.Map(createDto, It.IsAny<CalendarEvent>()))
                .Callback<CalendarEventCreateDto, CalendarEvent>((src, dest) =>
                {
                    dest.UserId = src.UserId;
                    dest.Title = src.Title;
                    dest.StartDateTime = src.StartDateTime;
                    dest.Duration = src.Duration;
                });
            _mockMapper.Setup(m => m.Map<CalendarEventDto>(It.IsAny<CalendarEvent>())).Returns(returnDto);
            _mockRepository.Setup(r => r.AddEventAsync(It.IsAny<CalendarEvent>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Post(createDto);

            // Assert
            Assert.IsInstanceOf<CreatedResult>(result);
            _mockRepository.Verify(r => r.AddEventAsync(It.IsAny<CalendarEvent>()), Times.Once);
        }

        [Test]
        public async Task Post_EmptyTitle_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CalendarEventCreateDto
            {
                UserId = 1,
                Title = "",
                StartDateTime = DateTime.Now,
                Duration = 1.0
            };

            // Act
            var result = await _controller.Post(createDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            _mockRepository.Verify(r => r.AddEventAsync(It.IsAny<CalendarEvent>()), Times.Never);
        }

        [Test]
        public async Task Post_ZeroDuration_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CalendarEventCreateDto
            {
                UserId = 1,
                Title = "Meeting",
                StartDateTime = DateTime.Now,
                Duration = 0
            };

            // Act
            var result = await _controller.Post(createDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        #endregion

        #region Get Tests

        [Test]
        public async Task Get_ExistingEvent_ReturnsOk()
        {
            // Arrange
            var eventId = 1;
            var calendarEvent = new CalendarEvent(1, "Meeting", DateTime.Now, 1.0, false, true, "Details") { Id = eventId };
            var eventDto = new CalendarEventDto { Id = eventId, Title = "Meeting" };

            _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync(calendarEvent);
            _mockMapper.Setup(m => m.Map(calendarEvent, It.IsAny<CalendarEventDto>()))
                .Callback<CalendarEvent, CalendarEventDto>((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.Title = src.Title;
                });

            // Act
            var result = await _controller.Get(eventId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            _mockRepository.Verify(r => r.GetEventByIdAsync(eventId), Times.Once);
        }

        [Test]
        public async Task Get_NonExistingEvent_ReturnsNotFound()
        {
            // Arrange
            var eventId = 999;
            _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync((CalendarEvent)null);

            // Act
            var result = await _controller.Get(eventId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        #endregion

        #region Put Tests

        [Test]
        public async Task Put_ExistingEvent_ReturnsOk()
        {
            // Arrange
            var eventDto = new CalendarEventDto
            {
                Id = 1,
                Title = "Updated Meeting",
                StartDateTime = DateTime.Now,
                Duration = 2.0
            };

            var existingEvent = new CalendarEvent(1, "Meeting", DateTime.Now, 1.0, false, true, "Details") { Id = 1 };

            _mockRepository.Setup(r => r.GetEventByIdAsync(eventDto.Id)).ReturnsAsync(existingEvent);
            _mockMapper.Setup(m => m.Map(eventDto, existingEvent)).Returns(existingEvent);
            _mockMapper.Setup(m => m.Map<CalendarEventDto>(existingEvent)).Returns(eventDto);
            _mockRepository.Setup(r => r.UpdateEventAsync(existingEvent)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Put(eventDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            _mockRepository.Verify(r => r.UpdateEventAsync(It.IsAny<CalendarEvent>()), Times.Once);
        }

        [Test]
        public async Task Put_NonExistingEvent_ReturnsNotFound()
        {
            // Arrange
            var eventDto = new CalendarEventDto { Id = 999, Title = "Updated" };
            _mockRepository.Setup(r => r.GetEventByIdAsync(eventDto.Id)).ReturnsAsync((CalendarEvent)null);

            // Act
            var result = await _controller.Put(eventDto);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            _mockRepository.Verify(r => r.UpdateEventAsync(It.IsAny<CalendarEvent>()), Times.Never);
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ExistingEvent_ReturnsNoContent()
        {
            // Arrange
            var eventId = 1;
            var existingEvent = new CalendarEvent(1, "Meeting", DateTime.Now, 1.0, false, true, "Details") { Id = eventId };

            _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync(existingEvent);
            _mockRepository.Setup(r => r.DeleteEventAsync(eventId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(eventId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            _mockRepository.Verify(r => r.DeleteEventAsync(eventId), Times.Once);
        }

        [Test]
        public async Task Delete_NonExistingEvent_ReturnsNotFound()
        {
            // Arrange
            var eventId = 999;
            _mockRepository.Setup(r => r.GetEventByIdAsync(eventId)).ReturnsAsync((CalendarEvent)null);

            // Act
            var result = await _controller.Delete(eventId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            _mockRepository.Verify(r => r.DeleteEventAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion

        #region GetEventsForUser Tests

        [Test]
        public async Task GetEventsForUser_ValidUserId_ReturnsOkWithEvents()
        {
            // Arrange
            var userId = 1;
            var events = new List<CalendarEvent>
            {
                new CalendarEvent(userId, "Event 1", DateTime.Now, 1.0, false, true, "Details") { Id = 1 },
                new CalendarEvent(userId, "Event 2", DateTime.Now.AddDays(1), 2.0, false, true, "Details") { Id = 2 }
            };

            _mockRepository.Setup(r => r.GetEventsByUserIdAsync(userId)).ReturnsAsync(events);
            _mockMapper.Setup(m => m.Map(It.IsAny<CalendarEvent>(), It.IsAny<CalendarEventDto>()))
                .Callback<CalendarEvent, CalendarEventDto>((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.Title = src.Title;
                });

            // Act
            var result = await _controller.GetEventsForUser(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedEvents = okResult.Value as List<CalendarEventDto>;
            Assert.IsNotNull(returnedEvents);
            Assert.AreEqual(2, returnedEvents.Count);
        }

        [Test]
        public async Task GetEventsForUser_NoEvents_ReturnsEmptyList()
        {
            // Arrange
            var userId = 1;
            _mockRepository.Setup(r => r.GetEventsByUserIdAsync(userId)).ReturnsAsync(new List<CalendarEvent>());

            // Act
            var result = await _controller.GetEventsForUser(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var returnedEvents = okResult.Value as List<CalendarEventDto>;
            Assert.IsNotNull(returnedEvents);
            Assert.AreEqual(0, returnedEvents.Count);
        }

        #endregion

        #region GetEventsInDateRangeForUser Tests

        [Test]
        public async Task GetEventsInDateRangeForUser_ValidRange_ReturnsEvents()
        {
            // Arrange
            var userId = 1;
            var dateRange = new CalendarEventsRangeDto
            {
                StartDate = new DateTime(2025, 12, 1),
                EndDate = new DateTime(2025, 12, 31)
            };

            var events = new List<CalendarEvent>
            {
                new CalendarEvent(userId, "Event 1", new DateTime(2025, 12, 5, 10, 0, 0), 1.0, false, true, "Details") { Id = 1 }
            };

            _mockRepository.Setup(r => r.GetEventsInDateRangeAsync(userId, dateRange.StartDate, dateRange.EndDate))
                .ReturnsAsync(events);
            _mockMapper.Setup(m => m.Map(It.IsAny<CalendarEvent>(), It.IsAny<CalendarEventDto>()))
                .Callback<CalendarEvent, CalendarEventDto>((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.Title = src.Title;
                });

            // Act
            var result = await _controller.GetEventsInDateRangeForUser(userId, dateRange);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            _mockRepository.Verify(r => r.GetEventsInDateRangeAsync(userId, dateRange.StartDate, dateRange.EndDate), Times.Once);
        }

        #endregion

        #region GetCalendarConflicts Tests

        [Test]
        public async Task GetCalendarConflicts_ValidRange_ReturnsConflicts()
        {
            // Arrange
            var userId = 1;
            var dateRange = new CalendarEventsRangeDto
            {
                StartDate = new DateTime(2025, 12, 8, 10, 0, 0),
                EndDate = new DateTime(2025, 12, 8, 12, 0, 0)
            };

            var conflicts = new List<CalendarEvent>
            {
                new CalendarEvent(userId, "Conflicting Event", new DateTime(2025, 12, 8, 11, 0, 0), 2.0, false, true, "Details") { Id = 1 }
            };

            _mockRepository.Setup(r => r.GetCalendarConflictsAsync(userId, dateRange.StartDate, dateRange.EndDate))
                .ReturnsAsync(conflicts);
            _mockMapper.Setup(m => m.Map(It.IsAny<CalendarEvent>(), It.IsAny<CalendarEventDto>()))
                .Callback<CalendarEvent, CalendarEventDto>((src, dest) =>
                {
                    dest.Id = src.Id;
                    dest.Title = src.Title;
                });

            // Act
            var result = await _controller.GetCalendarConflicts(userId, dateRange);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            _mockRepository.Verify(r => r.GetCalendarConflictsAsync(userId, dateRange.StartDate, dateRange.EndDate), Times.Once);
        }

        [Test]
        public async Task GetCalendarConflicts_NoConflicts_ReturnsEmptyList()
        {
            // Arrange
            var userId = 1;
            var dateRange = new CalendarEventsRangeDto
            {
                StartDate = new DateTime(2025, 12, 8, 10, 0, 0),
                EndDate = new DateTime(2025, 12, 8, 12, 0, 0)
            };

            _mockRepository.Setup(r => r.GetCalendarConflictsAsync(userId, dateRange.StartDate, dateRange.EndDate))
                .ReturnsAsync(new List<CalendarEvent>());

            // Act
            var result = await _controller.GetCalendarConflicts(userId, dateRange);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var returnedConflicts = okResult.Value as List<CalendarEventDto>;
            Assert.IsNotNull(returnedConflicts);
            Assert.AreEqual(0, returnedConflicts.Count);
        }

        #endregion
    }
}