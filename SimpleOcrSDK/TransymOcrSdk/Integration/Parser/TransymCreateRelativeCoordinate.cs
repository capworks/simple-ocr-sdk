using System;
using OcrMetadata.Model;
using OcrMetadata.Tools;

namespace TransymOcrSdk.Integration.Parser
{
    public interface ITransymCreateRelativeCoordinate
    {
        Coordinate Execute(TOcrResultStructures.TOcrResultsItem start, TOcrResultStructures.TOcrResultsItem end, int imgWidth, int imgHeight);
    }

    public class TransymCreateRelativeCoordinate : ITransymCreateRelativeCoordinate
    {
        private readonly ICreateRelativeCoordinate _createRelativeCoordinate;

        public TransymCreateRelativeCoordinate(ICreateRelativeCoordinate createRelativeCoordinate)
        {
            _createRelativeCoordinate = createRelativeCoordinate;
        }

        public Coordinate Execute(TOcrResultStructures.TOcrResultsItem start, TOcrResultStructures.TOcrResultsItem end, int imgWidth, int imgHeight)
        {
            double x = start.XPos;
            double y = start.YPos;
            double height = Math.Max(start.YDim, end.XDim);
            double width = end.XPos + end.XDim - start.XPos;

            return _createRelativeCoordinate.Execute(x, y, height, width, imgHeight, imgWidth);
        }
    }
}