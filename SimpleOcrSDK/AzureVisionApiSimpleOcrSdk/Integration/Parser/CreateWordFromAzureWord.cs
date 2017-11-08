using System;
using OcrMetadata.Model;
using Word = Microsoft.ProjectOxford.Vision.Contract.Word;

namespace AzureVisionApiSimpleOcrSdk.Integration.Parser
{
    public interface ICreateWordFromAzureWord
    {
        IWord Execute(Word word, int imgWidth, int imgHeight);
    }

    public class CreateWordFromAzureWord : ICreateWordFromAzureWord
    {
        private readonly IAzureCreateRelativeCoordinate _createRelativeCoordinate;

        public CreateWordFromAzureWord(IAzureCreateRelativeCoordinate createRelativeCoordinate)
        {
            _createRelativeCoordinate = createRelativeCoordinate;
        }

        public IWord Execute(Word word, int imgWidth, int imgHeight)
        {
            if (word == null) throw new ArgumentNullException(nameof(word));

            var coord = _createRelativeCoordinate.Execute(word.Rectangle, imgWidth, imgHeight);
            return new OcrMetadata.Model.Word(coord, word.Text);
        }
    }
}