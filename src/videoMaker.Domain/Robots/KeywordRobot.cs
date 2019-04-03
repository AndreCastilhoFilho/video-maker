using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1;
using IBM.WatsonDeveloperCloud.NaturalLanguageUnderstanding.v1.Model;
using IBM.WatsonDeveloperCloud.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace videoMaker.Domain.Robots
{
    public class NaturalLanguageUnderstandingRobot
    {
        private INaturalLanguageUnderstandingService _naturalLanguageUnderstandingService;

        public NaturalLanguageUnderstandingRobot(string apiKey, string url, string versionDate)
        {
            TokenOptions options = new TokenOptions()
            {
                IamApiKey = apiKey,
                ServiceUrl = url
            };

            try
            {
                _naturalLanguageUnderstandingService = new NaturalLanguageUnderstandingService(options, versionDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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
                        Limit = 8,
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
