using System.Linq;
using Microsoft.ProjectOxford.Vision.Contract;
using OcrMetadata.Model;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public interface ITransformAzureLineIntoSentence
    {
        ISentence Execute(Line line, int lineCount, int imgWidth, int imgHeight, int index);
    }

    public class TransformAzureLineIntoSentence : ITransformAzureLineIntoSentence
    {
        private readonly IAzureCreateRelativeCoordinate _createRelativeCoordinate;
        private readonly ICreateWordFromAzureWord _createWordFromAzureWord;

        public TransformAzureLineIntoSentence(IAzureCreateRelativeCoordinate createRelativeCoordinate,
            ICreateWordFromAzureWord createWordFromAzureWord)
        {
            _createRelativeCoordinate = createRelativeCoordinate;
            _createWordFromAzureWord = createWordFromAzureWord;
        }

        public ISentence Execute(Line line, int lineCount, int imgWidth, int imgHeight, int index)
        {
            if (line?.Words == null || !line.Words.Any()) return null;

            var words = line.Words.Select(word => _createWordFromAzureWord.Execute(word, imgWidth, imgHeight)).ToList();
            var text = words.Select(x => x.Value).Aggregate((i, j) => i + " " + j).Trim();

            return new Sentence(words, _createRelativeCoordinate.Execute(line.Rectangle, imgWidth, imgHeight), text,
                lineCount, index);
        }
    }
}