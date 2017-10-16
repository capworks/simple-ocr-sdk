using System;

namespace PdfOcrSDK.Exceptions
{
    public class PdfReadException : Exception
    {
        public PdfReadException(string msg) : base(msg) { }

        public PdfReadException(string msg, Exception e) : base(msg, e) { }
    }
}