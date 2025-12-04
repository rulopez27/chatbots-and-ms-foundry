using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using Roboto.Chatbot.Cards;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Roboto.Chatbot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        ILogger<MainDialog> _logger;
        private const string PROCESSING_YOUR_REQUEST = "Sure! One moment, I am processing your request...";
        public MainDialog(ILogger<MainDialog> logger, IServiceProvider serviceProvider) : base(nameof(MainDialog))
        {
            _logger = logger;
            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ShowMenuAsync,
                HandleChoiceAsync,
                HandleResultAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(serviceProvider.GetRequiredService<NewEventDialog>());
        }

        private async Task<DialogTurnResult> HandleIntentAsync(RobotoIntents intent, WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            switch (intent)
            {
                case RobotoIntents.NewEvent:
                    return await stepContext.BeginDialogAsync(nameof(NewEventDialog), null, cancellationToken);
                case RobotoIntents.TodaysSchedule:
                    return await stepContext.BeginDialogAsync(nameof(NewEventDialog), null, cancellationToken);
                case RobotoIntents.CheckForConflicts:
                    return await stepContext.BeginDialogAsync(nameof(NewEventDialog), null, cancellationToken);
                case RobotoIntents.ListEvents:
                    return await stepContext.BeginDialogAsync(nameof(NewEventDialog), null, cancellationToken);
                case RobotoIntents.Welcome:
                    return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
                default:
                    return await HandleBadRequestAsync(stepContext, cancellationToken);
            }

        }

        private async Task<DialogTurnResult> HandleBadRequestAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var message = MessageFactory.Text("Sorry! I could not understand your request. Please try again.", InputHints.AcceptingInput);
            await stepContext.Context.SendActivityAsync(message, cancellationToken);
            return await stepContext.ReplaceDialogAsync(nameof(MainDialog), new PromptOptions { Prompt = message }, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ShowMenuAsync fired on MainDialog");

            Attachment welcomeCard;
            using (AdaptiveCardHelper cardHelper = new AdaptiveCardHelper("menuCard.json"))
            {
                welcomeCard = cardHelper.GetAdaptiveCard();

                await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(welcomeCard));
                return await stepContext.PromptAsync(nameof(TextPrompt), 
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("What can I do for you today?")
                    }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> HandleChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string userChoice = stepContext.Result?.ToString();
            await stepContext.Context.SendActivityAsync(PROCESSING_YOUR_REQUEST);
            return await HandleIntentAsync(RobotoIntents.NewEvent, stepContext, cancellationToken);
        }

        private async Task<DialogTurnResult> HandleResultAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("HandleResultAsync fired on MainDialog");

            RobotoIntents userChoice = (RobotoIntents)stepContext.Result;
            return await HandleIntentAsync(userChoice, stepContext, cancellationToken);
        }
    }
}
