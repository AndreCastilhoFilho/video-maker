using System;
using System.Collections.Generic;
using System.Text;

namespace videoMaker.Domain.Models
{
    public class Content
    {
        public string SearchTerm { get; set; }
        public string Prefix { get; set; }
        public string SourceContentOriginal { get; set; }
        public string SourceContentSanitized { get; set; }
        public IList<Sentence> Sentences { get; set; }

        public class Sentence
        {
            public string Text { get; set; }
            public string[] Keywords { get; set; }
            public string[] Images { get; set; }
            public string GoogleSearchQuery { get; internal set; }
        }


        public override string ToString()
        {
            return $"{Prefix} {SearchTerm}";
        }
    }
}
