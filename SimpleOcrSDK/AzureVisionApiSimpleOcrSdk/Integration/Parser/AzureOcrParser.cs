using System;
using System.Collections.Generic;
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
        private readonly IGetLinesOrderedByTopPosition _getLinesOrderedByTopPosition;

        public AzureOcrParser(ITransformLinesIntoSentences transformLinesIntoSentences,
            ISortIntoLogicalLines sortIntoLogicalLines,
            IGetLinesOrderedByTopPosition getLinesOrderedByTopPosition)
        {
            _transformLinesIntoSentences = transformLinesIntoSentences;
            _sortIntoLogicalLines = sortIntoLogicalLines;
            _getLinesOrderedByTopPosition = getLinesOrderedByTopPosition;
        }

        public ImageContent Execute(RawAzureOcrResult ocrOutput, int height, int width)
        {
            if (ocrOutput == null) throw new ArgumentNullException(nameof(ocrOutput));

            var azureLines = _getLinesOrderedByTopPosition.Execute(ocrOutput);
            if(azureLines == null)
                return new ImageContent(new List<ISentence>());

            var lines = _sortIntoLogicalLines.Execute(azureLines);
            var sentences = _transformLinesIntoSentences.Execute(height, width, lines);

            return new ImageContent(sentences);
        }
    }
}