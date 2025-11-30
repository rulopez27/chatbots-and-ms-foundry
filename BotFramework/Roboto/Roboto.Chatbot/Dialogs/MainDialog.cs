using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using Roboto.Chatbot.Cards;
using Microsoft.Bot.Schema;
using System.Linq;
using Microsoft.Bot.Builder;

namespace Roboto.Chatbot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        public MainDialog() : base(nameof(MainDialog))
        {
            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ShowMenuAsync,
                HandleResultAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
        }

        private async Task<DialogTurnResult> ShowMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Attachment welcomeCard = AdaptiveCardHelper.GetAdaptiveCard(GetType().Assembly.GetManifestResourceNames().First(name => name.Equals("welcomeCard.json")));
            await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(welcomeCard));
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Welcome to Roboto Schedule Organizar!") }, cancellationToken);
        }

        private async Task<DialogTurnResult> HandleResultAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userChoice = stepContext.Result?.ToString();
            await stepContext.Context.SendActivityAsync($"{userChoice}");
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
