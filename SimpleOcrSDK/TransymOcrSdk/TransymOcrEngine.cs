using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OcrMetadata.Exceptions;
using OcrMetadata.Tools;
using PreProcessing;
using TransymOcrSdk.Integration;
using TransymOcrSdk.Integration.Parser;
using TransymOcrSdk.Model;

namespace TransymOcrSdk
{
    public class TransymOcrEngine
    {
        private readonly TransymAccess _transymAccess;
        private readonly IImagePreProcessingUtils _ocrPreProcessing;
        private readonly TransymParser _parser;

        private TransymOcrEngine(TransymAccess transymAccess, IImagePreProcessingUtils ocrPreProcessing, TransymParser parser)
        {
            _transymAccess = transymAccess;
            _ocrPreProcessing = ocrPreProcessing;
            _parser = parser;
        }
        public static TransymOcrEngine Build(string logFile = null)
        {
            var ocrLogFile = string.IsNullOrWhiteSpace(logFile)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Transym Ocr Logs", "TOCR_Log.txt")
                : logFile;
            var createCoords = new TransymCreateRelativeCoordinate(new CreateRelativeCoordinate());
            return new TransymOcrEngine(new TransymAccess(ocrLogFile), new ImagePreProcessingUtils(), new TransymParser(createCoords));
        }

        /// <summary>
        /// Method to call Transym installation on the machine to perform OCR and format the result into an easy to use format.
        /// Prior to calling the API, this method will rotate it according to the EXIF orientation.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns></returns>
        public TransymOcrResult OcrImage(string filePath)
        {
            var start = DateTime.Now;
            try
            {
                if (!File.Exists(filePath)) throw new FileNotFoundException("", filePath);
                using (var stream = File.OpenRead(filePath))
                {
                    return DoOcr(stream, start);
                }
            }
            catch (Exception e)
            {
                return TransymOcrResult.CreateErrorResult(DateTime.Now.Subtract(start), e);
            }
        }

        /// <summary>
        /// Method to call Transym installation on the machine to perform OCR and format the result into an easy to use format.
        /// Prior to calling the API, this method will rotate it according to the EXIF orientation.
        /// </summary>
        /// <param name="imageAsStream">Image stream</param>
        /// <returns></returns>
        public TransymOcrResult OcrImage(Stream imageAsStream)
        {
            return DoOcr(imageAsStream, DateTime.Now);
        }

        private TransymOcrResult DoOcr(Stream imageAsStream, DateTime start)
        {
            try
            {
                var src = new Bitmap(imageAsStream);
                ValidateImageProportions(src);
                src = PreProcess(src);
                var byteStream = new MemoryStream();
                src.Save(byteStream, ImageFormat.Tiff);
                var entries = _transymAccess.OcrByStream(byteStream);
                var rawTransymResult = RawTransymOcrResult.CreateFrom(entries);
                var content = _parser.Execute(entries);
                return TransymOcrResult.CreateSuccesResult(DateTime.Now.Subtract(start), content, rawTransymResult);
            }
            catch (Exception e)
            {
                return TransymOcrResult.CreateErrorResult(DateTime.Now.Subtract(start), e);
            }
        }

        private Bitmap PreProcess(Bitmap src)
        {
            _ocrPreProcessing.AdjustImageOrientation(src);
            src = _ocrPreProcessing.RemoveTransparency(src);
            return src;
        }

        private static void ValidateImageProportions(Bitmap src)
        {
            var height = src.Height;
            var width = src.Width;
            if ((Math.Abs(src.HorizontalResolution) > float.Epsilon &&
                 (src.HorizontalResolution < 25 || src.HorizontalResolution > 2000)) ||
                (Math.Abs(src.VerticalResolution) > float.Epsilon &&
                 (src.VerticalResolution < 25 || src.VerticalResolution > 2000)))
            {
                throw new ImageProportionsToSmallException(
                    $"Transym ocr need the images HorizontalResolution and VerticalResolution "+
                    "to be between 25 and 2000. "+
                    "VerticalResolution: '{Math.Abs(src.VerticalResolution)}' "+
                    "HorizontalResolution: '{Math.Abs(src.HorizontalResolution)}'");
            }
            if (height < 25 || width < 25)
            {
                throw new ImageProportionsToSmallException(GetImageProportionErrorMsg(height, width));
            }
            if (height > 12000 || width > 12000)
            {
                throw new ImageProportionsToLargeException(GetImageProportionErrorMsg(height, width));
            }
        }

        private static string GetImageProportionErrorMsg(int height, int width)
        {
            return $"Transym ocr need images to be maximum 12000x12000 pixels. Image was h: '{height}' w: '{width}'";
        }
    }
}