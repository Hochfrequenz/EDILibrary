using System;
using System.Threading.Tasks;
using EDILibrary;
using EDILibrary.Interfaces;
using EDILibrary.MAUS;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

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
            var loaderMock = Substitute.For<ITemplateLoader>();
            loaderMock
                .LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002")
                .Returns(Task.FromResult(new Anwendungshandbuch()));

            var (actualMaus, actualFormatVersion) = await loaderMock.LoadMausTemplateOrFallback(
                EdifactFormat.MSCONS,
                EdifactFormatVersion.FV2304,
                "13002"
            );
            actualMaus.Should().NotBeNull();
            actualFormatVersion.Should().Be(EdifactFormatVersion.FV2304);

            loaderMock.Received();
        }

        /// <summary>
        /// In case an older template exists, it should be returned instead of the requested one
        /// </summary>
        [TestMethod]
        public async Task TestLoadingFallbackTemplateIfExists()
        {
            var loaderMock = Substitute.For<ITemplateLoader>();
            loaderMock
                .LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2304, "13002")
                .Returns(Task.FromResult((Anwendungshandbuch)null));
            loaderMock
                .LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2210, "13002")
                .Returns(Task.FromResult((Anwendungshandbuch)null));
            loaderMock
                .LoadMausTemplate(EdifactFormat.MSCONS, EdifactFormatVersion.FV2110, "13002")
                .Returns(Task.FromResult(new Anwendungshandbuch()));

            var loaderUnderTest = loaderMock;
            var (actualMaus, actualFormatVersion) =
                await loaderUnderTest.LoadMausTemplateOrFallback(
                    EdifactFormat.MSCONS,
                    EdifactFormatVersion.FV2304,
                    "13002"
                );
            actualMaus.Should().NotBeNull();
            actualFormatVersion.Should().Be(EdifactFormatVersion.FV2110);

            loaderMock.Received();
        }

        /// <summary>
        /// If not even a fallback exists, only null should be returned
        /// </summary>
        [TestMethod]
        public async Task TestLoadingNothingIfNotEvenAFallbackExists()
        {
            var loaderMock = Substitute.For<ITemplateLoader>();
            loaderMock
                .LoadMausTemplate(EdifactFormat.MSCONS, Arg.Any<EdifactFormatVersion>(), "13002")
                .Returns(Task.FromResult((Anwendungshandbuch)null));

            var (actualMaus, actualFormatVersion) = await loaderMock.LoadMausTemplateOrFallback(
                EdifactFormat.MSCONS,
                EdifactFormatVersion.FV2304,
                "13002"
            );
            actualMaus.Should().BeNull();
            actualFormatVersion.Should().BeNull();

            loaderMock.ReceivedCalls();
        }
    }
}
