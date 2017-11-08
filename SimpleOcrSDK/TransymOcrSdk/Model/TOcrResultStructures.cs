using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TransymOcrSdk.Integration
{
    public class TOcrResultStructures
    {
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct TOcrResultsHeader
        {
            public int StructId;
            public int XPixelsPerInch;
            public int YPixelsPerInch;
            public int NumItems;
            public float MeanConfidence;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct TOcrResultsItem
        {
            public short StructId;
            public short OCRCha;
            public float Confidence;
            public short XPos;
            public short YPos;
            public short XDim;
            public short YDim;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct TOcrResults
        {
            public TOcrResultsHeader Hdr;
            public TOcrResultsItem[] Item;

            [DefaultValue(0)]
            public decimal Height;

            [DefaultValue(0)]
            public decimal Width;
        }
    }
}