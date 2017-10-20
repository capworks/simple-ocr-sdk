using System;
using System.IO;
using System.Threading.Tasks;
using AzureVisionApiSimpleOcrSdk.Integration;
using AzureVisionApiSimpleOcrSdk.Integration.Parser;
using AzureVisionApiSimpleOcrSdk.Model;
using OcrMetadata;
using OcrMetadata.Exceptions;
using OcrMetadata.PreProcessing;
using OcrMetadata.Tools;
using PreProcessing;

namespace AzureVisionApiSimpleOcrSdk
{
    public class AzureOcrEngine
    {
        private readonly AzureOcrApi _azureVisionApi;
        private readonly IOcrPreProcessing _ocrPreProcessing;
        private readonly AzureOcrParser _azureOcrParser;

        private AzureOcrEngine(AzureOcrApi azureVisionApi, IOcrPreProcessing ocrPreProcessing, AzureOcrParser azureOcrParser)
        {
            _azureVisionApi = azureVisionApi;
            _ocrPreProcessing = ocrPreProcessing;
            _azureOcrParser = azureOcrParser;
        }
        public static AzureOcrEngine Build(IAzureVisionConfigurations configurations)
        {
            return new AzureOcrEngine(new AzureOcrApi(configurations), OcrPreProcessing.Build(),
                new AzureOcrParser(
                    new TransformLinesIntoSentences(
                        new TransformAzureLineIntoSentence(
                            new AzureCreateRelativeCoordinate(new CreateRelativeCoordinate()))),
                    new SortIntoLogicalLines(), new GetLinesOrderedByTopPosition()));
        }

        /// <summary>
        /// Async method to load the image, call Azure's vision API and format the result into an easy to use format.
        /// Prior to calling the API, this method will compress the image if to large for the API service, and will rotate it according to the EXIF orientation.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="fileFormatEnum">The image format. Needed for compression.</param>
        /// <returns></returns>
        public async Task<AzureOcrResult> OcrImage(string filePath, FileFormatEnum fileFormatEnum)
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
                return AzureOcrResult.CreateErrorResult(DateTime.Now.Subtract(start), e);
            }
        }

        /// <summary>
        /// Async method to call Azure's vision API and format the result into an easy to use format.
        /// Prior to calling the API, this method will compress the image if to large for the API service, and will rotate it according to the EXIF orientation.
        /// </summary>
        /// <param name="imageAsStream">Image stream</param>
        /// <param name="fileFormatEnum">The image format. Needed for compression.</param>
        /// <returns></returns>
        public async Task<AzureOcrResult> OcrImage(Stream imageAsStream, FileFormatEnum fileFormatEnum)
        {
            return await DoOcr(imageAsStream, fileFormatEnum, DateTime.Now);
        }

        private async Task<AzureOcrResult> DoOcr(Stream imageAsStream, FileFormatEnum fileFormatEnum, DateTime start)
        {
            try
            {
                var preprocessedResult = _ocrPreProcessing.AjustOrientationAndSize(imageAsStream, fileFormatEnum);
                using (var stream = preprocessedResult.ImageFileStream)
                {
                        ValidateImageProportions(preprocessedResult.NewImageHeight, preprocessedResult.NewImageWidth);
                        var entries = await _azureVisionApi.Execute(stream);
                        var rawAzureOcrResult = RawAzureOcrResult.CreateFrom(entries);
                        var content = _azureOcrParser.Execute(rawAzureOcrResult, preprocessedResult.NewImageHeight,
                            preprocessedResult.NewImageWidth);
                        return AzureOcrResult.CreateSuccesResult(DateTime.Now.Subtract(start), content, rawAzureOcrResult);
                }
            }
            catch (Exception e)
            {
                return AzureOcrResult.CreateErrorResult(DateTime.Now.Subtract(start), e);
            }
        }

        private static void ValidateImageProportions(int height, int width)
        {
            if (height < 50 || width < 50)
            {
                throw new ImageProportionsToSmallException(GetImageProportionErrorMsg(height, width));
            }
            if (height >= 32000 || width >= 32000)
            {
                throw new ImageProportionsToLargeException(GetImageProportionErrorMsg(height, width));
            }
        }

        private static string GetImageProportionErrorMsg(int height, int width)
        {
            return $"Azure ocr need images between 40x40 and 32000x32000 pixels. Image was h: '{height}' w: '{width}'";
        }
    }
}