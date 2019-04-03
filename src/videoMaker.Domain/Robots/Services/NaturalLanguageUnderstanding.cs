using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1;
using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1.Model;
using IBM.WatsonDeveloperCloud.Util;
using System;
using System.Linq;

namespace videoMaker.Domain.Robots
{
    public class NaturalLanguageUnderstanding
    {
        private INaturalLanguageUnderstandingService _naturalLanguageUnderstandingService;

        public NaturalLanguageUnderstanding(string apiKey, string url, string versionDate)
        {
            TokenOptions options = new TokenOptions()
            {
                IamApiKey = apiKey,
                ServiceUrl = url
            };

            _naturalLanguageUnderstandingService = new NaturalLanguageUnderstandingService(options, versionDate);
        }

        public string[] ReturnKeywords(string content)
        {
            try
            {
                return GetKeywordsFromContent(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        private string[] GetKeywordsFromContent(string content)
        {
            Parameters parameters = new Parameters()
            {
                Text = content,
                Features = new Features()
                {
                    Keywords = new KeywordsOptions()
                    {
                        Limit = 7,
                        Sentiment = true,
                        Emotion = true
                    }
                }
            };

            var result = _naturalLanguageUnderstandingService.Analyze(parameters);

            return result.Keywords.Select(x => x.Text).ToArray();
        }
    }
}
