using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using Roboto.Dtos;
using Roboto.Sdk;
using System.Linq;

namespace Roboto.Chatbot.Dialogs
{
   public class TodaysScheduleDialog : ComponentDialog
    {
        ILogger<TodaysScheduleDialog> _logger;
        RobotoApiClient _robotoApiClient;
        public TodaysScheduleDialog(ILogger<TodaysScheduleDialog> logger, RobotoApiClient robotoApiClient) : base (nameof(TodaysScheduleDialog))
        {
            _logger = logger;
            _robotoApiClient = robotoApiClient;
            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ShowScheduleAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        }

        private async Task<DialogTurnResult> ShowScheduleAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ShowScheduleAsync task fired on ScheduleDialog");
            CalendarEventsRangeDto dateRange = new CalendarEventsRangeDto
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };
            var events = await _robotoApiClient.CalendarEvents.GetEventsInDateRangeAsync(1, dateRange, cancellationToken);
            if(!events.Any()) 
            {
                await stepContext.Context.SendActivityAsync("You have no events scheduled for today.", cancellationToken: cancellationToken);
            }
            else
            {
                string scheduleMessage = "Your events for today:\n";
                foreach(var calendarEvent in events)
                {
                    scheduleMessage += $"- {calendarEvent.Title} at {calendarEvent.StartDateTime.ToShortTimeString()}\n";
                }
                await stepContext.Context.SendActivityAsync(scheduleMessage, cancellationToken: cancellationToken);
            }
            await stepContext.ReplaceDialogAsync(nameof(MainDialog),RobotoIntents.Welcome);
            return EndOfTurn;
        }
    } 
}