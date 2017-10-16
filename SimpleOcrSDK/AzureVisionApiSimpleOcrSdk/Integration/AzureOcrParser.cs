using System;
using System.Collections.Generic;
using System.Linq;
using AzureVisionApiSimpleOcrSdk.Model;
using Microsoft.ProjectOxford.Vision.Contract;
using OcrMetadata.Model;
using Word = Microsoft.ProjectOxford.Vision.Contract.Word;

namespace AzureVisionApiSimpleOcrSdk.Integration
{
    public class AzureOcrParser
    {
        public ImageContent Execute(RawAzureOcrResult ocrOutput, int height, int width)
        {
            var azureLines = ocrOutput.Regions?.ToList().SelectMany(x => x.Lines).OrderBy(x => x.Rectangle.Top).ToList() ?? new List<Line>();
            var lines = new Dictionary<Point, List<Line>>();
            foreach (var line in azureLines)
            {
                var point = new Point() { Top = line.Rectangle.Top, Bottom = line.Rectangle.Top + line.Rectangle.Height };
                var parentLine = lines.Where(x => x.Key.IsWithinThisPoint(point))
                    .OrderBy(x => x.Key.Top)
                    .Select(x => x.Value)
                    .FirstOrDefault();

                if (parentLine == null)
                {
                    parentLine = new List<Line>();
                    lines.Add(point, parentLine);
                }
                parentLine.Add(line);
            }

            var sentences = new List<Sentence>();
            int lineCount = 0,
                sentenceIndex = 0;
            foreach (var line in lines.ToList().OrderBy(x => x.Key.Top))
            {
                var orderedLine = line.Value.OrderBy(x => x.Rectangle.Left);
                foreach (var sentence in orderedLine)
                {
                    sentences.Add(CreateSentence(sentence, lineCount, width, height, sentenceIndex));
                    sentenceIndex++;
                }
                lineCount++;
            }

            return new ImageContent(sentences);
        }


        private static Sentence CreateSentence(Line line, int lineCount, int imgWidth, int imgHeight, int index)
        {
            if (line?.Words == null || !line.Words.Any()) return null;

            var words = line.Words.Select(word => CreateWord(word, imgWidth, imgHeight)).ToList();
            var text = words.Select(x => x.Value).Aggregate((i, j) => i + " " + j).Trim();

            return new Sentence(words, CreateRelativeCoordinate(line.Rectangle, imgWidth, imgHeight), text, lineCount, index);
        }

        private static OcrMetadata.Model.Word CreateWord(Word word, int imgWidth, int imgHeight)
        {
            var coord = CreateRelativeCoordinate(word.Rectangle, imgWidth, imgHeight);
            return new OcrMetadata.Model.Word(coord, word.Text);
        }

        private static Coordinate CreateRelativeCoordinate(Rectangle word, int imgWidth, int imgHeight)
        {
            double x = word.Left;
            double y = word.Top;
            double height = word.Height;
            double width = word.Width;

            var xRelative = Math.Round(x / imgWidth, 3);
            var widthRelative = Math.Round(width / imgWidth, 3);

            var yRelative = Math.Round(y / imgHeight, 3);
            var heightRelative = Math.Round(height / imgHeight, 3);

            return new Coordinate(xRelative, yRelative, widthRelative, heightRelative);
        }

        private struct Point
        {
            public int Top { get; set; }
            public int Bottom { get; set; }

            public bool IsWithinThisPoint(Point point)
            {
                return Top <= point.Top && Bottom >= point.Bottom;
            }
        }
    }
}