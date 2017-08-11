using System.IO;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using OcrMetadata;
using OcrMetadata.PreProcessing;
using PreProcessing;
using SimpleGoogleOcrSDK.GoogleIntegration;
using SimpleGoogleOcrSDK.Model;

namespace SimpleGoogleOcrSDK
{
    public class GoogleOcrEngine
    {
        private readonly IGoogleOcrConfigurations _configurations;
        private readonly IOcrPreProcessing _ocrPreProcessing;
        private readonly GoogleOcrParser _googleOcrParser;

        private GoogleOcrEngine(IGoogleOcrConfigurations configurations, IOcrPreProcessing ocrPreProcessing, GoogleOcrParser googleOcrParser)
        {
            _configurations = configurations;
            _ocrPreProcessing = ocrPreProcessing;
            _googleOcrParser = googleOcrParser;
        }

        public static GoogleOcrEngine Build(IGoogleOcrConfigurations configurations)
        {
            return new GoogleOcrEngine(configurations, OcrPreProcessing.Build(), new GoogleOcrParser());
        }

        /// <summary>
        /// Async method to load the image, call google's vision API and format the result into an easy to use format.
        /// Prior to calling the API, this method will compress the image if to large for the API service, and will rotate it according to the EXIF orientation.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageFormatEnum">The image format. Needed for compression.</param>
        /// <returns></returns>
        public async Task<GoogleOcrResult> OcrImage(string filePath, ImageFormatEnum imageFormatEnum)
        {
            if(!File.Exists(filePath)) throw new FileNotFoundException("", filePath);
            using (var stream = File.OpenRead(filePath))
            {
                return await OcrImage(stream, imageFormatEnum);
            }
        }

        /// <summary>
        /// Async method to call google's vision API and format the result into an easy to use format.
        /// Prior to calling the API, this method will compress the image if to large for the API service, and will rotate it according to the EXIF orientation.
        /// </summary>
        /// <param name="imageAsStream">Image stream</param>
        /// <param name="imageFormatEnum">The image format. Needed for compression.</param>
        /// <returns></returns>
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