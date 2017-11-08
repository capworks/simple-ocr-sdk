using AzureVisionApiSimpleOcrSdk.Model;
using FluentAssertions;
using Microsoft.ProjectOxford.Vision.Contract;
using Moq;
using NUnit.Framework;
using Word = Microsoft.ProjectOxford.Vision.Contract.Word;

namespace AzureVisionApiSimpleOcrSdkTest.Model
{
    [TestFixture]
    public class RawAzureOcrResultTest
    {
        private Mock<OcrResults> _resultMock;
        private double? _textAngle;
        private string _language;
        private string _orientation;
        private Region[] _regions;

        [SetUp]
        public void Setup()
        {
            _textAngle = 23;
            _language = "da";
            _orientation = "test";
            _regions = new Region[]{};
            _resultMock = new Mock<OcrResults>();
            _resultMock.SetupAllProperties();
            _resultMock.Object.Language = _language;
            _resultMock.Object.Orientation = _orientation;
            _resultMock.Object.Regions = _regions;
            _resultMock.Object.TextAngle = _textAngle;
        }

        [Test]
        public void GivenAProjectOxfordOcrResult_WhenInvokingCreateFrom_ThenPropertiesAreSetCorrectly()
        {
            //Arrange / Act
            var result = RawAzureOcrResult.CreateFrom(_resultMock.Object);

            //Assert
            Assert.AreEqual(_language, result.Language);
            Assert.AreEqual(_orientation, result.Orientation);
            Assert.AreEqual(_regions, result.Regions);
            Assert.AreEqual(_textAngle, result.TextAngle);
        }

        [Test]
        public void GivenNoRegions_WhenCallingTextFound_ThenFalseIsReturned()
        {
            //Arrange
            var target = RawAzureOcrResult.CreateFrom(_resultMock.Object);

            //Act
            var result = target.TextFound();

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void GivenNullRegions_WhenCallingTextFound_ThenFalseIsReturned()
        {
            //Arrange 
            _resultMock.Object.Regions = null;
            var target = RawAzureOcrResult.CreateFrom(_resultMock.Object);

            //Act
            var result = target.TextFound();

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void GivenRegions_WhenCallingTextFound_ThenTrueIsReturned()
        {
            //Arrange 
            _resultMock.Object.Regions = new []{new Region()};
            var target = RawAzureOcrResult.CreateFrom(_resultMock.Object);

            //Act
            var result = target.TextFound();

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void GivenNoRegions_WhenCallingGetAsPlainText_ThenEmptyStringIsReturned()
        {
            //Arrange
            var target = RawAzureOcrResult.CreateFrom(_resultMock.Object);

            //Act
            var result = target.GetAsPlainText();

            //Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void GivenNullRegions_WhenCallingGetAsPlainText_ThenEmptyStringIsReturned()
        {
            //Arrange 
            _resultMock.Object.Regions = null;
            var target = RawAzureOcrResult.CreateFrom(_resultMock.Object);

            //Act
            var result = target.GetAsPlainText();

            //Assert
            result.Should().BeEmpty();
        }

        [Test]
        public void GivenRegions_WhenCallingGetAsPlainText_ThenTextOfAllWordsAreReturned()
        {
            //Arrange 
            _resultMock.Object.Regions = new[]
            {
                new Region
                {
                    Lines = new[]
                    {
                        new Line {Words = new[] {new Word {Text = "Test A"}, new Word {Text = "Test B"}}},
                        new Line {Words = new[] {new Word {Text = "Test C"}}}
                    }
                },
                new Region
                {
                    Lines = new[]
                    {
                        new Line {Words = new[] {new Word {Text = "Test D"}, new Word {Text = "Test E"}}},
                    }
                }
            };
            var target = RawAzureOcrResult.CreateFrom(_resultMock.Object);

            //Act
            var result = target.GetAsPlainText();

            //Assert
            result.Should().Be("Test A Test B Test C Test D Test E");
        }
    }
}
