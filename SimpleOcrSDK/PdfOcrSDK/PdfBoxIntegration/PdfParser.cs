using System;
using System.Collections.Generic;
using System.Linq;
using OcrMetadata.Model;
using PdfOcrSDK.Model;

namespace PdfOcrSDK.PdfBoxIntegration
{
    internal class PdfParser
    {
        public ImageContent Execute(PdfOcrResult pdfOcrResult)
        {
            var height = pdfOcrResult.Height;
            var width = pdfOcrResult.Width;

            var list = pdfOcrResult.Items;
            foreach (var charItem in list)
            {
                if (string.IsNullOrWhiteSpace(charItem.Letter))
                    charItem.Letter = " ";
                else if (charItem.Letter.Length == 1)
                {
                    if (charItem.Letter.ToCharArray()[0] ==173)
                        charItem.Letter = "-";
                }
                else if (charItem.Letter.Length > 1)
                {
                    charItem.Letter = charItem.Letter.Substring(0, 1);
                }
            }

            list = list.Where(x => x.Left > 0 && x.Top > 0).OrderBy(x => x.Top).ToArray();
            var lines = list.Select(x => x.Top).Distinct()
                .Select(line => list.Where(x => Math.Abs(x.Top - line) < double.Epsilon).OrderBy(x => x.Left).ToList())
                .ToList();

            var result = new List<Sentence>();
            var lineCount = 0;
            var index = 0;

            var tempList = new List<CharItem>();
            CharItem last = null;

            foreach (var line in lines)
            {
                foreach (var current in line)
                {
                    if (IsNewSentence(last, current))
                    {
                        AddNewSentence(tempList, lineCount, result, ref index, height, width);
                    }

                    last = current;
                    tempList.Add(current);
                }

                AddNewSentence(tempList, lineCount, result, ref index, height, width);
                lineCount++;
                last = null;
            }
            return new ImageContent(result);
        }

        private bool IsNewSentence(CharItem last, CharItem current)
        {
            if (last == null)
                return false;
            if (current.Letter == "S" && last.Letter == "5")
            {

            }
            var newR = last.Right + (Math.Abs(last.Space) < double.Epsilon
                           ? Math.Max(last.Height, last.Width)
                           : last.Space * 1.3);
            var isNewSentence = newR < current.Left;
            return isNewSentence;
        }

        private void AddNewSentence(List<CharItem> temp, int line, List<Sentence> result, ref int index,
            double imgHeight, double imgWidth)
        {
            Trim(temp);
            if (temp.Count == 0) return;
            var value = temp.Aggregate("", (current, wd) => current + wd.Letter);

            var start = temp.First();
            var end = temp.Last();

            var wordList = new List<Word>();
            var words = value.Split(' ');
            var i = 0;
            foreach (var word in words)
            {
                if (temp.Count <= i)
                    continue;

                var wStart = temp[i];
                i = i + word.Length + 1;
                var wEnd = temp[i - 2];
                if (string.IsNullOrWhiteSpace(word))
                    continue;

                var coord = CreateRelativeCoordinate(imgWidth, imgHeight, wStart, wEnd);
                wordList.Add(new Word(coord, word));
            }
            if (wordList.Count > 0)
            {
                var coord = CreateRelativeCoordinate(imgWidth, imgHeight, start, end);
                var sen = new Sentence(wordList, coord, value, line, index);
                result.Add(sen);
                index++;
            }
            temp.Clear();
        }

        private static Coordinate CreateRelativeCoordinate(double imgWidth, double imgHeight, CharItem start,
            CharItem end)
        {
            var y = start.Top;
            var x = start.Left;

            var height = start.Bottom - y;
            var width = end.Right - x;

            var xRelative = Math.Round(x / imgWidth, 3);
            var widthRelative = Math.Round(width / imgWidth, 3);

            var yRelative = Math.Round(y / imgHeight, 3);
            var heightRelative = Math.Round(height / imgHeight, 3);

            return new Coordinate(xRelative, yRelative, widthRelative, heightRelative);
        }

        private static void Trim(List<CharItem> temp)
        {
            List<CharItem> toRemove = temp.TakeWhile(x => x.Letter == " ").ToList();
            toRemove.ForEach(x => temp.Remove(x));
            while (temp.Count > 0 && temp.Last().Letter == " ")
            {
                temp.Remove(temp.Last());
            }
        }
    }
}