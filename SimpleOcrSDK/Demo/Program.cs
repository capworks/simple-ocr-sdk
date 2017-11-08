using System;
using System.IO;
using System.Threading.Tasks;
using AzureVisionApiSimpleOcrSdk;
using OcrMetadata;
using OcrMetadata.Model;
using PdfOcrSDK;
using SimpleGoogleOcrSDK;
using TransymOcrSdk;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("###### OCR Wrapper demo project ######");
            GoogleOcrEngine googleOcrEngine = null;
            AzureOcrEngine azureOcrEngine = null;
            string visionApiChoice = null;

            while (true)
            {
                Console.WriteLine("Input file path:");
                var userChoice = Console.ReadLine();
                if (userChoice == null || userChoice.ToLower().Equals("q"))
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

                    if (visionApiChoice == null)
                    {
                        Console.WriteLine("What vision API do you wish to use, Google or Azure? \nOr you can use an installation of Tranym ocr if you have it installed on this machine. \n(Options: azure, google, transym).");
                        while (string.IsNullOrWhiteSpace(visionApiChoice) ||
                               (visionApiChoice != "google" && visionApiChoice != "azure" && visionApiChoice != "transym"))
                        {
                            visionApiChoice = Console.ReadLine()?.ToLower();
                        }
                    }

                    if (visionApiChoice == "google")
                    {
                        if (googleOcrEngine == null)
                        {
                            googleOcrEngine = GetGoogleOcrEngine();
                        }
                        PerformAction(DoGoogleOcr(googleOcrEngine, imageFormat, userChoice)).ContinueWith(t => Pause()).Wait();
                    }
                    else if(visionApiChoice == "azure")
                    {
                        if (azureOcrEngine == null)
                        {
                            azureOcrEngine = GetAzureOcrEngine();
                        }
                        PerformAction(DoAzureOcr(azureOcrEngine, imageFormat, userChoice)).ContinueWith(t => Pause()).Wait();
                    }
                    else if (visionApiChoice == "transym")
                    {
                        Console.WriteLine("Do you have a licensed Transym installation on this machine? (y/n)\nOtherwise the ocr will fail.");
                        var isTransymInstalled = Console.ReadLine()?.ToLower();
                        if (isTransymInstalled == "y")
                        {
                            PerformAction(DoTransymOcr(userChoice)).ContinueWith(t => Pause()).Wait();
                        }
                    }
                }
            }
        }

        private static GoogleOcrEngine GetGoogleOcrEngine()
        {
            Console.WriteLine("To OCR process an image you need a google account and access to their vision API.");
            Console.WriteLine("Input google API key:");
            string apiKey = null;
            while (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Console.ReadLine();
            }

            var ocr = GoogleOcrEngine.Build(new GoogleOcrConfigurations(apiKey, "OcrWrapperDemo"));
            Console.WriteLine("Google ocr engine instantiated.");
            Console.WriteLine();
            return ocr;
        }

        private static AzureOcrEngine GetAzureOcrEngine()
        {
            Console.WriteLine("To OCR process an image with Azure Vision API you need a azure account and access to their vision API.");
            Console.WriteLine("Input your Azure Vision API subscription key:");
            string subscriptionKey = null;
            while (string.IsNullOrWhiteSpace(subscriptionKey))
            {
                subscriptionKey = Console.ReadLine();
            }

            var ocr = AzureOcrEngine.Build(new AzureVisionConfigurations(subscriptionKey));
            Console.WriteLine("Azure ocr engine instantiated.");
            Console.WriteLine();
            return ocr;
        }

        private static async Task<OcrResult> DoGoogleOcr(GoogleOcrEngine googleOcrEngine, FileFormatEnum fileFormat, string file)
        {
            return await googleOcrEngine.OcrImage(file, fileFormat);
        }

        private static async Task<OcrResult> DoAzureOcr(AzureOcrEngine azureOcrEngine, FileFormatEnum fileFormat, string file)
        {
            return await azureOcrEngine.OcrImage(file, fileFormat);
        }

        private static async Task<OcrResult> DoPdfExtraction(string file)
        {
            return PdfOcrEngine.Build().OcrPdf(file);
        }

        private static async Task<OcrResult> DoTransymOcr(string file)
        {
            return TransymOcrEngine.Build().OcrImage(file);
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
