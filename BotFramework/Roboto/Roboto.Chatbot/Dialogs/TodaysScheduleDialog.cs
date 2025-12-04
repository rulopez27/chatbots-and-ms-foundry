using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using Roboto.Repository;

namespace Roboto.Chatbot.Dialogs
{
   public class TodaysScheduleDialog : ComponentDialog
    {
        ILogger<TodaysScheduleDialog> _logger;
        IRobotoRepository _repository;
        public TodaysScheduleDialog(ILogger<TodaysScheduleDialog> logger, IRobotoRepository repository) : base (nameof(TodaysScheduleDialog))
        {
            _logger = logger;
            _repository = repository;
            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ShowScheduleAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
        }

        private async Task<DialogTurnResult> ShowScheduleAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ShowScheduleAsync task fired on ScheduleDialog");
            var events = await _repository.GetEventsByDateAsync(DateTime.Now);
            if(events.Count == 0)
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