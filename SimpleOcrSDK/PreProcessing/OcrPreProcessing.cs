using System;
using System.Drawing;
using System.IO;
using OcrMetadata;
using OcrMetadata.PreProcessing;

namespace PreProcessing
{
    public class OcrPreProcessing : IOcrPreProcessing
    {
        private readonly IGetBitmapAsStream _getBitmapAsStream;
        private readonly ImagePreProcessingUtils _imagePreProcessingUtils;

        internal OcrPreProcessing(ImagePreProcessingUtils imagePreProcessingUtils, IGetBitmapAsStream getBitmapAsStream)
        {
            _getBitmapAsStream = getBitmapAsStream ?? throw new ArgumentNullException(nameof(getBitmapAsStream));
            _imagePreProcessingUtils = imagePreProcessingUtils ?? throw new ArgumentNullException(nameof(imagePreProcessingUtils));
        }

        public static OcrPreProcessing Build()
        {
            return new OcrPreProcessing(new ImagePreProcessingUtils(), new GetBitmapAsStream());
        }

        public IPreprocessedResult AjustOrientationAndSize(Stream imageFileStream, FileFormatEnum fileFormat)
        {
            var src = new Bitmap(imageFileStream);

            _imagePreProcessingUtils.AdjustImageOrientation(src);
            imageFileStream = _getBitmapAsStream.WithMaxLength(3900000, fileFormat, src);

            imageFileStream.Seek(0, SeekOrigin.Begin);
            return new PreprocessedResult
            {
                ImageFileStream = imageFileStream,
                NewImageHeight = src.Size.Height,
                NewImageWidth = src.Size.Width
            };
        }
    }
}