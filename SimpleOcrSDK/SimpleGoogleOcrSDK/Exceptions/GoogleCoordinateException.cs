using System;

namespace SimpleGoogleOcrSDK.Exceptions
{
    public class GoogleCoordinateException : Exception
    {
        public GoogleCoordinateException(string msg) : base(msg) { }
    }
}