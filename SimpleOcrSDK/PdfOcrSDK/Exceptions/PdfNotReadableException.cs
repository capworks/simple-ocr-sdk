using System;

namespace PdfOcrSDK.Exceptions
{
    public class PdfNotReadableException : PdfReadException
    {
        public PdfNotReadableException(string msg) : base(msg) { }

        public PdfNotReadableException(string msg, Exception e) : base(msg, e) { }
    }
}