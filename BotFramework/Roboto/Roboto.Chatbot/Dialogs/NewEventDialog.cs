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

namespace Roboto.Chatbot.Dialogs
{
    public class NewEventDialog : ComponentDialog
    {
        ILogger<MainDialog> _logger;
        CalendarEvent calendarEvent;
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
                SaveEvent
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
                return await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Please fill form above to add a new event, or tell me the event details please...")
                    }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> AskForEventDate(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AskForEventDate task fired on NewEventDialog");
            
            calendarEvent = (CalendarEvent)stepContext.Options;
            if (string.IsNullOrEmpty(EventStartDate))
            {
                var promptMessage = MessageFactory.Text("When is this event scheduled for?", inputHint: InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = promptMessage
                }, cancellationToken);
            }
            
            return await stepContext.NextAsync(EventStartDate, cancellationToken);
        }

      private async Task<DialogTurnResult> SaveEvent(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("SaveEvent task fired on NewEventDialog");
            calendarEvent = (CalendarEvent)stepContext.Options;

            // Persist using repository (repository handles DbContext and SaveChanges)
            await _repository.AddEventAsync(calendarEvent);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Event saved: {calendarEvent.Title} at {calendarEvent.StartDateTime}"), cancellationToken);
            return await stepContext.EndDialogAsync(calendarEvent, cancellationToken);
        }

    }
}
