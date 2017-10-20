using System;
using OcrMetadata.Model;

namespace OcrMetadata.Tools
{
    public interface ICreateRelativeCoordinate
    {
        Coordinate Execute(double x, double y, double height, double width, double imgWidth, double imgHeight);
    }

    public class CreateRelativeCoordinate : ICreateRelativeCoordinate
    {
        public Coordinate Execute(double x, double y, double height, double width, double imgWidth, double imgHeight)
        {
            var xRelative = Math.Round(x / imgWidth, 3);
            var widthRelative = Math.Round(width / imgWidth, 3);

            var yRelative = Math.Round(y / imgHeight, 3);
            var heightRelative = Math.Round(height / imgHeight, 3);

            return new Coordinate(xRelative, yRelative, widthRelative, heightRelative);
        }
    }
}