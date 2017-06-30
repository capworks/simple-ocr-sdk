using System;
using System.IO;
using System.Threading.Tasks;
using OcrMetadata;
using SimpleOcrSDK;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("###### Google OCR Wrapper demo project ######");
            var ocr = GetOcrEngine();

            while (true)
            {
                Console.WriteLine("Input file path:");
                var userChoice = Console.ReadLine();
                if (userChoice.ToLower().Equals("q"))
                {
                    return;
                }

                PerformAction(ocr, userChoice).ContinueWith(t => Pause()).Wait();
            }
        }

        private static OcrEngine GetOcrEngine()
        {
            Console.WriteLine("Input google API key:");
            string apiKey = null;
            while (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Console.ReadLine();
            }

            var ocr = OcrEngine.Build(new GoogleOcrConfigurations(apiKey, "OcrWrapperDemo"));
            Console.WriteLine("Ocr engine instantiated.");
            Console.WriteLine();
            return ocr;
        }

        private static async Task PerformAction(OcrEngine ocr, string file)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("Processing file...");
                var imageContent = await ocr.OcrImage(file, GetImageFormat(file));

                if (imageContent.TextFound())
                {
                    Console.WriteLine("Following was returned from google vision api: ");
                    Console.WriteLine(imageContent.FormattedResult.GetPlainTextWithLineBreaks());
                }
                else
                {
                    Console.WriteLine("No text found on image");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File was not found please correct path and try again...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong while processing image: ");
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }

        private static ImageFormatEnum GetImageFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException($"Unable to determine file extension for fileName: {fileName}");

            switch (extension.ToLower())
            {
                case @".jpg":
                case @".jpeg":
                    return ImageFormatEnum.Jpeg;

                case @".png":
                    return ImageFormatEnum.Png;

                default:
                    throw new ArgumentOutOfRangeException($"Unsuported image format for file: {fileName}");
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
