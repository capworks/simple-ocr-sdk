namespace SimpleGoogleOcrSDK
{
    public interface IGoogleOcrConfigurations
    {
        string GoogleVisionKey { get; }
        string ApplicationName { get; }
    }

    public class GoogleOcrConfigurations : IGoogleOcrConfigurations
    {
        public GoogleOcrConfigurations(string googleVisionKey, string applicationName)
        {
            GoogleVisionKey = googleVisionKey;
            ApplicationName = applicationName;
        }

        public string GoogleVisionKey { get; }
        public string ApplicationName { get; }
    }
}