using System;

namespace OcrMetadata.Model
{
    public interface IOcrResult
    {
        TimeSpan ProcessTime { get; }
        IImageContent Content { get; }
        Exception Error { get; }
        bool HasError { get; }
        bool TextFound { get; }
    }

    public abstract class OcrResult : IOcrResult
    {
        protected OcrResult(TimeSpan processTime, IImageContent imageContent, Exception error)
        {
            ProcessTime = processTime;
            Content = imageContent;
            Error = error;
        }

        public TimeSpan ProcessTime { get; }
        public IImageContent Content { get; }
        public Exception Error { get; }
        public bool HasError => Error != null;
        public bool TextFound => Content != null && Content.TextFound();
    }
}