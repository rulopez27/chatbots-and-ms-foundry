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
using Roboto.Service.Services;

namespace Roboto.Service.Tests.Controllers
{
    [TestFixture]
    public class CalendarEventsControllerTests
    {
        private CalendarEventsController _controller;
        private Mock<ICalendarEventService> _mockCalendarEventService;

        [SetUp]
        public void Setup()
        {
            _mockCalendarEventService = new Mock<ICalendarEventService>();

            _controller = new CalendarEventsController(
                _mockCalendarEventService.Object
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

            var returnDto = new CalendarEventDto { Id = 1, Title = "Meeting" };

            _mockCalendarEventService.Setup(s => s.CreateEventAsync(createDto))
                .ReturnsAsync(returnDto);

            // Act
            var result = await _controller.Post(createDto);

            // Assert
            Assert.IsInstanceOf<CreatedResult>(result);
            _mockCalendarEventService.Verify(s => s.CreateEventAsync(createDto), Times.Once);
        }

        #endregion

        #region Get Tests

        [Test]
        public async Task Get_ExistingEvent_ReturnsOk()
        {
            // Arrange
            var eventId = 1;
            var eventDto = new CalendarEventDto { Id = eventId, Title = "Meeting" };

            _mockCalendarEventService.Setup(s => s.GetEventByIdAsync(eventId))
                .ReturnsAsync(eventDto);

            // Act
            var result = await _controller.Get(eventId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            _mockCalendarEventService.Verify(s => s.GetEventByIdAsync(eventId), Times.Once);
        }

        [Test]
        public async Task Get_NonExistingEvent_ReturnsNotFound()
        {
            // Arrange
            var eventId = 999;
            _mockCalendarEventService.Setup(s => s.GetEventByIdAsync(eventId))
                .ReturnsAsync((CalendarEventDto)null);

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

            _mockCalendarEventService.Setup(s => s.UpdateEventAsync(eventDto))
                .ReturnsAsync(eventDto);

            // Act
            var result = await _controller.Put(eventDto.Id, eventDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            _mockCalendarEventService.Verify(s => s.UpdateEventAsync(eventDto), Times.Once);
        }

        [Test]
        public async Task Put_NonExistingEvent_ReturnsNotFound()
        {
            // Arrange
            var eventDto = new CalendarEventDto { Id = 999, Title = "Updated" };
            _mockCalendarEventService.Setup(s => s.UpdateEventAsync(eventDto))
                .ReturnsAsync((CalendarEventDto)null);

            // Act
            var result = await _controller.Put(eventDto.Id, eventDto);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            _mockCalendarEventService.Verify(s => s.UpdateEventAsync(eventDto), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ExistingEvent_ReturnsNoContent()
        {
            // Arrange
            var eventId = 1;

            _mockCalendarEventService.Setup(s => s.DeleteEventAsync(eventId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(eventId);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
            _mockCalendarEventService.Verify(s => s.DeleteEventAsync(eventId), Times.Once);
        }

        [Test]
        public async Task Delete_NonExistingEvent_ReturnsNotFound()
        {
            // Arrange
            var eventId = 999;
            _mockCalendarEventService.Setup(s => s.DeleteEventAsync(eventId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(eventId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            _mockCalendarEventService.Verify(s => s.DeleteEventAsync(eventId), Times.Once);
        }

        #endregion
    }
}