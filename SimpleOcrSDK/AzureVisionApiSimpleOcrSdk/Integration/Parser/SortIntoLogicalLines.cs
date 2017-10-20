using System.Collections.Generic;
using System.Linq;
using Microsoft.ProjectOxford.Vision.Contract;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public interface ISortIntoLogicalLines
    {
        IOrderedEnumerable<KeyValuePair<Point, List<Line>>> Execute(IEnumerable<Line> azureLines);
    }

    public class SortIntoLogicalLines : ISortIntoLogicalLines
    {
        public IOrderedEnumerable<KeyValuePair<Point, List<Line>>> Execute(IEnumerable<Line> azureLines)
        {
            var lines = new Dictionary<Point, List<Line>>();
            foreach (var line in azureLines)
            {
                var point = new Point { Top = line.Rectangle.Top, Bottom = line.Rectangle.Top + line.Rectangle.Height };
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

            return lines.ToList().OrderBy(x => x.Key.Top);
        }
    }
}