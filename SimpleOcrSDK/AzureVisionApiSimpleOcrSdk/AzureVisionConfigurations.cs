namespace AzureVisionApiSimpleOcrSdk
{
    public interface IAzureVisionConfigurations
    {
        string SubscriptionKey { get; }
    }

    public class AzureVisionConfigurations : IAzureVisionConfigurations
    {
        public AzureVisionConfigurations(string azureVisionApiSubscriptionKey)
        {
            SubscriptionKey = azureVisionApiSubscriptionKey;
        }

        public string SubscriptionKey { get; }
    }
}