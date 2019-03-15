﻿using Algorithmia;
using System;
using System.Text.RegularExpressions;
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
            SanitizeContent();
            //BrakContentIntoSentences();

            Console.WriteLine($"Recebi com sucesso o content:{ Content.ToString() }");

        }

        private void FetchContentFromWikipedia()
        {
            var client = new Client(RobotSettings.ApiKey);
            var algorithm = client.algo(RobotSettings.WikipidiaAlgorithm);

            var wikipidiaResponse = algorithm.pipe<object>(Content.SearchTerm);
            var wikipidiaContent = wikipidiaResponse.result.ToString();


            Content.SourceContentOriginal = wikipidiaContent;

        }

        private void SanitizeContent()
        {
            var withoutBlankLines = RemoveBlankLines(Content.SourceContentOriginal);
            var withoutMarkDown = RemoveMarkdown(withoutBlankLines);
            var withoutDateInParenteses = RemoveDateInParentheses(withoutMarkDown);

            Content.SourceContentSanitized = withoutDateInParenteses;

            Console.WriteLine(withoutDateInParenteses);

        }

        private string RemoveDateInParentheses(string text)
        {
            return Regex.Replace(text, @"/\((?:\([^()]*\)|[^()])*\)", string.Empty, RegexOptions.Multiline);

        }

        private string RemoveMarkdown(string text)
        {
            return Regex.Replace(text, @"^=*", string.Empty, RegexOptions.Multiline).Replace("===", "").Replace("==", "");
        }

        private string RemoveBlankLines(string text)
        {
            return Regex.Replace(text, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline)
                .Replace("\\n", "\r\n");
        }

        private void BrakContentIntoSentences()
        {
            throw new NotImplementedException();
        }
    }
}