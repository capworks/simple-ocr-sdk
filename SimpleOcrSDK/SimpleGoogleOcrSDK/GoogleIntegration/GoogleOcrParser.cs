using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Vision.v1.Data;
using OcrMetadata.Model;
using SimpleGoogleOcrSDK.Model;
using Word = OcrMetadata.Model.Word;

namespace SimpleGoogleOcrSDK.GoogleIntegration
{
    internal class GoogleOcrParser
    {
        public ImageContent Execute(RawGoogleOcrResult output, int imgHeight, int imgWidth)
        {
            var sentences = new List<Sentence>();
            if (!output.TextFound()) return new ImageContent(sentences);

            var words = output?.EntityAnnotations?.ToList().Skip(1).Select(AsWord).Where(x => x != null).OrderBy(x => x.Top).ToList();
            var lines = new Dictionary<Point, List<GoogleWord>>();
            foreach (var word in words)
            {
                var point = new Point() { Top = word.Top, Bottom = word.Bottom };
                var parentLine = lines.Where(x => x.Key.IsWithinThisPoint(point))
                    .OrderBy(x => x.Key.Top)
                    .Select(x => x.Value)
                    .FirstOrDefault();

                if (parentLine == null)
                {
                    parentLine = new List<GoogleWord>();
                    lines.Add(point, parentLine);
                }
                parentLine.Add(word);
            }

            int lineCount = 0, sentenceIndex = 0;
            foreach (var line in lines.ToList().OrderBy(x => x.Key.Top))
            {
                var orderedLine = line.Value.OrderBy(x => x.Left);
                sentences.AddRange(CreateSentences(orderedLine, lineCount, imgWidth, imgHeight, ref sentenceIndex));
                lineCount++;
            }

            return new ImageContent(sentences);
        }

        private static GoogleWord AsWord(EntityAnnotation arg)
        {
            var vertices = arg.BoundingPoly.Vertices;

            var yArray = vertices.Where(x => x.Y != null && x.Y >= 0).Select(x => x.Y.Value).Distinct();
            var xArray = vertices.Where(x => x.X != null && x.X >= 0).Select(x => x.X.Value).Distinct();

            if (xArray.Count() < 2 || yArray.Count() < 2)
            {
                return null;
            }

            return new GoogleWord()
            {
                Bottom = yArray.Max(),
                Top = yArray.Min(),
                Left = xArray.Min(),
                Right = xArray.Max(),
                Description = arg.Description
            };
        }

        private Coordinate CreateRelativeCoordinate(GoogleWord word, int imgWidth, int imgHeight)
        {
            double x = word.Left;
            double y = word.Top;
            var height = word.Bottom - y;
            var width = word.Right - x;

            var xRelative = Math.Round(x / imgWidth, 3);
            var widthRelative = Math.Round(width / imgWidth, 3);

            var yRelative = Math.Round(y / imgHeight, 3);
            var heightRelative = Math.Round(height / imgHeight, 3);

            return new Coordinate(xRelative, yRelative, widthRelative, heightRelative);
        }

        private IEnumerable<Sentence> CreateSentences(IEnumerable<GoogleWord> line, int lineCount, int imgWidth, int imgHeight, ref int index)
        {
            if (line == null || !line.Any()) return null;

            var words = line.Select(word => new Word(CreateRelativeCoordinate(word, imgWidth, imgHeight), word.Description)).ToList();

            var sentences = new List<List<Word>>();
            var last = words[0];
            sentences.Add(new List<Word>() { last });

            for (var i = 1; i < words.Count; i++)
            {
                var word = words[i];
                if (word.Coordinate.X < last.Coordinate.X + last.Coordinate.Width + last.Coordinate.Height * 1.5)
                {
                    sentences.Last().Add(word);
                }
                else
                {
                    sentences.Add(new List<Word>() { word });
                }

                last = word;
            }

            var result = new List<Sentence>();
            foreach (var sentence in sentences)
            {
                var text = sentence.Select(x => x.Value).Aggregate((i, j) => i + " " + j).Trim();
                result.Add(new Sentence(sentence, GetCoordsForSentence(sentence), text, lineCount, index));
                index++;
            }
            return result;
        }

        private static Coordinate GetCoordsForSentence(IEnumerable<Word> sentence)
        {
            var xCoord = sentence.Min(x => x.Coordinate.X);
            var yCoord = sentence.Min(x => x.Coordinate.Y);

            var width = sentence.Max(x => x.Coordinate.X + x.Coordinate.Width) - xCoord;
            var height = sentence.Max(x => x.Coordinate.Y + x.Coordinate.Height) - yCoord;

            return new Coordinate(x: xCoord, y: yCoord, width: width, height: height);
        }

        private class GoogleWord
        {
            public int Top { get; set; }
            public int Bottom { get; set; }
            public int Left { get; set; }
            public int Right { get; set; }
            public string Description { get; set; }
        }

        private struct Point
        {
            public int Top { get; set; }
            public int Bottom { get; set; }

            public bool IsWithinThisPoint(Point point)
            {
                if (point.Top == Top && point.Bottom == Bottom) return true;
                var centerOfPoint = point.Top + ((point.Bottom - point.Top) / 2);
                return centerOfPoint >= Top && centerOfPoint <= Bottom;
            }
        }
    }
}