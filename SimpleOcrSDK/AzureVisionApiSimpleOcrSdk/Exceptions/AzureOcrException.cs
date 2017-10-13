using Microsoft.ProjectOxford.Vision;
using OcrMetadata.Exceptions;

namespace AzureVisionApiSimpleOcrSdk.Exceptions
{
    public class AzureOcrException : OcrException
    {
        public ClientException ClientException { get; set; }

        public AzureOcrException(ClientException e) : base("AzureOcrException occured")
        {
            ClientException = e;
        }

        public override string ToString()
        {
            return $"AzureOcrException. Azure could not process image. Error: '{ClientException.Error.Code}' '{ClientException.Error.Message}' RequestId: '{ClientException.Error.RequestId}'" + base.ToString();
        }

        public override string StackTrace => ClientException.StackTrace;
    }
}