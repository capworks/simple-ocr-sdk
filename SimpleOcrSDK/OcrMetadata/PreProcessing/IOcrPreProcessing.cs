using System.IO;

namespace OcrMetadata.PreProcessing
{
    public interface IOcrPreProcessing
    {
        IPreprocessedResult AjustOrientationAndSize(Stream imageFileStream, ImageFormatEnum imageFormat);
    }
}