using System.IO;
using OcrMetadata.Model;
using PdfOcrSDK.PdfBoxIntegration;

namespace PdfOcrSDK
{
    public class PdfOcrEngine
    {
        private readonly PdfParser _parser;
        private readonly ExtractPdf _extractPdf;

        private PdfOcrEngine(PdfParser parser, ExtractPdf extractPdf)
        {
            _parser = parser;
            _extractPdf = extractPdf;
        }

        public static PdfOcrEngine Build()
        {
            return new PdfOcrEngine(new PdfParser(), new ExtractPdf());
        }

        /// <summary>
        /// Method to load the pdf, and extract the text from it.
        /// </summary>
        /// <param name="filePathToPdf">File path</param>
        /// <returns></returns>
        public ImageContent OcrPdf(string filePathToPdf)
        {
            if (!File.Exists(filePathToPdf)) throw new FileNotFoundException("", filePathToPdf);
            return OcrPdf(File.ReadAllBytes(filePathToPdf));
        }

        /// <summary>
        /// Method to extract the text from the pdf.
        /// </summary>s
        /// <param name="bytes">byte array of the pdf</param>
        /// <returns></returns>
        public ImageContent OcrPdf(byte[] bytes)
        {
            return Execute(bytes);
        }

        private ImageContent Execute(byte[] bytes)
        {
            var result = _extractPdf.Execute(bytes);

            return _parser.Execute(result);
        }
    }
}
