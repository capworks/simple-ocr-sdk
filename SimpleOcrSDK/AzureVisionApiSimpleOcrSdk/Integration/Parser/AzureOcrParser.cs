using System.Collections.Generic;
using System.Linq;
using AzureVisionApiSimpleOcrSdk.Model;
using Microsoft.ProjectOxford.Vision.Contract;
using OcrMetadata.Model;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public class AzureOcrParser
    {
        private readonly TransformLinesIntoSenteces _transformLinesIntoSenteces;
        private readonly SortIntoLogicalLines _sortIntoLogicalLines;

        public AzureOcrParser(TransformLinesIntoSenteces transformLinesIntoSenteces, SortIntoLogicalLines sortIntoLogicalLines)
        {
            _transformLinesIntoSenteces = transformLinesIntoSenteces;
            _sortIntoLogicalLines = sortIntoLogicalLines;
        }

        public ImageContent Execute(RawAzureOcrResult ocrOutput, int height, int width)
        {
            var azureLines = ocrOutput.Regions?.ToList().SelectMany(x => x.Lines).OrderBy(x => x.Rectangle.Top).ToList() ?? new List<Line>();
            var lines = _sortIntoLogicalLines.Execute(azureLines);
            var sentences = _transformLinesIntoSenteces.Execute(height, width, lines);

            return new ImageContent(sentences);
        }
    }
}