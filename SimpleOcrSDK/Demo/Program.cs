using System;
using System.IO;
using System.Threading.Tasks;
using OcrMetadata;
using OcrMetadata.Model;
using PdfOcrSDK;
using SimpleGoogleOcrSDK;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("###### OCR Wrapper demo project ######");
            GoogleOcrEngine ocr = null; 

            while (true)
            {
                Console.WriteLine("Input file path:");
                var userChoice = Console.ReadLine();
                if (userChoice.ToLower().Equals("q"))
                {
                    return;
                }

                var imageFormat = GetImageFormat(userChoice);
                if (imageFormat == FileFormatEnum.Unsupported)
                {
                    Console.WriteLine("Unsupported file format. Supported types are; jpg, jpeg, png and pdf.");
                    continue;
                }

                if (imageFormat == FileFormatEnum.Pdf)
                {
                    PerformAction(DoPdfExtraction(userChoice)).ContinueWith(t => Pause()).Wait();
                }
                else
                {
                    if (ocr == null)
                    {
                        ocr = GetOcrEngine();
                    }
                    PerformAction(DoGoogleOcr(ocr, imageFormat, userChoice)).ContinueWith(t => Pause()).Wait();
                }
            }
        }

        private static GoogleOcrEngine GetOcrEngine()
        {
            Console.WriteLine("To OCR process an image you need a google account and access to their vision API.");
            Console.WriteLine("Input google API key:");
            string apiKey = null;
            while (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Console.ReadLine();
            }

            var ocr = GoogleOcrEngine.Build(new GoogleOcrConfigurations(apiKey, "OcrWrapperDemo"));
            Console.WriteLine("Ocr engine instantiated.");
            Console.WriteLine();
            return ocr;
        }

        private static async Task<OcrResult> DoGoogleOcr(GoogleOcrEngine googleOcrEngine, FileFormatEnum fileFormat, string file)
        {
            return await googleOcrEngine.OcrImage(file, fileFormat);
        }

        private static async Task<OcrResult> DoPdfExtraction(string file)
        {
            return PdfOcrEngine.Build().OcrPdf(file);
        }

        private static async Task PerformAction(Task<OcrResult> action)
        {
            Console.WriteLine();
            Console.WriteLine("Processing file...");
            var result = await action;

            if (result.HasError)
            {
                if (result.Error is FileNotFoundException)
                    Console.WriteLine("File was not found please correct path and try again...");
                else
                {
                    Console.WriteLine("Something went wrong while processing image: ");
                    Console.WriteLine(result.Error);
                }
                Console.ReadLine();
            }
            else if (result.TextFound)
            {
                Console.WriteLine("Following text was found: ");
                Console.WriteLine(result.Content.GetPlainTextWithLineBreaks());
            }
            else
            {
                Console.WriteLine("No text found on image");
            }
        }

        private static FileFormatEnum GetImageFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                return FileFormatEnum.Unsupported;

            switch (extension.ToLower())
            {
                case @".jpg":
                case @".jpeg":
                    return FileFormatEnum.Jpeg;

                case @".png":
                    return FileFormatEnum.Png;

                case @".pdf":
                    return FileFormatEnum.Pdf;

                default:
                    return FileFormatEnum.Unsupported;
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
