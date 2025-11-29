using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Microsoft.Bot.Schema;


namespace Roboto.Bot.Cards
{
    public class AdaptiveCardHelper
    {
        private readonly string _resourceName;
        public AdaptiveCardHelper(string resourceName)
        {
            _resourceName = resourceName;
        }
        public Attachment GetAdaptiveCard()
        {
            string cardPath = GetType().Assembly.GetManifestResourceNames().First(name => name.EndsWith(_resourceName));
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
