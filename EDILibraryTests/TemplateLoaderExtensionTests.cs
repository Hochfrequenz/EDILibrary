using System;
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
        [TestMethod]
        public async Task TestLoadingRegularTemplateIfExists()
        {
            var loaderMock = new Mock<ITemplateLoader>();
            loaderMock.Setup(l => l.LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002")).ReturnsAsync(new Anwendungshandbuch());

            var loaderUnderTest = loaderMock.Object;
            var (actualMaus, actualFormatVersion) = await loaderUnderTest.LoadMausTemplateOrFallback(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002");
            actualMaus.Should().NotBeNull();
            actualFormatVersion.Should().Be(EdifactFormatVersion.FV2304);
        }

        [TestMethod]
        public async Task TestLoadingFallbackTemplateIfExists()
        {
            var loaderMock = new Mock<ITemplateLoader>();
            loaderMock.Setup(l => l.LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002")).ReturnsAsync((Anwendungshandbuch)null);
            loaderMock.Setup(l => l.LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2210, "13002")).ReturnsAsync((Anwendungshandbuch)null);
            loaderMock.Setup(l => l.LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2110, "13002")).ReturnsAsync(new Anwendungshandbuch());

            var loaderUnderTest = loaderMock.Object;
            var (actualMaus, actualFormatVersion) = await loaderUnderTest.LoadMausTemplateOrFallback(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002");
            actualMaus.Should().NotBeNull();
            actualFormatVersion.Should().Be(EdifactFormatVersion.FV2110);
        }

        [TestMethod]
        public async Task TestLoadingNothingIfNotEvenAFallbackExists()
        {
            var loaderMock = new Mock<ITemplateLoader>();
            loaderMock.Setup(l => l.LoadMausTemplate(EdifactFormat.MSCONS, It.IsAny<EdifactFormatVersion>(), "13002")).ReturnsAsync((Anwendungshandbuch)null);

            var loaderUnderTest = loaderMock.Object;
            var (actualMaus, actualFormatVersion) = await loaderUnderTest.LoadMausTemplateOrFallback(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002");
            actualMaus.Should().BeNull();
            actualFormatVersion.Should().BeNull();
        }
    }
}
