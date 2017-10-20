using AzureVisionApiSimpleOcrSdk.Integration.Parser;
using FluentAssertions;
using NUnit.Framework;

namespace AzureVisionApiSimpleOcrSdkTest.Integration.Parser
{
    [TestFixture]
    public class PointTest
    {
        [TestCase(0, 0, 0, 0, true, "When both point is one pixel true is returned")]
        [TestCase(999, 999, 999, 999, true, "When both point is one pixel true is returned")]
        [TestCase(-999, -999, -999, -999, true, "When both point is one pixel true is returned, we do not care about negtive coords.")]
        [TestCase(50, 60, 50, 60, true, "If the same pixel range, true is returned.")]
        [TestCase(50, 60, 10, 65, true, "If smaller pixel range, within the compare poinr, then true is returned")]
        [TestCase(10, 12, 11, 15, false, "If bottom is outside the compare pixel range, then false is returned.")]
        [TestCase(11, 16, 11, 15, false, "If top is outside the compare pixel range, then false is returned.")]
        [TestCase(10, 11, 12, 15, false, "If outside the compare pixel range, then false is returned.")]
        public void GivenAPointAndAComparePoint_WhenInvokingIsWithinThisPoint_TheExpectedResultIsReturned(int top,
            int bottom, int compareTop, int compareBottom, bool expectedResult, string caseDescription)
        {
            //Arrange
            var point = new Point {Bottom = bottom, Top = top};
            var comparePoint = new Point {Bottom = compareBottom, Top = compareTop};

            //Act
            var result = point.IsWithinThisPoint(comparePoint);

            //Assert
            result.Should().Be(expectedResult, caseDescription);
        }
    }
}