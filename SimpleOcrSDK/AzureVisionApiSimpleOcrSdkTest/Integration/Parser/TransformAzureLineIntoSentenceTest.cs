using AzureVisionApiSimpleOcrSdk.Integration.Parser;
using FluentAssertions;
using Microsoft.ProjectOxford.Vision.Contract;
using Moq;
using NUnit.Framework;
using OcrMetadata.Model;
using Word = Microsoft.ProjectOxford.Vision.Contract.Word;

namespace AzureVisionApiSimpleOcrSdkTest.Integration.Parser
{
    [TestFixture]
    public class TransformAzureLineIntoSentenceTest
    {
        private Mock<IAzureCreateRelativeCoordinate> _relativeCoords;
        private Mock<ICreateWordFromAzureWord> _createWord;
        private TransformAzureLineIntoSentence _target;
        private Line _line;
        private IWord _metaWord1;
        private IWord _metaWord2;
        private IWord _metaWord3;
        private Word _word3;
        private Word _word2;
        private Word _word1;
        private int _imgHeight;
        private int _imgWidth;

        [SetUp]
        public void Setup()
        {
            _relativeCoords = new Mock<IAzureCreateRelativeCoordinate>();
            _createWord = new Mock<ICreateWordFromAzureWord>();

            _imgWidth = 100;
            _imgHeight = 200;
            _word1 = new Word();
            _word2 = new Word();
            _word3 = new Word();
            _line = new Line { Words = new[] { _word1, _word2, _word3 } };

            var metaWord1Mock = new Mock<IWord>();
            var metaWord2Mock = new Mock<IWord>();
            var metaWord3Mock = new Mock<IWord>();

            _metaWord1 = metaWord1Mock.Object;
            _metaWord2 = metaWord2Mock.Object;
            _metaWord3 = metaWord3Mock.Object;

            metaWord1Mock.Setup(x => x.Value).Returns("This is");
            metaWord2Mock.Setup(x => x.Value).Returns("a");
            metaWord3Mock.Setup(x => x.Value).Returns("very important test");

            _createWord.Setup(x => x.Execute(_word1, _imgWidth, _imgHeight)).Returns(metaWord1Mock.Object);
            _createWord.Setup(x => x.Execute(_word2, _imgWidth, _imgHeight)).Returns(metaWord2Mock.Object);
            _createWord.Setup(x => x.Execute(_word3, _imgWidth, _imgHeight)).Returns(metaWord3Mock.Object);

            _target = new TransformAzureLineIntoSentence(_relativeCoords.Object, _createWord.Object);
        }

        [Test]
        public void GivenLineNull_WhenInvokingExecute_ThenNullIsReturned()
        {
            //Arrange
            Line line = null;

            //Act
            var result = _target.Execute(line, 0, 0, 0, 0);

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public void GivenLineWithWordsNull_WhenInvokingExecute_ThenNullIsReturned()
        {
            //Arrange
            var line = new Line();
            Assert.AreEqual(line.Words, null);

            //Act
            var result = _target.Execute(line, 0, 0, 0, 0);

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public void GivenLineWithWordsIsEmpty_WhenInvokingExecute_ThenNullIsReturned()
        {
            //Arrange
            var line = new Line {Words = new Word[] { }};

            //Act
            var result = _target.Execute(line, 0, 0, 0, 0);

            //Assert
            result.Should().BeNull();
        }

        [Test]
        public void GivenLineWithWords_WhenInvokingExecute_ThenCreateWordIsCalledForeachAndTheOutoutIsReturnedInTheSentence()
        {
            //Arrange

            //Act
            var result = _target.Execute(_line, 0, _imgWidth, _imgHeight, 0);

            //Assert
            _createWord.Verify(x => x.Execute(_word1, _imgWidth, _imgHeight), Times.Once);
            _createWord.Verify(x => x.Execute(_word2, _imgWidth, _imgHeight), Times.Once);
            _createWord.Verify(x => x.Execute(_word3, _imgWidth, _imgHeight), Times.Once);
            result.Words.Contains(_metaWord1).Should().BeTrue();
            result.Words.Contains(_metaWord2).Should().BeTrue();
            result.Words.Contains(_metaWord3).Should().BeTrue();
        }

        [Test]
        public void GivenLineWithWords_WhenInvokingExecute_ThenSentenceValueIsConcatOfWordValues()
        {
            //Arrange
            var imgWidth = 100;
            var imgHeight = 200;
            var word1 = new Word();
            var word2 = new Word();
            var word3 = new Word();
            var line = new Line { Words = new[] { word1, word2, word3 } };

            var metaWord1 = new Mock<IWord>();
            var metaWord2 = new Mock<IWord>();
            var metaWord3 = new Mock<IWord>();

            metaWord1.Setup(x => x.Value).Returns("This is");
            metaWord2.Setup(x => x.Value).Returns("a");
            metaWord3.Setup(x => x.Value).Returns("very important test");

            _createWord.Setup(x => x.Execute(word1, imgWidth, imgHeight)).Returns(metaWord1.Object);
            _createWord.Setup(x => x.Execute(word2, imgWidth, imgHeight)).Returns(metaWord2.Object);
            _createWord.Setup(x => x.Execute(word3, imgWidth, imgHeight)).Returns(metaWord3.Object);

            //Act
            var result = _target.Execute(line, 0, imgWidth, imgHeight, 0);

            //Assert
            result.Value = "This is a very important test";
        }

        [Test]
        public void GivenLine_WhenInvokingExecute_ThenSentenceWithIndexIsReturned()
        {
            //Arrange
            var index = 240;

            //Act
            var result = _target.Execute(_line, 0, _imgWidth, _imgHeight, index);

            //Assert
            result.Index.Should().Be(index);
        }

        [Test]
        public void GivenLine_WhenInvokingExecute_ThenSentenceWithLineAndCountIsReturned()
        {
            //Arrange
            var lineCount = 42;

            //Act
            var result = _target.Execute(_line, lineCount, _imgWidth, _imgHeight, 0);

            //Assert
            result.Line.Should().Be(lineCount);
        }

        [Test]
        public void GivenLine_WhenInvokingExecute_ThenSentenceWithRelativeCoordinatesIsReturned()
        {
            //Arrange
            var left = 10;
            var top = 20;
            var width = 40;
            var height = 15;

            var returnCoord = new Coordinate(0, 0, 0, 0);
            _line.BoundingBox = $"{left},{top},{width},{height}";
            _relativeCoords.Setup(z => z.Execute(
                    It.Is<Rectangle>(x => x.Height == height && x.Width == width && x.Top == top && x.Left == left),
                    _imgWidth, _imgHeight)).Returns(returnCoord);
            //Act
            var result = _target.Execute(_line, 0, _imgWidth, _imgHeight, 0);

            //Assert
            result.Coords.Should().Be(returnCoord);
        }
    }
}