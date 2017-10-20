using System;
using AzureVisionApiSimpleOcrSdk.Model;
using OcrMetadata.Model;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public interface IAzureOcrParser
    {
        ImageContent Execute(RawAzureOcrResult ocrOutput, int height, int width);
    }

    public class AzureOcrParser : IAzureOcrParser
    {
        private readonly ITransformLinesIntoSentences _transformLinesIntoSentences;
        private readonly ISortIntoLogicalLines _sortIntoLogicalLines;
        private readonly IGetLinesOrDefaultOrderedByTopPosition _getLinesOrDefaultOrderedByTopPosition;

        public AzureOcrParser(ITransformLinesIntoSentences transformLinesIntoSentences,
            ISortIntoLogicalLines sortIntoLogicalLines,
            IGetLinesOrDefaultOrderedByTopPosition getLinesOrDefaultOrderedByTopPosition)
        {
            _transformLinesIntoSentences = transformLinesIntoSentences;
            _sortIntoLogicalLines = sortIntoLogicalLines;
            _getLinesOrDefaultOrderedByTopPosition = getLinesOrDefaultOrderedByTopPosition;
        }

        public ImageContent Execute(RawAzureOcrResult ocrOutput, int height, int width)
        {
            if (ocrOutput == null) throw new ArgumentNullException(nameof(ocrOutput));

            var azureLines = _getLinesOrDefaultOrderedByTopPosition.Execute(ocrOutput);
            var lines = _sortIntoLogicalLines.Execute(azureLines);
            var sentences = _transformLinesIntoSentences.Execute(height, width, lines);

            return new ImageContent(sentences);
        }
    }
}