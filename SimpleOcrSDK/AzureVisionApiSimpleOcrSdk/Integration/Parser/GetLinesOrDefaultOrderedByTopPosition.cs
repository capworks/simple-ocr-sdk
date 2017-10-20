using System.Collections.Generic;
using System.Linq;
using AzureVisionApiSimpleOcrSdk.Model;
using Microsoft.ProjectOxford.Vision.Contract;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public interface IGetLinesOrDefaultOrderedByTopPosition
    {
        List<Line> Execute(RawAzureOcrResult ocrOutput);
    }

    public class GetLinesOrDefaultOrderedByTopPosition : IGetLinesOrDefaultOrderedByTopPosition
    {
        public List<Line> Execute(RawAzureOcrResult ocrOutput)
        {
            return ocrOutput.Regions?.ToList().SelectMany(x => x.Lines).OrderBy(x => x.Rectangle.Top).ToList() ??
                   new List<Line>();
        }
    }
}