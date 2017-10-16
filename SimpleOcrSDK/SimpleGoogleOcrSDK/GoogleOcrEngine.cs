using System;
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
        /// <param name="fileFormatEnum">The image format. Needed for compression.</param>
        /// <returns></returns>
        public async Task<GoogleOcrResult> OcrImage(string filePath, FileFormatEnum fileFormatEnum)
        {
            var start = DateTime.Now;
            try
            {
                if (!File.Exists(filePath)) throw new FileNotFoundException("", filePath);
                using (var stream = File.OpenRead(filePath))
                {
                    return await DoOcr(stream, fileFormatEnum, start);
                }
            }
            catch (Exception e)
            {
                return GoogleOcrResult.CreateErrorResult(DateTime.Now.Subtract(start), e);
            }
        }

        /// <summary>
        /// Async method to call google's vision API and format the result into an easy to use format.
        /// Prior to calling the API, this method will compress the image if to large for the API service, and will rotate it according to the EXIF orientation.
        /// </summary>
        /// <param name="imageAsStream">Image stream</param>
        /// <param name="fileFormatEnum">The image format. Needed for compression.</param>
        /// <returns></returns>
        public async Task<GoogleOcrResult> OcrImage(Stream imageAsStream, FileFormatEnum fileFormatEnum)
        {
            return await DoOcr(imageAsStream, fileFormatEnum, DateTime.Now);
        }

        private async Task<GoogleOcrResult> DoOcr(Stream imageAsStream, FileFormatEnum fileFormatEnum, DateTime start)
        {
            try
            {
                var preprocessedResult = _ocrPreProcessing.AjustOrientationAndSize(imageAsStream, fileFormatEnum);
                using (var stream = preprocessedResult.ImageFileStream)
                {
                    using (var service = VisionService())
                    {
                        var entries = await service.RecognizeTextAsync(stream);
                        var rawGoogleOcrResult = RawGoogleOcrResult.CreateFrom(entries);
                        var content = _googleOcrParser.Execute(rawGoogleOcrResult, preprocessedResult.NewImageHeight,
                            preprocessedResult.NewImageWidth);
                        return GoogleOcrResult.CreateSuccessResult(DateTime.Now.Subtract(start), content, rawGoogleOcrResult);
                    }
                }
            }
            catch (Exception e)
            {
                return GoogleOcrResult.CreateErrorResult(DateTime.Now.Subtract(start), e);
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