using AzureVisionApiSimpleOcrSdk.Integration.Parser;
using FluentAssertions;
using Microsoft.ProjectOxford.Vision.Contract;
using Moq;
using NUnit.Framework;
using OcrMetadata.Model;
using OcrMetadata.Tools;

namespace AzureVisionApiSimpleOcrSdkTest.Integration.Parser
{
    [TestFixture]
    public class AzureCreateRelativeCoordinateTest
    {
        private Mock<ICreateRelativeCoordinate> _createRelativeMock;
        private AzureCreateRelativeCoordinate _target;

        [SetUp]
        public void Setup()
        {
            _createRelativeMock = new Mock<ICreateRelativeCoordinate>();
            _target = new AzureCreateRelativeCoordinate(_createRelativeMock.Object);
        }

        [Test]
        public void GivenARectangle_WhenCallingExecute_ThenValuesAreCorrectlyParsedOntoCreateRelativeCoordinateAndTheResultIsReturned()
        {
            //Arrange
            var expectedResult = new Coordinate(1,1,1,1);
            int x = 5,
                y = 10,
                height = 4,
                width = 15,
                imageHeight = 50,
                imageWidth = 200;
            var rectangle = new Rectangle {Left = x, Top = y, Height = height, Width = width};

            _createRelativeMock.Setup(mock => mock.Execute(x, y, height, width, imageHeight, imageWidth))
                .Returns(expectedResult);

            //Act
            var result = _target.Execute(rectangle, imageWidth, imageHeight);

            //Assert
            _createRelativeMock.Verify(mock => mock.Execute(x, y, height, width, imageHeight, imageWidth), Times.Once());
            result.Should().Be(expectedResult);
        }
    }
}