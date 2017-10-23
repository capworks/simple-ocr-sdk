using System;
using System.Collections.Generic;
using System.Linq;

namespace OcrMetadata.Model
{
    public interface IImageContent
    {
        /// <summary>
        /// All sentences found on the image
        /// </summary>
        List<ISentence> Sentences { get; }

        /// <summary>
        /// All words found on the image.
        /// </summary>
        List<IWord> Words { get; }

        /// <summary>
        /// All lines found on the image. Dictionary with zero index line numbers as key, and list of sentences as value.
        /// </summary>
        Dictionary<int, List<ISentence>> Lines { get; set; }

        /// <summary>
        /// Get the content as a plain text string on one line.
        /// </summary>
        /// <param name="sentenceSeperator">Pr. default the text sentences is seperated by a space. You can define your own seperator, fx linebreak "\n" or colon ";"</param>
        /// <returns></returns>
        string GetPlainText(string sentenceSeperator = " ");

        /// <summary>
        /// Get the content as a plain text string, but each line is on a new line.
        /// </summary>
        /// <param name="sentenceSeperator">Pr. default the text sentences is seperated by a space. You can define your own seperator, fx linebreak "\n" or colon ";"</param>
        /// <returns></returns>
        string GetPlainTextWithLineBreaks(string sentenceSeperator = " ");

        /// <summary>
        /// Is any text found on the image
        /// </summary>
        /// <returns>Returns true if any text was found.</returns>
        bool TextFound();
    }

    public class ImageContent : IImageContent
    {
        /// <inheritdoc />
        public List<ISentence> Sentences { get; }

        /// <inheritdoc />
        public List<IWord> Words { get;  }

        /// <inheritdoc />
        public Dictionary<int, List<ISentence>> Lines { get; set; }

        public ImageContent(List<ISentence> sentences)
        {
            Sentences = sentences ?? throw new ArgumentNullException(nameof(sentences));
            Words = Sentences.SelectMany(x => x.Words).ToList();
            Lines = Sentences.GroupBy(x => x.Line).ToDictionary(g => g.Key, g => g.ToList());
        }

        /// <inheritdoc />
        public string GetPlainText(string sentenceSeperator = " ")
        {
            return GetAggregatedValue(Sentences, sentenceSeperator);
        }

        /// <inheritdoc />
        public string GetPlainTextWithLineBreaks(string sentenceSeperator = " ")
        {
            return Lines.Aggregate("", (current, wd) => current + (string.IsNullOrEmpty(current)? "" : "\n") + GetAggregatedValue(wd.Value, sentenceSeperator));
        }

        private static string GetAggregatedValue(IEnumerable<ISentence> sentences, string sentenceSeperator)
        {
            return sentences.Aggregate("", (current, wd) => current + (string.IsNullOrEmpty(current) ? "" : sentenceSeperator) + wd.Value);
        }

        /// <inheritdoc />
        public bool TextFound()
        {
            return Sentences.Any();
        }
    }
}