using System.Linq;
using Microsoft.ProjectOxford.Vision.Contract;
using OcrMetadata.Model;
using Word = Microsoft.ProjectOxford.Vision.Contract.Word;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public interface ITransformAzureLineIntoSentence
    {
        Sentence Execute(Line line, int lineCount, int imgWidth, int imgHeight, int index);
    }

    public class TransformAzureLineIntoSentence : ITransformAzureLineIntoSentence
    {
        private readonly IAzureCreateRelativeCoordinate _createRelativeCoordinate;

        public TransformAzureLineIntoSentence(IAzureCreateRelativeCoordinate createRelativeCoordinate)
        {
            _createRelativeCoordinate = createRelativeCoordinate;
        }

        public Sentence Execute(Line line, int lineCount, int imgWidth, int imgHeight, int index)
        {
            if (line?.Words == null || !line.Words.Any()) return null;

            var words = line.Words.Select(word => CreateWord(word, imgWidth, imgHeight)).ToList();
            var text = words.Select(x => x.Value).Aggregate((i, j) => i + " " + j).Trim();

            return new Sentence(words, _createRelativeCoordinate.Execute(line.Rectangle, imgWidth, imgHeight), text, lineCount, index);
        }
        private OcrMetadata.Model.Word CreateWord(Word word, int imgWidth, int imgHeight)
        {
            var coord = _createRelativeCoordinate.Execute(word.Rectangle, imgWidth, imgHeight);
            return new OcrMetadata.Model.Word(coord, word.Text);
        }
    }
}