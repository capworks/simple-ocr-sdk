using System;
using System.Collections.Generic;

namespace OcrMetadata.Model
{
    public interface ISentence
    {
        /// <summary>
        /// Sentence index
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Zero index line number
        /// </summary>
        int Line { get; }

        /// <summary>
        /// The aggreageted text values of the containing words.
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Relative coordinate
        /// </summary>
        Coordinate Coords { get; }

        /// <summary>
        /// List of words the sentence consists of
        /// </summary>
        List<IWord> Words { get; }
    }

    /// <summary>
    /// A Sentence in this context is not a grammatical sentence. 
    /// It's a collection of words on a horizontal line, 
    /// without any whitespaces larger then max a few spaces 
    /// (exact measure depending on font and image quality). 
    /// </summary>
    public class Sentence : ISentence
    {
        /// <inheritdoc />
        public int Index { get; protected set; }

        /// <inheritdoc />
        public int Line { get; protected set; }


        /// <inheritdoc />
        public string Value { get; set; }

        /// <inheritdoc />
        public Coordinate Coords { get; protected set; }

        /// <inheritdoc />
        public List<IWord> Words { get; protected set; }

        public Sentence(List<IWord> words, Coordinate coordinate, string value, int line, int index)
        {
            if (words == null || words.Count == 0) throw new ArgumentNullException(nameof(words));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));
            if (line < 0) throw new ArgumentOutOfRangeException(nameof(line));
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            Words = words;
            Coords = coordinate;
            Value = value;
            Line = line;
            Index = index;
        }
    }
}