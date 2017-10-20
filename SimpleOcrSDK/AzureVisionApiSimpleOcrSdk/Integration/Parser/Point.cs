namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public struct Point
    {
        public int Top { get; set; }
        public int Bottom { get; set; }

        public bool IsWithinThisPoint(Point point)
        {
            return point.Top <= Top && point.Bottom >= Bottom;
        }
    }
}