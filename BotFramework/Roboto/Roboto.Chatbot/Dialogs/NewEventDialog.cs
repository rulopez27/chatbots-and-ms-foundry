using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Roboto.Chatbot.Cards;
using System.Threading;
using System.Threading.Tasks;

namespace Roboto.Chatbot.Dialogs
{
    public class NewEventDialog : ComponentDialog
    {
        ILogger<MainDialog> _logger;
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

    }
}
