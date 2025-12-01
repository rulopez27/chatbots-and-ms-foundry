// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.22.0

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Roboto.Chatbot
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        private readonly Dialog _dialog;
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;
        public DialogBot(ConversationState conversationState, UserState userState, T dialog)
        {
            _conversationState = conversationState;
            _userState = userState;
            _dialog = dialog;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //Send welcome messsage
                    await turnContext.SendActivityAsync("Welcome to Roboto Schedule Assistant!", cancellationToken: cancellationToken);
                    
                    //Initiate main dialog
                    var dialogState = _conversationState.CreateProperty<DialogState>("DialogState");
                    await _dialog.RunAsync(turnContext, dialogState, cancellationToken);
                }
            }
        }

        public override async Task OnTurnAsync(ITurnContext turnContext,
            CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            //Save any state change
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var dialogState = _conversationState.CreateProperty<DialogState>("DialogState");
            await _dialog.RunAsync(turnContext, dialogState, cancellationToken);
        }
    }
}
