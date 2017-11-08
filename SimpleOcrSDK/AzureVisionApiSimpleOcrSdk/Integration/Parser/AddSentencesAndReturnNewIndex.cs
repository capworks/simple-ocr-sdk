using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ProjectOxford.Vision.Contract;
using OcrMetadata.Model;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public interface IAddSentencesAndReturnNewIndex
    {
        int Execute(int height, int width, KeyValuePair<Point, List<Line>> line, int lineCount, int sentenceIndex, List<ISentence> sentences);
    }

    public class AddSentencesAndReturnNewIndex : IAddSentencesAndReturnNewIndex
    {
        private readonly ITransformAzureLineIntoSentence _transformAzureLineIntoSentence;

        public AddSentencesAndReturnNewIndex(ITransformAzureLineIntoSentence transformAzureLineIntoSentence)
        {
            _transformAzureLineIntoSentence = transformAzureLineIntoSentence;
        }

        public int Execute(int height, int width, KeyValuePair<Point, List<Line>> line, int lineCount, int sentenceIndex, List<ISentence> sentences)
        {
            if (sentences == null) throw new ArgumentNullException(nameof(sentences));

            var orderedLine = line.Value.OrderBy(x => x.Rectangle.Left);
            foreach (var azureline in orderedLine)
            {
                var sentence =
                    _transformAzureLineIntoSentence.Execute(azureline, lineCount, width, height, sentenceIndex);
                sentences.Add(sentence);
                sentenceIndex++;
            }
            return sentenceIndex;
        }
    }
}