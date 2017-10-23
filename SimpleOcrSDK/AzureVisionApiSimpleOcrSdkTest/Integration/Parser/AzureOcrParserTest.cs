using System;
using System.Collections.Generic;
using System.Linq;
using AzureVisionApiSimpleOcrSdk.Integration.Parser;
using AzureVisionApiSimpleOcrSdk.Model;
using FluentAssertions;
using Microsoft.ProjectOxford.Vision.Contract;
using Moq;
using NUnit.Framework;
using OcrMetadata.Model;
using Point = AzureVisionApiSimpleOcrSdk.Integration.Parser.Point;

namespace AzureVisionApiSimpleOcrSdkTest.Integration.Parser
{
    [TestFixture]
    public class AzureOcrParserTest
    {
        private Mock<ITransformLinesIntoSentences> _transformLinesIntoSenteces;
        private Mock<ISortIntoLogicalLines> _sortIntoLogicalLines;
        private AzureOcrParser _target;
        private Mock<IGetLinesOrderedByTopPosition> _getLinesOrderedByTopPosition;

        [SetUp]
        public void Setup()
        {
            _transformLinesIntoSenteces = new Mock<ITransformLinesIntoSentences>();
            _sortIntoLogicalLines = new Mock<ISortIntoLogicalLines>();
            _getLinesOrderedByTopPosition = new Mock<IGetLinesOrderedByTopPosition>();

            _target = new AzureOcrParser(_transformLinesIntoSenteces.Object, _sortIntoLogicalLines.Object,
                _getLinesOrderedByTopPosition.Object);
        }

        [Test]
        public void GivenNoRawResult_WhenInvokingExecute_ThenArgumentNullExceptionIsThrown()
        {
            //Arrange
            Action action = () => _target.Execute(null, 100, 100);

            //Act
            //Assert
            action.ShouldThrow<ArgumentNullException>()
                .WithMessage("Value cannot be null.\r\nParameter name: ocrOutput");
        }

        [Test]
        public void GivenOcrResult_WhenInvokingExecute_ThenGetLinesAndSortIntoLogicalLinesAndTransformIntoSentencesAreCalledAnImageContentIsReturned()
        {
            //Arrange
            var rawResult = new RawAzureOcrResult();
            int height = 100, width = 100;

            var azureLines = new List<Line>();
            _getLinesOrderedByTopPosition.Setup(x => x.Execute(rawResult)).Returns(azureLines);

            var logicalLines = new Mock<IOrderedEnumerable<KeyValuePair<Point, List<Line>>>>().Object;
            _sortIntoLogicalLines.Setup(x => x.Execute(azureLines)).Returns(logicalLines);

            var sentences = new List<ISentence>();
            _transformLinesIntoSenteces.Setup(x => x.Execute(height, width, logicalLines)).Returns(sentences);

            //Act
            var imageContent = _target.Execute(rawResult, height, width);

            //Assert
            Assert.AreEqual(sentences, imageContent.Sentences);
        }

        [Test]
        public void GivenEmptyOcrResult_WhenInvokingExecute_ThenGetLinesAndSortIntoLogicalLinesAndTransformIntoSentencesAreCalledAnImageContentIsReturned()
        {
            //Arrange
            var rawResult = new RawAzureOcrResult();
            int height = 100, width = 100;
            _getLinesOrderedByTopPosition.Setup(x => x.Execute(rawResult)).Returns((List<Line>) null);

            //Act
            var imageContent = _target.Execute(rawResult, height, width);

            //Assert
            imageContent.Should().NotBeNull();
            imageContent.Sentences.Should().BeEmpty();
        }
    }
}