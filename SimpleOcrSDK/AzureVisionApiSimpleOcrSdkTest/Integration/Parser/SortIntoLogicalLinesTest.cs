using System.Collections.Generic;
using System.Linq;
using AzureVisionApiSimpleOcrSdk.Integration.Parser;
using FluentAssertions;
using Microsoft.ProjectOxford.Vision.Contract;
using NUnit.Framework;

namespace AzureVisionApiSimpleOcrSdkTest.Integration.Parser
{
    [TestFixture]
    public class SortIntoLogicalLinesTest
    {
        [Test]
        public void Test1()
        {
            //Arrange
            var logicLine1a = new Line { BoundingBox = "0,4,0,10" };
            var logicLine1b = new Line { BoundingBox = "0,5,0,5" };

            var logicLine2a = new Line { BoundingBox = "0,5,0,15" };
            var logicLine2b = new Line { BoundingBox = "0,6,0,10" };
            var logicLine2c = new Line { BoundingBox = "0,5,0,15" };

            var lines = new List<Line>() {logicLine2a, logicLine2b, logicLine1a, logicLine2c, logicLine1b };

            var target = new SortIntoLogicalLines();

            //Act
            var result = target.Execute(lines);

            //Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);

            var line1 = result.ToList()[0];
            line1.Key.Top.Should().Be(4);
            line1.Key.Bottom.Should().Be(14);
            line1.Value.Count.Should().Be(2);
            line1.Value.Contains(logicLine1a);
            line1.Value.Contains(logicLine1b);

            var line2 = result.ToList()[1];
            line2.Key.Top.Should().Be(5);
            line2.Key.Bottom.Should().Be(20);
            line2.Value.Count.Should().Be(3);
            line2.Value.Contains(logicLine2a);
            line2.Value.Contains(logicLine2b);
            line2.Value.Contains(logicLine2c);
        }
    }
}