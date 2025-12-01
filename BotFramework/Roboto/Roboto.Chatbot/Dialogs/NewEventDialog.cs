using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Roboto.Chatbot.Cards;
using System.Threading;
using System.Threading.Tasks;
using Roboto.Models;

namespace Roboto.Chatbot.Dialogs
{
    public class NewEventDialog : ComponentDialog
    {
        ILogger<MainDialog> _logger;
        CalendarEvent calendarEvent;
        public NewEventDialog(ILogger<MainDialog> logger) : base(nameof(NewEventDialog))
        {
            _logger = logger;
            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ShowNewEventCard
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
                        Prompt = MessageFactory.Text("Tell me the event details please...")
                    }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> AskForEventDate(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AskForEventDate task fired on NewEventDialog");
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("What is the date of this event?")
            }, cancellationToken);
        }

        private async Task<DialogTurnResult> AskForStartTime(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("AskForStartTime task fired on NewEventDialog");

        }

    }
}
