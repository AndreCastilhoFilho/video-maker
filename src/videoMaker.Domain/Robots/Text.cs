using Algorithmia;
using System;
using videoMaker.Domain.Models;

namespace videoMaker.Domain.Robots
{
    public class Text
    {
        public Content Content { get; set; }

        public Text(Content content)
        {
            Content = content;
        }


        public void Robot()
        {

            FetchContentFromWikipedia();
            // SanitizeContent();
            //  BrakContentIntoSentences();

            Console.WriteLine($"Recebi com sucesso o content:{ Content.ToString() }");

        }

        private void FetchContentFromWikipedia()
        {
            var client = new Client(RobotSettings.ApiKey);
            var algorithm = client.algo(RobotSettings.WikipidiaAlgorithm);

            var wikipidiaResponse = algorithm.pipe<object>(Content.SearchTerm);
            var wikipidiaContent = wikipidiaResponse.result.ToString();

            Console.WriteLine(wikipidiaContent);
        }

        private void SanitizeContent()
        {
            throw new NotImplementedException();
        }

        private void BrakContentIntoSentences()
        {
            throw new NotImplementedException();
        }
    }
}
