using System;
using OcrMetadata.Model;

namespace AzureVisionApiSimpleOcrSdk.Model
{
    public class AzureOcrResult : OcrResult
    {
        protected AzureOcrResult(TimeSpan processTime, IImageContent imageContent, Exception error,
            RawAzureOcrResult rawAzureOcrResult) : base(processTime, imageContent, error)
        {
            RawResult = rawAzureOcrResult;
        }

        public static AzureOcrResult CreateSuccesResult(TimeSpan processTime, IImageContent imageContent, RawAzureOcrResult rawAzureOcrResult)
        {
            if (imageContent == null) throw new ArgumentNullException(nameof(imageContent));
            if (rawAzureOcrResult == null) throw new ArgumentNullException(nameof(rawAzureOcrResult));
            return new AzureOcrResult(processTime, imageContent, null, rawAzureOcrResult);
        }

        public static AzureOcrResult CreateErrorResult(TimeSpan processTime, Exception error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            return new AzureOcrResult(processTime, null, error, null);
        }

        public RawAzureOcrResult RawResult { get; }
    }
}