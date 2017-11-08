using System;
using OcrMetadata.Exceptions;

namespace TransymOcrSdk.Exceptions
{
    public class TransymException : OcrException
    {
        public TransymException(string msg) : base(msg) { }

        public TransymException(string msg, Exception e) : base(msg, e) { }
    }
}
