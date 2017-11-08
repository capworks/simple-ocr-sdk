using System;
using OcrMetadata.Model;

namespace TransymOcrSdk.Model
{
    public class TransymOcrResult : OcrResult
    {
        protected TransymOcrResult(TimeSpan processTime, IImageContent imageContent, Exception error,
            RawTransymOcrResult rawAzureOcrResult) : base(processTime, imageContent, error)
        {
            RawResult = rawAzureOcrResult;
        }

        public static TransymOcrResult CreateSuccesResult(TimeSpan processTime, IImageContent imageContent, RawTransymOcrResult rawAzureOcrResult)
        {
            if (imageContent == null) throw new ArgumentNullException(nameof(imageContent));
            if (rawAzureOcrResult == null) throw new ArgumentNullException(nameof(rawAzureOcrResult));
            return new TransymOcrResult(processTime, imageContent, null, rawAzureOcrResult);
        }

        public static TransymOcrResult CreateErrorResult(TimeSpan processTime, Exception error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            return new TransymOcrResult(processTime, null, error, null);
        }

        public RawTransymOcrResult RawResult { get; }
    }
}