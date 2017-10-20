namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public struct Point
    {
        public int Top { get; set; }
        public int Bottom { get; set; }

        public bool IsWithinThisPoint(Point point)
        {
            return Top <= point.Top && Bottom >= point.Bottom;
        }
    }
}