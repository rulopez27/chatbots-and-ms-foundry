using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using Roboto.Chatbot.Cards;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Roboto.Chatbot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        ILogger<MainDialog> _logger;
        public MainDialog(ILogger<MainDialog> logger, IServiceProvider serviceProvider) : base(nameof(MainDialog))
        {
            _logger = logger;
            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ShowMenuAsync,
                HandleChoiceAsync,
                
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(serviceProvider.GetRequiredService<NewEventDialog>());
            AddDialog(serviceProvider.GetRequiredService<ScheduleDialog>());
        }

        private async Task<DialogTurnResult> HandleChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            RobotoIntents intent = RobotoIntents.Welcome;
            string choiceTitle = null;

            // Detect: is this an Adaptive Card submit or text input?
            if (stepContext.Context.Activity?.Value != null)
            {
                // Card submit: parse structured data
                var payload = stepContext.Context.Activity.Value as JObject ?? JObject.FromObject(stepContext.Context.Activity.Value);
                var intentStr = payload.Value<string>("intent");
                choiceTitle = payload.Value<string>("title");

                if (!string.IsNullOrEmpty(intentStr) && Enum.TryParse<RobotoIntents>(intentStr, out var parsedIntent))
                {
                    intent = parsedIntent;
                }
            }
            else
            {
                // Text input: parse user's typed text
                string userChoice = stepContext.Result?.ToString();
                choiceTitle = userChoice;

                if (!string.IsNullOrEmpty(userChoice))
                {
                    if (userChoice.Contains("event", StringComparison.OrdinalIgnoreCase))
                        intent = RobotoIntents.NewEvent;
                    else if (userChoice.Contains("today", StringComparison.OrdinalIgnoreCase))
                        intent = RobotoIntents.TodaysSchedule;
                    else if (userChoice.Contains("conflict", StringComparison.OrdinalIgnoreCase))
                        intent = RobotoIntents.CheckForConflicts;
                    else if (userChoice.Contains("list", StringComparison.OrdinalIgnoreCase))
                        intent = RobotoIntents.ListEvents;
                }
            }

            // Echo the choice back to chat
            if (!string.IsNullOrEmpty(choiceTitle))
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Sure! One moment, I am processing your '{choiceTitle}' request..."), cancellationToken);
            }

            // Forward intent to next step
            switch (intent)
            {
                case RobotoIntents.NewEvent:
                    return await stepContext.BeginDialogAsync(nameof(NewEventDialog), null, cancellationToken);
                case RobotoIntents.TodaysSchedule:
                    return await stepContext.BeginDialogAsync(nameof(ScheduleDialog), null, cancellationToken);
                case RobotoIntents.CheckForConflicts:
                case RobotoIntents.ListEvents:
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
                return EndOfTurn;
            }
        }

    }
}
