using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EDILibrary;

namespace EDILibraryTests
{
    /// <summary>
    /// Tests <see cref="EdifactFormatVersion"/> and <see cref="EdifactFormat"/>
    /// </summary>
    [TestClass]
    public class EdifactFormatVersionTests
    {
        [TestMethod]
        [DataRow("21009", EdifactFormat.IFTSTA)]
        [DataRow("23001", EdifactFormat.INSRPT)]
        [DataRow("31001", EdifactFormat.INVOIC)]
        [DataRow("17002", EdifactFormat.ORDERS)]
        [DataRow("19002", EdifactFormat.ORDRSP)]
        [DataRow("27005", EdifactFormat.PRICAT)]
        [DataRow("11042", EdifactFormat.UTILMD)]
        [DataRow("13002", EdifactFormat.MSCONS)]
        [DataRow("35002", EdifactFormat.QUOTES)]
        [DataRow("15002", EdifactFormat.REQOTE)]
        [DataRow("33001", EdifactFormat.REMADV)]
        [DataRow("11042", EdifactFormat.UTILMD)]
        public void TestPruefiToFormat(string pruefi, EdifactFormat expectedFormat)
        {
            var actualFormat = EdifactFormatHelper.FromPruefidentifikator(pruefi);
            Assert.AreEqual(expectedFormat, actualFormat);
        }

        [TestMethod]
        public void NoPruefiNoFormat()
        {
            Assert.ThrowsException<ArgumentNullException>(() => EdifactFormatHelper.FromPruefidentifikator(null));
            Assert.ThrowsException<ArgumentNullException>(() => EdifactFormatHelper.FromPruefidentifikator("   "));
        }

        [TestMethod]
        public void UnmappedThrowsNotImplemented()
        {
            Assert.ThrowsException<NotImplementedException>(() => EdifactFormatHelper.FromPruefidentifikator("88888"));
        }

        [TestMethod]
        [DataRow("04/21", EdifactFormatVersion.FV2104)]
        [DataRow("FV2104", EdifactFormatVersion.FV2104)]
        [DataRow("04/20", EdifactFormatVersion.FV2004)]
        [DataRow("FV2004", EdifactFormatVersion.FV2004)]
        [DataRow("12/19", EdifactFormatVersion.FV1912)]
        [DataRow("FV1912", EdifactFormatVersion.FV1912)]
        [DataRow("04/19", EdifactFormatVersion.FV1904)]
        [DataRow("FV1904", EdifactFormatVersion.FV1904)]
        [DataRow("10/17", EdifactFormatVersion.FV1710)]
        [DataRow("FV1710", EdifactFormatVersion.FV1710)]
        public void TestLegacyStrings(string legacyString, EdifactFormatVersion expectedFormatVersion)
        {
            var actualFormatVersion = legacyString.ToEdifactFormatVersion();
            Assert.AreEqual(expectedFormatVersion, actualFormatVersion);
            if (!legacyString.StartsWith("FV"))
            {
                Assert.AreEqual(legacyString, expectedFormatVersion.ToLegacyVersionString());
            }
            else
            {
                Assert.AreEqual(legacyString, expectedFormatVersion.ToString());
            }
        }
    }
}