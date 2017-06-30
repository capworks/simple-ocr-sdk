using System.IO;

namespace OcrMetadata.PreProcessing
{
    public interface IPreprocessedResult
    {
        int NewImageHeight { get; }
        int NewImageWidth { get; }
        Stream ImageFileStream { get; }
    }
}