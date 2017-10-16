namespace PdfOcrSDK.Model
{
    internal class PdfOcrResult
    {
        public CharItem[] Items;

        public double Height { get; set; }

        public double Width { get; set; }

        public string Keywords { get; set; }
    }
}