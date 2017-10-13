using System;
using OcrMetadata.Model;

namespace PdfOcrSDK.Model
{
    public class PdfResult : OcrResult
    {
        protected PdfResult(TimeSpan processTime, ImageContent imageContent, Exception error) 
            : base(processTime, imageContent, error)
        {
        }

        public static PdfResult CreateSuccesResult(TimeSpan processTime, ImageContent imageContent)
        {
            if (imageContent == null) throw new ArgumentNullException(nameof(imageContent));
            return new PdfResult(processTime, imageContent, null);
        }

        public static PdfResult CreateErrorResult(TimeSpan processTime, Exception error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            return new PdfResult(processTime, null, error);
        }
    }
}