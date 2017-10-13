using System;

namespace OcrMetadata.Exceptions
{
    public class ImageProportionsToLargeException : OcrException
    {
        public ImageProportionsToLargeException(string msg) : base(msg) { }

        public ImageProportionsToLargeException(string msg, Exception e) : base(msg, e) { }
    }
}