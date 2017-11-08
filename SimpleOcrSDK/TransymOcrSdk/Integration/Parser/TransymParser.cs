using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OcrMetadata.Model;

namespace TransymOcrSdk.Integration.Parser
{
    public class TransymParser
    {
        private readonly ITransymCreateRelativeCoordinate _transymCreateRelativeCoordinate;

        public TransymParser(ITransymCreateRelativeCoordinate transymCreateRelativeCoordinate)
        {
            _transymCreateRelativeCoordinate = transymCreateRelativeCoordinate;
        }

        private readonly Regex _priceIdentifiers = new Regex(@"(\d|\.|,)");
        private readonly Regex _letter = new Regex(@"[A-Za-z]");

        public ImageContent Execute(TOcrResultStructures.TOcrResults ocrOutput)
        {
            var result = new List<ISentence>();
            if (ocrOutput.Hdr.NumItems > 0)
            {
                var line = 0;
                var index = 0;
                var temp = new List<TOcrResultStructures.TOcrResultsItem>();
                var space = false;
                var last = ocrOutput.Item[0];
                var count = ocrOutput.Hdr.NumItems;
                temp.Add(last);
                for (var i = 1; i < count; i++)
                {
                    var item = ocrOutput.Item[i];
                    if (item.OCRCha == 13 || (space && IsNewSentence(last, item)))
                    {
                        if (space)
                        {
                            temp.RemoveAt(temp.Count - 1);
                        }
                        space = false;
                        AddNewSentence(temp, line, result, ref index, (int) ocrOutput.Height, (int) ocrOutput.Width);
                        temp.Clear();

                        if (item.OCRCha == 13)
                        {
                            line++;
                        }
                        else
                        {
                            temp.Add(item);
                            last = item;
                        }
                    }
                    else
                    {
                        temp.Add(item);
                        if (item.OCRCha == 32)
                        {
                            space = true;
                        }
                        else
                        {
                            space = false;
                            last = item;
                        }
                    }
                }

                if (temp.Count > 0)
                {
                    AddNewSentence(temp, line, result, ref index, (int)ocrOutput.Height, (int)ocrOutput.Width);
                }
            }

            return new ImageContent(result);
        }

        private void AddNewSentence(List<TOcrResultStructures.TOcrResultsItem> temp, int line, List<ISentence> result, ref int index, int height, int width)
        {
            Trim(temp);
            if (temp.Count == 0) return;

            var value = temp.Aggregate("", (current, wd) => current + Convert.ToChar((short)wd.OCRCha));
            var start = temp.First();
            var end = temp.Last();

            var manyDigits = value.Length > 3 && value.Length < 13 && _priceIdentifiers.Matches(value).Count > value.Length / 2;
            if (manyDigits)
            {
                var possibleNumbers = temp.Where(x => x.Confidence < 0.75 && _letter.IsMatch(Convert.ToChar(x.OCRCha).ToString(), 0)).ToList();
                foreach (var pn in possibleNumbers)
                {
                    var s = Convert.ToChar(pn.OCRCha).ToString();
                    var sb = new StringBuilder(value);
                    if (s == "J")
                    {
                        var ii = temp.IndexOf(pn);
                        sb.Remove(ii, 1);
                        sb.Insert(ii, ",7");
                        temp.Insert(ii, pn);
                        value = sb.ToString();
                    }
                    else
                    {
                        var s2 = s.Replace("O", "0").Replace("I", "1").Replace("l", "1").Replace("S", "5").Replace("B", "8").ToCharArray().First();
                        sb[temp.IndexOf(pn)] = s2;
                        value = sb.ToString();
                    }
                }
            }

            var wordList = new List<IWord>();
            var words = value.Split(' ');
            var i = 0;
            foreach (var word in words)
            {
                if (temp.Count <= i)
                    continue;
                var wStart = temp[i];
                i = i + word.Length + 1;
                var wEnd = temp[i - 2];
                if (!string.IsNullOrWhiteSpace(word))
                    wordList.Add(new Word(CreateCoordinate(wStart, wEnd, width, height), word));
            }
            if (wordList.Count > 0)
            {
                var sen = new Sentence(wordList, CreateCoordinate(start, end, width, height), value, line, index);
                result.Add(sen);
                index++;
            }
        }

        private Coordinate CreateCoordinate(TOcrResultStructures.TOcrResultsItem start, TOcrResultStructures.TOcrResultsItem end, int width, int heigt)
        {
            return _transymCreateRelativeCoordinate.Execute(start, end, width, heigt);
        }

        private static bool IsNewSentence(TOcrResultStructures.TOcrResultsItem last, TOcrResultStructures.TOcrResultsItem item)
        {
            return (last.XPos + last.XDim + (Math.Max(item.YDim, last.YDim) * 2)) < item.XPos;
        }

        private static void Trim(List<TOcrResultStructures.TOcrResultsItem> temp)
        {
            List<TOcrResultStructures.TOcrResultsItem> toRemove = temp.TakeWhile(x => x.OCRCha == 32).ToList();
            toRemove.ForEach(x => temp.Remove(x));
            while (temp.Count > 0 && temp.Last().OCRCha == 32)
            {
                temp.Remove(temp.Last());
            }
        }
    }
}
