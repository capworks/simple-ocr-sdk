using System;

namespace SimpleOcrSDK.Exceptions
{
    public class GoogleCoordinateException : Exception
    {
        public GoogleCoordinateException(string msg) : base(msg) { }
    }
}