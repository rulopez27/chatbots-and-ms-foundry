// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.22.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json.Linq;
using Roboto.Models;

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
            
            //Check if the activity has a value payload (from adaptive card submit)
            if (turnContext.Activity?.Value != null)
            {
                //Get the payload as JObject
                var payload = turnContext.Activity.Value as JObject ?? JObject.FromObject(turnContext.Activity.Value);
                
                //Check if it's a new event submission
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

                    //Respond to user
                    await turnContext.SendActivityAsync(MessageFactory.Text($"New event created: {newEvent}"), cancellationToken);
                    
                    return; //Exit after handling the adaptive card submission
                }
            }
            await _dialog.RunAsync(turnContext, dialogState, cancellationToken);
        }
    }
}
