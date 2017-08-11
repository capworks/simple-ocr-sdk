using System.Collections.Generic;
using System.Linq;
using Google.Apis.Vision.v1.Data;

namespace SimpleGoogleOcrSDK.Model
{
    public class RawGoogleOcrResult
    {
        public IList<EntityAnnotation> EntityAnnotations { get; set; }

        public static RawGoogleOcrResult CreateFrom(IList<EntityAnnotation> ocrResults)
        {
            return new RawGoogleOcrResult()
            {
                EntityAnnotations = ocrResults,
            };
        }

        public string GetAsPlainText()
        {
            return EntityAnnotations?.Skip(1).SelectMany(x => x.Description).Aggregate("", (i, j) => i + " " + j).Trim() ?? "";
        }

        public bool TextFound()
        {
            return EntityAnnotations != null && EntityAnnotations.Any();
        }
    }
}
