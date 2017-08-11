using System.IO;
using OcrMetadata.PreProcessing;

namespace PreProcessing
{
    internal class PreprocessedResult : IPreprocessedResult
    {
        public int NewImageHeight { get; set; }
        public int NewImageWidth { get; set; }
        public Stream ImageFileStream { get; set; }
    }
}