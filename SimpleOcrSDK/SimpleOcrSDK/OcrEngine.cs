using System.IO;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using OcrMetadata;
using OcrMetadata.PreProcessing;
using PreProcessing;
using SimpleOcrSDK.GoogleIntegration;
using SimpleOcrSDK.Model;

namespace SimpleOcrSDK
{
    public class OcrEngine
    {
        private readonly IGoogleOcrConfigurations _configurations;
        private readonly IOcrPreProcessing _ocrPreProcessing;
        private readonly GoogleOcrParser _googleOcrParser;

        private OcrEngine(IGoogleOcrConfigurations configurations, IOcrPreProcessing ocrPreProcessing, GoogleOcrParser googleOcrParser)
        {
            _configurations = configurations;
            _ocrPreProcessing = ocrPreProcessing;
            _googleOcrParser = googleOcrParser;
        }

        public static OcrEngine Build(IGoogleOcrConfigurations configurations)
        {
            return new OcrEngine(configurations, OcrPreProcessing.Build(), new GoogleOcrParser());
        }

        public async Task<GoogleOcrResult> OcrImage(string filePath, ImageFormatEnum imageFormatEnum)
        {
            if(!File.Exists(filePath)) throw new FileNotFoundException("", filePath);
            using (var stream = File.OpenRead(filePath))
            {
                return await OcrImage(stream, imageFormatEnum);
            }
        }

        public async Task<GoogleOcrResult> OcrImage(Stream imageAsStream, ImageFormatEnum imageFormatEnum)
        {
            var preprocessedResult = _ocrPreProcessing.AjustOrientationAndSize(imageAsStream, imageFormatEnum);
            using (var stream = preprocessedResult.ImageFileStream)
            {
                using (var service = VisionService())
                {
                    var entries = await service.RecognizeTextAsync(stream);
                    var ocrResult = RawGoogleOcrResult.CreateFrom(entries);
                    return new GoogleOcrResult(_googleOcrParser.Execute(ocrResult, preprocessedResult.NewImageHeight, preprocessedResult.NewImageWidth), ocrResult);
                }
            }
        }

        private VisionService VisionService()
        {
            return new VisionService(new BaseClientService.Initializer
            {
                ApplicationName = _configurations.ApplicationName,
                ApiKey = _configurations.GoogleVisionKey,
            });
        }
    }
}