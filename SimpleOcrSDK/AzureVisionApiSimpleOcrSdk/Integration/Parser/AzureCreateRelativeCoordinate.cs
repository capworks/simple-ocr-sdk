using Microsoft.ProjectOxford.Vision.Contract;
using OcrMetadata.Model;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public class AzureCreateRelativeCoordinate
    {
        private readonly CreateRelativeCoordinate _createRelativeCoordinate;

        public AzureCreateRelativeCoordinate(CreateRelativeCoordinate createRelativeCoordinate)
        {
            _createRelativeCoordinate = createRelativeCoordinate;
        }
        public Coordinate Execute(Rectangle word, int imgWidth, int imgHeight)
        {
            double x = word.Left;
            double y = word.Top;
            double height = word.Height;
            double width = word.Width;

            return _createRelativeCoordinate.Execute(x, y, height, width, imgWidth, imgHeight);
        }
    }
}