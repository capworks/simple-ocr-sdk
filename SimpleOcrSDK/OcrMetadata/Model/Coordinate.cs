using System;

namespace OcrMetadata.Model
{
    public class Coordinate
    {
        public double X { get; }
        public double Y { get; }

        public double Height { get; }
        public double Width { get; }

        public Coordinate(double x, double y, double width, double height)
        {
            if (x < 0) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0) throw new ArgumentOutOfRangeException(nameof(y));
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));

            X = x;
            Y = y;
            Height = height;
            Width = width;
        }
    }
}