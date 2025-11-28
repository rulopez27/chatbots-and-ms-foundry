// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.22.0

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Roboto.Bot
{
    public class RobotoBot : ActivityHandler
    {
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    Attachment welcomeCard = CreateWelcomeAdptiveCard();
                    var response = MessageFactory.Attachment(welcomeCard,"Welcome to Roboto Schedule Assistant!", null, null);
                    await turnContext.SendActivityAsync(response, cancellationToken);
                }
            }
        }

        private Attachment CreateWelcomeAdptiveCard()
        {
            string cardPath = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith("welcomeCard.json"));
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(cardPath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string adaptiveCard = reader.ReadToEnd();
                    return new Attachment()
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = JsonConvert.DeserializeObject(adaptiveCard, new JsonSerializerSettings { MaxDepth = null })
                    };
                }
            }
        }
    }
}
