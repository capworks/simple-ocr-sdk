using System;
using OcrMetadata.Exceptions;

namespace PdfOcrSDK.Exceptions
{
    public class PdfReadException : OcrException
    {
        public PdfReadException(string msg) : base(msg) { }

        public PdfReadException(string msg, Exception e) : base(msg, e) { }
    }
}