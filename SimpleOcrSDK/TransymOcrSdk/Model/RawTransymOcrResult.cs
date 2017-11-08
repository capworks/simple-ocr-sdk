using TransymOcrSdk.Integration;

namespace TransymOcrSdk.Model
{
    public class RawTransymOcrResult 
    {
        public TOcrResultStructures.TOcrResults OcrResults { get; set; }

        public static RawTransymOcrResult CreateFrom(TOcrResultStructures.TOcrResults ocrResults)
        {
            return new RawTransymOcrResult()
            {
               OcrResults = ocrResults
            };
        }
    }
}