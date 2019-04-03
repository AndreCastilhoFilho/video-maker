using Algorithmia;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using videoMaker.Domain.Models;
using static videoMaker.Domain.Models.Content;

namespace videoMaker.Domain.Robots
{
    public class Text
    {
        public Content _content { get; set; }
        private RobotSettings _settings { get; set; }

        private NaturalLanguageUnderstanding _nluService;

        public Text(RobotSettings settings)
        {
            _content = State.Load();
            _settings = settings;
            _nluService = new NaturalLanguageUnderstanding
                (settings.NluApiKey, settings.NluUrl, settings.NluVersionDate);
        }
        public void Robot()
        {
            FetchContentFromWikipedia();
            SanitizeContent();
            BreakContentIntoSentences();
            FetchKeywordsOfAllSentences();

            State.Save(_content);

            Console.WriteLine($"Recebi com sucesso o content:{ _content }");
        }

        private void FetchKeywordsOfAllSentences()
        {
            foreach (var s in _content.Sentences)
            {
                s.Keywords = _nluService.ReturnKeywords(s.Text);
            }
        }

        private void FetchContentFromWikipedia()
        {
            var client = new Client(_settings.ApiKey);
            var algorithm = client.algo(_settings.WikipidiaAlgorithm);

            var wikipidiaResponse = algorithm.pipe<object>(_content.SearchTerm);
            var wikipidiaContent = wikipidiaResponse.result.ToString();


            _content.SourceContentOriginal = wikipidiaContent;
        }

        private void SanitizeContent()
        {
            var withoutBlankLines = RemoveBlankLines(_content.SourceContentOriginal);
            var withoutMarkDown = RemoveMarkdown(withoutBlankLines);
            var withoutDateInParenteses = RemoveDateInParentheses(withoutMarkDown);

            _content.SourceContentSanitized = withoutDateInParenteses;
        }

        private string RemoveDateInParentheses(string text)
        {
            return Regex.Replace(text, @"/\((?:\([^()]*\)|[^()])*\)", string.Empty, RegexOptions.Multiline);

        }

        private string RemoveMarkdown(string text)
        {
            return Regex.Replace(text, @"^=*", string.Empty, RegexOptions.Multiline).Replace("===", "").Replace("==", "").Replace("{", "").Replace("}", "");
        }

        private string RemoveBlankLines(string text)
        {
            return Regex.Replace(text, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline)
                .Replace("\\n", "\r\n");
        }

        private void BreakContentIntoSentences()
        {
            var sentences = PragmaticSegmenterNet.Segmenter.Segment(_content.SourceContentSanitized).ToList<string>()
                .Select(x => new Sentence() { Text = x })
                .ToList();

            _content.Sentences = sentences.Take(_settings.MaximunSentences).ToList();
        }
    }
}
