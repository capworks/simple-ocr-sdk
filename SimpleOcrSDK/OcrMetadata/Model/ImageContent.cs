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

        /// <summary>
        /// All sentences found on the image
        /// </summary>
        public List<Sentence> Sentences { get; }

        /// <summary>
        /// All words found on the image.
        /// </summary>
        public List<Word> Words { get;  }

        /// <summary>
        /// All lines found on the image. Dictionary with zero index line numbers as key, and list of sentences as value.
        /// </summary>
        public Dictionary<int, List<Sentence>> Lines { get; set; }

        public ImageContent(List<Sentence> sentences, int height, int width)
        {
            Height = height;
            Width = width;
            Sentences = sentences ?? throw new ArgumentNullException(nameof(sentences));
            Words = Sentences.SelectMany(x => x.Words).ToList();
            Lines = Sentences.GroupBy(x => x.Line).ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <summary>
        /// Get the content as a plain text string on one line.
        /// </summary>
        /// <param name="sentenceSeperator">Pr. default the text sentences is seperated by a space. You can define your own seperator, fx linebreak "\n" or colon ";"</param>
        /// <returns></returns>
        public string GetPlainText(string sentenceSeperator = " ")
        {
            return GetAggregatedValue(Sentences, sentenceSeperator);
        }

        /// <summary>
        /// Get the content as a plain text string, but each line is on a new line.
        /// </summary>
        /// <param name="sentenceSeperator">Pr. default the text sentences is seperated by a space. You can define your own seperator, fx linebreak "\n" or colon ";"</param>
        /// <returns></returns>
        public string GetPlainTextWithLineBreaks(string sentenceSeperator = " ")
        {
            return Lines.Aggregate("", (current, wd) => current + (string.IsNullOrEmpty(current)? "" : "\n") + GetAggregatedValue(wd.Value, sentenceSeperator));
        }

        private static string GetAggregatedValue(IEnumerable<Sentence> sentences, string sentenceSeperator)
        {
            return sentences.Aggregate("", (current, wd) => current + (string.IsNullOrEmpty(current) ? "" : sentenceSeperator) + wd.Value);
        }

        /// <summary>
        /// Is any text found on the image
        /// </summary>
        /// <returns>Returns true if any text was found.</returns>
        public bool TextFound()
        {
            return Sentences.Any();
        }
       
    }
}