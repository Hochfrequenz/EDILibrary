using System;
using System.Linq;
using System.Threading.Tasks;
using EDILibrary;
using EDILibrary.Interfaces;
using EDILibrary.MAUS;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace EDILibraryTests
{
    [TestClass]
    public class TemplateLoaderExtensionTests
    {
        /// <summary>
        /// In case the requested template exists, it should be loaded directly; Without using any fallback
        /// </summary>
        [TestMethod]
        public async Task TestLoadingRegularTemplateIfExists()
        {
            var loaderMock = new Mock<ITemplateLoader>();
            loaderMock.Setup(l => l.LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002")).ReturnsAsync(new Anwendungshandbuch()).Verifiable();

            var loaderUnderTest = loaderMock.Object;
            var (actualMaus, actualFormatVersion) = await loaderUnderTest.LoadMausTemplateOrFallback(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002");
            actualMaus.Should().NotBeNull();
            actualFormatVersion.Should().Be(EdifactFormatVersion.FV2304);

            loaderMock.VerifyAll();
            loaderMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// In case an older template exists, it should be returned instead of the requested one
        /// </summary>
        [TestMethod]
        public async Task TestLoadingFallbackTemplateIfExists()
        {
            var loaderMock = new Mock<ITemplateLoader>();
            loaderMock.Setup(l => l.LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002")).ReturnsAsync((Anwendungshandbuch)null).Verifiable();
            loaderMock.Setup(l => l.LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2210, "13002")).ReturnsAsync((Anwendungshandbuch)null).Verifiable();
            loaderMock.Setup(l => l.LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2110, "13002")).ReturnsAsync(new Anwendungshandbuch()).Verifiable();

            var loaderUnderTest = loaderMock.Object;
            var (actualMaus, actualFormatVersion) = await loaderUnderTest.LoadMausTemplateOrFallback(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002");
            actualMaus.Should().NotBeNull();
            actualFormatVersion.Should().Be(EdifactFormatVersion.FV2110);

            loaderMock.VerifyAll();
            loaderMock.VerifyNoOtherCalls();
        }

        /// <summary>
        /// If not even a fallback exists, only null should be returned
        /// </summary>
        [TestMethod]
        public async Task TestLoadingNothingIfNotEvenAFallbackExists()
        {
            var loaderMock = new Mock<ITemplateLoader>();
            loaderMock.Setup(l => l.LoadMausTemplate(EdifactFormat.MSCONS, It.IsAny<EdifactFormatVersion>(), "13002")).ReturnsAsync((Anwendungshandbuch)null).Verifiable();


            var loaderUnderTest = loaderMock.Object;
            var (actualMaus, actualFormatVersion) = await loaderUnderTest.LoadMausTemplateOrFallback(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002");
            actualMaus.Should().BeNull();
            actualFormatVersion.Should().BeNull();

            loaderMock.VerifyAll();
        }
    }
}
