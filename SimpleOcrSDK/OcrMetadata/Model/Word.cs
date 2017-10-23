using System;

namespace OcrMetadata.Model
{
    public interface IWord
    {
        /// <summary>
        /// Relative coordinate
        /// </summary>
        Coordinate Coordinate { get; }

        /// <summary>
        /// The text value
        /// </summary>
        string Value { get; }
    }

    public class Word : IWord
    {
        /// <inheritdoc />
        public Coordinate Coordinate { get; protected set; }

        /// <inheritdoc />
        public string Value { get; protected set; }

        public Word(Coordinate coordinate, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

            Coordinate = coordinate ?? throw new ArgumentNullException(nameof(coordinate));
            Value = value;
        }
    }
}