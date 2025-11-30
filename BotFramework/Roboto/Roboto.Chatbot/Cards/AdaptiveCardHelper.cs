using Newtonsoft.Json;
using System.IO;
using Microsoft.Bot.Schema;


namespace Roboto.Chatbot.Cards
{
    public static class AdaptiveCardHelper
    {
        public static Attachment GetAdaptiveCard(string resourceFilePath)
        {
            using (StreamReader reader = new StreamReader(resourceFilePath))
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
