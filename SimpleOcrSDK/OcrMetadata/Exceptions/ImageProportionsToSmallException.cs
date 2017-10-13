using System;

namespace OcrMetadata.Exceptions
{
    public class ImageProportionsToSmallException : OcrException
    {
        public ImageProportionsToSmallException(string msg) : base(msg) { }

        public ImageProportionsToSmallException(string msg, Exception e) : base(msg, e) { }
    }
}