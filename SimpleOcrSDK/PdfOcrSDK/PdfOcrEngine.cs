using System;
using System.IO;
using PdfOcrSDK.Model;
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
        public PdfResult OcrPdf(string filePathToPdf)
        {
            var start = DateTime.Now;
            try
            {
                if (!File.Exists(filePathToPdf)) throw new FileNotFoundException("", filePathToPdf);
                return Execute(File.ReadAllBytes(filePathToPdf), start);
            }
            catch (Exception e)
            {
                return PdfResult.CreateErrorResult(DateTime.Now.Subtract(start), e);
            }
        }

        /// <summary>
        /// Method to extract the text from the pdf.
        /// </summary>s
        /// <param name="bytes">byte array of the pdf</param>
        /// <returns></returns>
        public PdfResult OcrPdf(byte[] bytes)
        {
            return Execute(bytes, DateTime.Now);
        }

        private PdfResult Execute(byte[] bytes, DateTime start)
        {
            try
            {
                var result = _extractPdf.Execute(bytes);

                var content = _parser.Execute(result);
                return PdfResult.CreateSuccesResult(DateTime.Now.Subtract(start), content);
            }
            catch (Exception e)
            {
                return PdfResult.CreateErrorResult(DateTime.Now.Subtract(start), e);
            }
        }
    }
}
