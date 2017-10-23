using System;
using System.Collections.Generic;
using Microsoft.ProjectOxford.Vision.Contract;
using OcrMetadata.Model;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public interface ITransformLinesIntoSentences
    {
        List<ISentence> Execute(int height, int width, IEnumerable<KeyValuePair<Point, List<Line>>> logicalLines);
    }

    public class TransformLinesIntoSentences : ITransformLinesIntoSentences
    {
        private readonly IAddSentencesAndReturnNewIndex _addSentencesAndReturnNewIndex;

        public TransformLinesIntoSentences(IAddSentencesAndReturnNewIndex addSentencesAndReturnNewIndex)
        {
            _addSentencesAndReturnNewIndex = addSentencesAndReturnNewIndex;
        }

        public List<ISentence> Execute(int height, int width,
            IEnumerable<KeyValuePair<Point, List<Line>>> logicalLines)
        {
            if (logicalLines == null) throw new ArgumentNullException(nameof(logicalLines));

            var sentences = new List<ISentence>();
            int lineCount = 0,
                sentenceIndex = 0;
            foreach (var line in logicalLines)
            {
                sentenceIndex =
                    _addSentencesAndReturnNewIndex.Execute(height, width, line, lineCount, sentenceIndex, sentences);
                lineCount++;
            }
            return sentences;
        }
    }
}