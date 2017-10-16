using System;

namespace OcrMetadata.Exceptions
{
    public class OcrException : Exception
    {
        public OcrException(string msg) : base(msg) { }
        public OcrException(string msg, Exception innerException) : base(msg, innerException) { }
    }
}