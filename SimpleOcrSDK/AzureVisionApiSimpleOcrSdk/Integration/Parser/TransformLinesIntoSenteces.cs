using System.Collections.Generic;
using System.Linq;
using Microsoft.ProjectOxford.Vision.Contract;
using OcrMetadata.Model;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public interface ITransformLinesIntoSentences
    {
        List<Sentence> Execute(int height, int width, IOrderedEnumerable<KeyValuePair<Point, List<Line>>> logicalLines);
    }

    public class TransformLinesIntoSentences : ITransformLinesIntoSentences
    {
        private readonly ITransformAzureLineIntoSentence _transformAzureLineIntoSentence;

        public TransformLinesIntoSentences(ITransformAzureLineIntoSentence transformAzureLineIntoSentence)
        {
            _transformAzureLineIntoSentence = transformAzureLineIntoSentence;
        }

        public List<Sentence> Execute(int height, int width, IOrderedEnumerable<KeyValuePair<Point, List<Line>>> logicalLines)
        {
            var sentences = new List<Sentence>();
            int lineCount = 0,
                sentenceIndex = 0;
            foreach (var line in logicalLines)
            {
                var orderedLine = line.Value.OrderBy(x => x.Rectangle.Left);
                foreach (var azureline in orderedLine)
                {
                    var sentence = _transformAzureLineIntoSentence.Execute(azureline, lineCount, width, height, sentenceIndex);
                    sentences.Add(sentence);
                    sentenceIndex++;
                }
                lineCount++;
            }
            return sentences;
        }
    }
}