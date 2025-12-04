using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Roboto.Chatbot.Cards;
using System.Threading;
using System.Threading.Tasks;
using Roboto.Models;
using System;
using Roboto.Repository;
using Newtonsoft.Json.Linq;

namespace Roboto.Chatbot.Dialogs
{
    public class NewEventDialog : ComponentDialog
    {
        ILogger<MainDialog> _logger;
        IRobotoRepository _repository;
        
        public string EventStartDate { get; set; }
        public string EventStartTime { get; set; }

        public NewEventDialog(ILogger<MainDialog> logger, IRobotoRepository repository) : base(nameof(NewEventDialog))
        {
            _logger = logger;
            _repository = repository;
            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ShowNewEventCard,
                HandleSubmitAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallStep), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
        }

        private async Task<DialogTurnResult> ShowNewEventCard(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ShowNewEventCard task fired on NewEventDialog");
            Attachment newEventCard;
            using (AdaptiveCardHelper cardHelper = new AdaptiveCardHelper("newEventCard.json"))
            {
                newEventCard = cardHelper.GetAdaptiveCard();
                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(newEventCard));
                return EndOfTurn;
            }
        }

      private async Task<DialogTurnResult> HandleSubmitAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("SaveEvent task fired on NewEventDialog");
            if(stepContext.Context.Activity.Value != null)
            {
                var payload = stepContext.Context.Activity.Value as JObject;
                if(payload.Value<string>("submitType") == "newEvent" && payload["date"] != null)
                {
                    //Extract event details from payload
                    string title = payload.Value<string>("title") ?? "No Title";
                    string date = payload.Value<string>("date") ?? DateTime.Now.ToString("yyyy-MM-dd");
                    string time = payload.Value<string>("time") ?? "00:00";
                    double duration = double.TryParse(payload.Value<string>("duration"), out double dur) ? dur : 1.0;
                    string details = payload.Value<string>("notes") ?? string.Empty;
                    bool isAllDay = payload.Value<bool?>("allDay") ?? false;
                    bool blockCalendar = payload.Value<bool?>("blocksCalendar") ?? false;

                    //Create a CalendarEvent object (assuming such a class exists)
                    CalendarEvent newEvent = new CalendarEvent(title, DateTime.Parse(date), time, duration, isAllDay, blockCalendar, details);
                    try
                    {
                        // Persist using repository (repository handles DbContext and SaveChanges)
                        await _repository.AddEventAsync(newEvent);
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Event saved: {newEvent.Title} at {newEvent.StartDateTime}"), cancellationToken);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError($"Error creating CalendarEvent: {ex.Message}");
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text("There was an error creating the event. Please try again."), cancellationToken);
                        return await stepContext.EndDialogAsync(null, cancellationToken);
                    }
                    return await stepContext.EndDialogAsync(RobotoIntents.Welcome, cancellationToken);
                }
            }
            // No payload or unexpected payload — end dialog (or optionally continue dialog to ask for missing info)
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("I didn't receive the form submission."), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

    }
}
