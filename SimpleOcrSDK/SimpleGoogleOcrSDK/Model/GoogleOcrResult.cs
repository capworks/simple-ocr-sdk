using System;
using OcrMetadata.Model;

namespace SimpleGoogleOcrSDK.Model
{
    public class GoogleOcrResult : OcrResult
    {
        protected GoogleOcrResult(TimeSpan processTime, ImageContent imageContent, Exception error,
            RawGoogleOcrResult rawGoogleOcrResult) : base(processTime, imageContent, error)
        {
            RawResult = rawGoogleOcrResult;
        }

        public static GoogleOcrResult CreateSuccesResult(TimeSpan processTime, ImageContent imageContent, RawGoogleOcrResult rawGoogleOcrResult)
        {
            if (imageContent == null) throw new ArgumentNullException(nameof(imageContent));
            if (rawGoogleOcrResult == null) throw new ArgumentNullException(nameof(rawGoogleOcrResult));
            return new GoogleOcrResult(processTime, imageContent, null, rawGoogleOcrResult);
        }

        public static GoogleOcrResult CreateErrorResult(TimeSpan processTime, Exception error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            return new GoogleOcrResult(processTime, null, error, null);
        }

        public RawGoogleOcrResult RawResult { get; }
    }
}