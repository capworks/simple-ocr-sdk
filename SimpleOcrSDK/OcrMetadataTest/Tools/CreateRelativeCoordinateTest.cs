using FluentAssertions;
using NUnit.Framework;
using OcrMetadata.Tools;

namespace OcrMetadataTest.Tools
{
    [TestFixture]
    public class CreateRelativeCoordinateTest
    {
        private CreateRelativeCoordinate _target;

        [SetUp]
        public void Setup()
        {
            _target = new CreateRelativeCoordinate();
        }

        [TestCase(10, 7, 3, 18, 50, 200, 0.05, 0.14, 0.06, 0.09)]
        [TestCase(5, 10, 15, 4, 100, 100, 0.05, 0.10, 0.15, 0.04)]
        public void TestGivenCoordinates_WhenInvokingExecute_ThenCorrectRelativeCoordinatesIsReturned(double x,
            double y, double height, double width, double imgHeight, double imgWidth,
            double expectedX, double expectedY, double expectedHeight, double expectedWidth)
        {
            //Arrange
            //Act
            var result = _target.Execute(x, y, height, width, imgHeight, imgWidth);

            //Assert
            result.X.Should().Be(expectedX);
            result.Y.Should().Be(expectedY);
            result.Height.Should().Be(expectedHeight);
            result.Width.Should().Be(expectedWidth);
        }
    }
}
