namespace PdfOcrSDK.Exceptions
{
    public class PdfContentToShortException : PdfReadException
    {
        public PdfContentToShortException(string msg) : base(msg) { }
    }
}