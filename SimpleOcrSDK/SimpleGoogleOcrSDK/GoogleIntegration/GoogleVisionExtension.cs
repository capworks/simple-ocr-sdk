using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;

namespace SimpleGoogleOcrSDK.GoogleIntegration
{
    internal static class GoogleVisionExtension
    {
        public static async Task<IList<EntityAnnotation>> RecognizeTextAsync(this VisionService service, Stream stream)
        {
            var result = await service.Images.Annotate(CreateRequest(stream)).ExecuteAsync();
            return GetTextResult(result);
        }

        public static IList<EntityAnnotation> RecognizeText(this VisionService service, Stream stream)
        {
            var result = service.Images.Annotate(CreateRequest(stream)).Execute();
            return GetTextResult(result);
        }

        private static IList<EntityAnnotation> GetTextResult(BatchAnnotateImagesResponse result)
        {
            return result?.Responses?.Count > 0 ? result.Responses[0]?.TextAnnotations : new List<EntityAnnotation>();
        }

        private static BatchAnnotateImagesRequest CreateRequest(Stream stream)
        {
            var request = new BatchAnnotateImagesRequest
            {
                Requests = new List<AnnotateImageRequest>
                {
                    CreateAnnotationImageRequest(stream)
                }
            };
            return request;
        }

        private static AnnotateImageRequest CreateAnnotationImageRequest(Stream stream)
        {
            return new AnnotateImageRequest
            {
                Image = new Image() { Content = Convert.ToBase64String(StreamToBytes(stream)) },
                Features = new List<Feature>() { new Feature() { Type = "TEXT_DETECTION" } },
            };
        }

        private static byte[] StreamToBytes(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}