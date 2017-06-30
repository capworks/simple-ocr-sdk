using System;

namespace OcrMetadata.Model
{
    public class Word
    {
        /// <summary>
        /// Relative coordinate
        /// </summary>
        public Coordinate Coordinate { get; protected set; }

        /// <summary>
        /// The text value
        /// </summary>
        public string Value { get; protected set; }

        public Word(Coordinate coordinate, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

            Coordinate = coordinate ?? throw new ArgumentNullException(nameof(coordinate));
            Value = value;
        }
    }
}