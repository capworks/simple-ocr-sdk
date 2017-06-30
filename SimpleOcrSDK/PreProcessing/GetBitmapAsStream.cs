using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OcrMetadata;

namespace PreProcessing
{
    internal interface IGetBitmapAsStream
    {
        Stream WithMaxLength(int maxsize, ImageFormatEnum format, Bitmap src);
    }

    internal class GetBitmapAsStream : IGetBitmapAsStream
    {
        public Stream WithMaxLength(int maxsize, ImageFormatEnum imageFormat, Bitmap src)
        {
            var format = GetImageFormat(imageFormat);

            Stream stream = new MemoryStream();
            src.Save(stream, format);

            var compression = 0;
            while (stream.Length > maxsize && compression < 100)
            {
                compression += 10;
                stream.Dispose();
                stream = SaveToStream(src, format, compression);
            }
            return stream;
        }

        private static Stream SaveToStream(Bitmap src, ImageFormat format, long quality = 50)
        {
            var stream = new MemoryStream();
            using (var encoderParameters = new EncoderParameters(1))
            using (var encoderParameter = new EncoderParameter(Encoder.Quality, quality))
            {
                var codecInfo = ImageCodecInfo.GetImageDecoders().First(codec => codec.FormatID == format.Guid);
                encoderParameters.Param[0] = encoderParameter;
                src.Save(stream, codecInfo, encoderParameters);
            }

            return stream;
        }

        private static ImageFormat GetImageFormat(ImageFormatEnum format)
        {
            switch (format)
            {
                case ImageFormatEnum.Jpeg:
                    return ImageFormat.Jpeg;

                case ImageFormatEnum.Png:
                    return ImageFormat.Png;

                default:
                    throw new ArgumentOutOfRangeException($"Unsuported image format: {format}");
            }
        }
    }
}