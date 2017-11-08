using System;
using System.Collections.Generic;
using AzureVisionApiSimpleOcrSdk.Integration.Parser;
using FluentAssertions;
using Microsoft.ProjectOxford.Vision.Contract;
using Moq;
using NUnit.Framework;
using OcrMetadata.Model;
using Point = AzureVisionApiSimpleOcrSdk.Integration.Parser.Point;

namespace AzureVisionApiSimpleOcrSdkTest.Integration.Parser
{
    [TestFixture]
    public class AddSentencesAndReturnNewIndexTest
    {
        private Mock<ITransformAzureLineIntoSentence> _transformAzureLineIntoSentence;
        private AddSentencesAndReturnNewIndex _target;
        private int _height;
        private int _width;
        private KeyValuePair<Point, List<Line>> _line;
        private Line _azureLine1;
        private Line _azureLine2;
        private Line _azureLine3;
        private int _lineCount;

        [SetUp]
        public void Setup()
        {
            _height = 100;
            _width = 120;
            _lineCount = 12;

            _azureLine1 = new Line();
            _azureLine2 = new Line();
            _azureLine3 = new Line();
            _azureLine1.BoundingBox = "0,0,0,0";
            _azureLine2.BoundingBox = "0,0,0,0";
            _azureLine3.BoundingBox = "0,0,0,0";

            _line = new KeyValuePair<Point, List<Line>>(new Point(), new List<Line>()
            {
                _azureLine1,
                _azureLine2,
                _azureLine3
            });

            _transformAzureLineIntoSentence = new Mock<ITransformAzureLineIntoSentence>();
            _target = new AddSentencesAndReturnNewIndex(_transformAzureLineIntoSentence.Object);
        }

        [Test]
        public void GivenSentencesIsNull_WhenInvokingExecute_ThenExceptionIsThrown()
        {
            //Arrange
            //Act
            Action action = () => _target.Execute(0, 0, new KeyValuePair<Point, List<Line>>(), 0, 0, null);

            //Assert
            action.ShouldNotThrow<NullReferenceException>();
        }

        [Test]
        public void GivenLine_WhenInvokingExecute_ThenTransformAzureLineIntoSentenceIsCalledForeachLine()
        {
            //Arrange
            var index = 42;

            //Act
            _target.Execute(_height, _width, _line, _lineCount, index, new List<ISentence>());

            //Assert
            _transformAzureLineIntoSentence.Verify(x => x.Execute(_azureLine1, _lineCount, _width, _height, It.IsAny<int>()), Times.Once);
            _transformAzureLineIntoSentence.Verify(x => x.Execute(_azureLine2, _lineCount, _width, _height, It.IsAny<int>()), Times.Once);
            _transformAzureLineIntoSentence.Verify(x => x.Execute(_azureLine3, _lineCount, _width, _height, It.IsAny<int>()), Times.Once);
        }

        [Test]
        public void GivenLine_WhenInvokingExecute_ThenTransformAzureLineIntoSentenceIsCalledWithIncrementingIndexAndNewIndexisReturned()
        {
            //Arrange
            var index = 42;

            //Act
            var result = _target.Execute(_height, _width, _line, _lineCount, index, new List<ISentence>());

            //Assert
            _transformAzureLineIntoSentence.Verify(x => x.Execute(_azureLine1, _lineCount, _width, _height, 42), Times.Once);
            _transformAzureLineIntoSentence.Verify(x => x.Execute(_azureLine2, _lineCount, _width, _height, 43), Times.Once);
            _transformAzureLineIntoSentence.Verify(x => x.Execute(_azureLine3, _lineCount, _width, _height, 44), Times.Once);
            result.Should().Be(45);
        }

        [Test]
        public void GivenLine_WhenInvokingExecute_ThenTransformAzureLineIsOrderedByLeftAndSentecesIsAddedInOrderToSentencesList()
        {
            //Arrange
            var index = 42;
            var list = new List<ISentence>();
            _azureLine1.BoundingBox = "50,0,0,0";
            _azureLine2.BoundingBox = "4,0,0,0";
            _azureLine3.BoundingBox = "12,0,0,0";

            var sentence1 = new Mock<ISentence>().Object;
            var sentence2 = new Mock<ISentence>().Object;
            var sentence3 = new Mock<ISentence>().Object;

            _transformAzureLineIntoSentence.Setup(x => x.Execute(_azureLine1, _lineCount, _width, _height, 44)).Returns(sentence1);
            _transformAzureLineIntoSentence.Setup(x => x.Execute(_azureLine2, _lineCount, _width, _height, 42)).Returns(sentence2);
            _transformAzureLineIntoSentence.Setup(x => x.Execute(_azureLine3, _lineCount, _width, _height, 43)).Returns(sentence3);

            //Act
            _target.Execute(_height, _width, _line, _lineCount, index, list);

            //Assert
            list.Count.Should().Be(3);

            list[0].Should().Be(sentence2);
            list[1].Should().Be(sentence3);
            list[2].Should().Be(sentence1);
        }
    }
}