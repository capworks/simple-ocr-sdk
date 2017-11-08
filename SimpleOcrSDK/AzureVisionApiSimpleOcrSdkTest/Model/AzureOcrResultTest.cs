using System;
using AzureVisionApiSimpleOcrSdk.Model;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OcrMetadata.Model;

namespace AzureVisionApiSimpleOcrSdkTest.Model
{
    [TestFixture]
    public class AzureOcrResultTest
    {
        private TimeSpan _timespan;
        private RawAzureOcrResult _rawResult;
        private Mock<IImageContent> _imageContent;

        [SetUp]
        public void Setup()
        {
            _timespan = new TimeSpan();
            _rawResult = new Mock<RawAzureOcrResult>().Object;
            _imageContent = new Mock<IImageContent>();
        }

        [Test]
        public void GivenNoException_WhenCallingCreateErrorResult_ThenArgumentNullExceptionIsThrown()
        {
            //Arrange
            Action act = () => AzureOcrResult.CreateErrorResult(_timespan, null);

            //Act
            //Assert
            act.ShouldThrow<ArgumentNullException>()
                .WithMessage("Value cannot be null.\r\nParameter name: error");
        }

        [Test]
        public void GivenException_WhenCallingCreateErrorResult_ThenPropertiesShouldBeSetAsExpected()
        {
            //Arrange
            var exception = new Mock<Exception>().Object;

            //Act
            var result = AzureOcrResult.CreateErrorResult(_timespan, exception);

            //Assert
            result.RawResult.Should().BeNull();
            result.Content.Should().BeNull();
            result.ProcessTime.Should().Be(_timespan);
            result.HasError.Should().BeTrue();
            result.Error.Should().Be(exception);
            result.TextFound.Should().BeFalse();
        }

        [Test]
        public void GivenNoImageContent_WhenCallingCreateSuccesResult_ThenArgumentNullExceptionIsThrown()
        {
            //Arrange
            Action act = () => AzureOcrResult.CreateSuccesResult(_timespan, null, _rawResult);

            //Act
            //Assert
            act.ShouldThrow<ArgumentNullException>()
                .WithMessage("Value cannot be null.\r\nParameter name: imageContent");
        }
        [Test]
        public void GivenNoRawResult_WhenCallingCreateSuccesResult_ThenArgumentNullExceptionIsThrown()
        {
            //Arrange
            Action act = () => AzureOcrResult.CreateSuccesResult(_timespan, _imageContent.Object, null);

            //Act
            //Assert
            act.ShouldThrow<ArgumentNullException>()
                .WithMessage("Value cannot be null.\r\nParameter name: rawAzureOcrResult");
        }

        [Test]
        public void GivenRawResultAndContent_WhenCallingCreateSuccesResult_ThenPropertiesShouldBeSetAsExpected()
        {
            //Arrange
            //Act
            var result = AzureOcrResult.CreateSuccesResult(_timespan, _imageContent.Object, _rawResult);

            //Assert
            result.RawResult.Should().Be(_rawResult);
            result.Content.Should().Be(_imageContent.Object);
            result.ProcessTime.Should().Be(_timespan);
            result.HasError.Should().BeFalse();
            result.Error.Should().BeNull();
            result.TextFound.Should().BeFalse();
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void GivenSuccessAzureOcrResult_WhenCallingTextFound_ThenReturnValueShouldBeSameAsImageContentTextFound(bool textFound, bool expected)
        {
            //Arrange
            _imageContent.Setup(c => c.TextFound()).Returns(textFound);
            var ocrResult = AzureOcrResult.CreateSuccesResult(_timespan, _imageContent.Object, _rawResult);

            //Act
            var result = ocrResult.TextFound;

            //Assert
            result.Should().Be(expected);
        }
    }
}