using System.IO;
using System.Threading.Tasks;
using AzureVisionApiSimpleOcrSdk.Exceptions;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace AzureVisionApiSimpleOcrSdk.Integration
{
    public class AzureOcrApi
    {
        private readonly VisionServiceClient _client;

        public AzureOcrApi(IAzureVisionConfigurations azureVisionConfigurations)
        {
            _client = new VisionServiceClient(azureVisionConfigurations.SubscriptionKey);
        }

        public virtual async Task<OcrResults> Execute(Stream imageStream, string language = "unk")
        {
            try
            {
                return await _client.RecognizeTextAsync(imageStream, language);
            }
            catch (ClientException e)
            {
                throw new AzureOcrException(e);
            }
        }
    }
}