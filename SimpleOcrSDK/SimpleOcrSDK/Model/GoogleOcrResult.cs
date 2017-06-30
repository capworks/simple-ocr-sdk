using OcrMetadata;
using OcrMetadata.Model;

namespace SimpleOcrSDK.Model
{
    public class GoogleOcrResult
    {
        public GoogleOcrResult(ImageContent imageContent, RawGoogleOcrResult rawGoogleOcrResult)
        {
            FormattedResult = imageContent;
            RawResult = rawGoogleOcrResult;
        }

        public ImageContent FormattedResult { get; }
        public RawGoogleOcrResult RawResult { get; }

        public bool TextFound()
        {
            return RawResult.TextFound();
        }
    }
}