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
    public class TransformLinesIntoSentecesTest
    {
        private Mock<IAddSentencesAndReturnNewIndex> _addSentencesAndReturnNewIndex;
        private TransformLinesIntoSentences _target;
        private int _height;
        private int _width;
        private List<KeyValuePair<Point, List<Line>>> _logicalLines;
        private KeyValuePair<Point, List<Line>> _line1;
        private KeyValuePair<Point, List<Line>> _line2;
        private KeyValuePair<Point, List<Line>> _line3;

        [SetUp]
        public void Setup()
        {
            _height = 100;
            _width = 120;

            _line1 = new KeyValuePair<Point, List<Line>>(new Point {Top = 1}, new List<Line>() );
            _line2 = new KeyValuePair<Point, List<Line>>(new Point { Top = 2 }, new List<Line>());
            _line3 = new KeyValuePair<Point, List<Line>>(new Point { Top = 3 }, new List<Line>());

            _logicalLines = new List<KeyValuePair<Point, List<Line>>>()
            {
                _line1,
                _line2,
                _line3
            };

            _addSentencesAndReturnNewIndex = new Mock<IAddSentencesAndReturnNewIndex>();
            _target = new TransformLinesIntoSentences(_addSentencesAndReturnNewIndex.Object);
        }

        [Test]
        public void GivenLogicalLinesAreNull_WhenInvokingExecute_ThenExceptionIsThrown()
        {
            //Arrange
            //Act
            Action action = () => _target.Execute(_height, _width, null);

            //Assert
            action.ShouldNotThrow<NullReferenceException>();
        }

        [Test]
        public void GivenLogicalLines_WhenInvokingExecute_ThenAddSentencesAndReturnNewIndexIsCalledWithIncrementedLineCount()
        {
            //Arrange
            //Act
            var result = _target.Execute(_height, _width, _logicalLines);

            //Assert
            _addSentencesAndReturnNewIndex.Verify(
                x => x.Execute(_height, _width, _line1, 0 ,It.IsAny<int>(), result), Times.Once());

            _addSentencesAndReturnNewIndex.Verify(
                x => x.Execute(_height, _width, _line2, 1, It.IsAny<int>(), result), Times.Once());

            _addSentencesAndReturnNewIndex.Verify(
                x => x.Execute(_height, _width, _line3, 2, It.IsAny<int>(), result), Times.Once());
        }

        [Test]
        public void GivenLogicalLines_WhenInvokingExecute_ThenSentenceIndexIsUpdatedForEachAddSentenceCallAndUpdatedValuesIsUsedForNextCall()
        {
            //Arrange
            _addSentencesAndReturnNewIndex.Setup(
                x => x.Execute(_height, _width, _line1, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ISentence>>())).Returns(2);

            _addSentencesAndReturnNewIndex.Setup(
                x => x.Execute(_height, _width, _line2, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<ISentence>>())).Returns(130);

            //Act
            var result = _target.Execute(_height, _width, _logicalLines);

            //Assert
            _addSentencesAndReturnNewIndex.Verify(
                x => x.Execute(_height, _width, _line1, 0, 0, result), Times.Once());

            _addSentencesAndReturnNewIndex.Verify(
                x => x.Execute(_height, _width, _line2, 1, 2, result), Times.Once());

            _addSentencesAndReturnNewIndex.Verify(
                x => x.Execute(_height, _width, _line3, 2, 130, result), Times.Once());
        }
    }
}