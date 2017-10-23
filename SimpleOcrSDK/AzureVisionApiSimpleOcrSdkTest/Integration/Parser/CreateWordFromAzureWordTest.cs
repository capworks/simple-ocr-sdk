using System;
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
    public class CreateWordFromAzureWordTest
    {
        private Mock<IAzureCreateRelativeCoordinate> _relativeCoords;
        private CreateWordFromAzureWord _target;
        private int _imgHeight;
        private int _imgWidth;

        [SetUp]
        public void Setup()
        {
            _imgWidth = 100;
            _imgHeight = 200;
            _relativeCoords = new Mock<IAzureCreateRelativeCoordinate>();
            _target = new CreateWordFromAzureWord(_relativeCoords.Object);
        }

        [Test]
        public void GivenWordIsNull_WhenInvokingExecute_ThenExceptionIsThrown()
        {
            //Arrange
            //Act
            Action action = ()=> _target.Execute(null, _imgWidth, _imgHeight);

            //Assert
            action.ShouldNotThrow<NullReferenceException>();
        }

        [Test]
        public void GivenWord_WhenInvokingExecute_ThenWordIsReturned()
        {
            //Arrange
            var left = 10;
            var top = 20;
            var width = 40;
            var height = 15;
            var returnCoord = new Coordinate(0, 0, 0, 0);
            _relativeCoords.Setup(z => z.Execute(
                It.Is<Rectangle>(x => x.Height == height && x.Width == width && x.Top == top && x.Left == left),
                100, 200)).Returns(returnCoord);

            var wordValue = "MyTestWord";

            var word = new Word
            {
                BoundingBox = $"{left},{top},{width},{height}",
                Text = wordValue
            };

            //Act
            var result = _target.Execute(word, _imgWidth, _imgHeight);

            //Assert
            result.Value.Should().Be(wordValue);
            result.Coordinate.Should().Be(returnCoord);
        }
    }
}