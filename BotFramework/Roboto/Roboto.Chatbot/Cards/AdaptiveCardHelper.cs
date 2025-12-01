using Newtonsoft.Json;
using System.IO;
using Microsoft.Bot.Schema;
using System.Reflection;
using System;
using System.Linq;


namespace Roboto.Chatbot.Cards
{
    public class AdaptiveCardHelper : IDisposable
    {
        private Stream _resourceStream;

        public AdaptiveCardHelper(string adaptieCardResourceName)
        {
            string resourceName = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith(adaptieCardResourceName));
            _resourceStream = GetType().Assembly.GetManifestResourceStream(resourceName);
        }
        public Attachment GetAdaptiveCard()
        {
            using (StreamReader reader = new StreamReader(_resourceStream))
            {
                string adaptiveCard = reader.ReadToEnd();
                return new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(adaptiveCard, new JsonSerializerSettings { MaxDepth = null })
                };
            }
        }

        public void Dispose()
        {
            _resourceStream.Dispose();
        }
    }
}
