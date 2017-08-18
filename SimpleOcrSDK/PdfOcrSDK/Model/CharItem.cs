namespace PdfOcrSDK.Model
{
    internal class CharItem
    {
        public CharItem(string letter, double top, double height, double left, double width, double space)
        {
            Letter = letter;
            Top = top;
            Bottom = top + height;
            Left = left;
            Right = left + width;
            Space = space;
            Width = width;
            Height = height;
        }

        public string Letter;
        public double Space;
        public double Left;
        public double Right;
        public double Top;
        public double Bottom;
        public double Height;
        public double Width;
    }
}