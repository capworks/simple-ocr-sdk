using System;
using System.Collections.Generic;
using System.Linq;

namespace OcrMetadata.Model
{
    public class ImageContent
    {
        /// <summary>
        /// Height of the image (viewing orientation taken into account)
        /// </summary>
        public decimal Height { get; protected set; }

        /// <summary>
        /// Width of the image (viewing orientation taken into account)
        /// </summary>
        public decimal Width { get; protected set; }

        public List<Sentence> Sentences { get; }
        public List<Word> Words { get;  }
        public Dictionary<int, List<Sentence>> Lines { get; set; }

        public string GetPlainText(string sentenceSeperator = " ")
        {
            return GetAggregatedValue(Sentences, sentenceSeperator);
        }
        public string GetPlainTextWithLineBreaks(string sentenceSeperator = " ")
        {
            return Lines.Aggregate("", (current, wd) => current + (string.IsNullOrEmpty(current)? "" : "\n") + GetAggregatedValue(wd.Value, sentenceSeperator));
        }

        private static string GetAggregatedValue(IEnumerable<Sentence> sentences, string sentenceSeperator)
        {
            return sentences.Aggregate("", (current, wd) => current + (string.IsNullOrEmpty(current) ? "" : sentenceSeperator) + wd.Value);
        }

        public bool TextFound()
        {
            return Sentences.Any();
        }

        public ImageContent(List<Sentence> sentences, int height, int width)
        {
            Height = height;
            Width = width;
            Sentences = sentences ?? throw new ArgumentNullException(nameof(sentences));
            Words =  Sentences.SelectMany(x => x.Words).ToList();
            Lines = Sentences.GroupBy(x => x.Line).ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}