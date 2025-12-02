using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Roboto.Chatbot.Cards;
using System.Threading;
using System.Threading.Tasks;
using Roboto.Models;
using System;

namespace Roboto.Chatbot.Dialogs
{
    public class NewEventDialog : ComponentDialog
    {
        ILogger<MainDialog> _logger;
        CalendarEvent calendarEvent;
        public string EventStartDate { get; set; }
        public string EventStartTime { get; set; }

        public NewEventDialog(ILogger<MainDialog> logger) : base(nameof(NewEventDialog))
        {
            _logger = logger;
            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ShowNewEventCard,
                AskForEventDate,
                AskForStartTime,
                AskForEventName,
                AskForEventDuration
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

        private async Task<DialogTurnResult> AskForStartTime(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AskForStartTime task fired on NewEventDialog");
            calendarEvent = (CalendarEvent)stepContext.Options;
            
            //Get result from previous step
            EventStartDate = (string)stepContext.Result;
            if (string.IsNullOrEmpty(EventStartTime))
            {
                var promptMessage = MessageFactory.Text("At what time does the event start?", inputHint: InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = promptMessage
                }, cancellationToken);
            }

            return await stepContext.NextAsync(EventStartTime, cancellationToken);

        }
        private async Task<DialogTurnResult> AskForEventName(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AskForEventName task fired on NewEventDialog");
            calendarEvent = (CalendarEvent)stepContext.Options;

            //Get result from previous step
            EventStartTime = (string)stepContext.Result;

            calendarEvent.StartDateTime = DateTime.Parse($"{EventStartDate} {EventStartTime}");
            if(string.IsNullOrEmpty(calendarEvent.Title))
            {
                var promptMessage = MessageFactory.Text("What is the name of the event?", inputHint: InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
                {
                    Prompt = promptMessage
                }, cancellationToken);
            }

            return await stepContext.NextAsync(calendarEvent.Title, cancellationToken);

        }

        private async Task<DialogTurnResult> AskForEventDuration(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AskForEventDuration task fired on NewEventDialog");
            calendarEvent = (CalendarEvent)stepContext.Options;

            //Get result from previous step
            calendarEvent.Title = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text("How long will the event last (in hours)?", inputHint: InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = promptMessage
            }, cancellationToken);
        }

    }
}
