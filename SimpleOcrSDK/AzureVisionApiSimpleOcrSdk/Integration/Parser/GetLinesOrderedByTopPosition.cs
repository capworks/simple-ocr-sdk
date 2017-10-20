using System;
using System.Collections.Generic;
using System.Linq;
using AzureVisionApiSimpleOcrSdk.Model;
using Microsoft.ProjectOxford.Vision.Contract;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public interface IGetLinesOrderedByTopPosition
    {
        List<Line> Execute(RawAzureOcrResult ocrOutput);
    }

    public class GetLinesOrderedByTopPosition : IGetLinesOrderedByTopPosition
    {
        public List<Line> Execute(RawAzureOcrResult ocrOutput)
        {
            if (ocrOutput == null) throw new ArgumentNullException(nameof(ocrOutput));
            return ocrOutput.Regions?.ToList().SelectMany(x => x.Lines).OrderBy(x => x.Rectangle.Top).ToList();
        }
    }
}