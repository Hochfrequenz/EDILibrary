using System;
using System.Collections.Generic;
using EDILibrary;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        [DataRow("15002", EdifactFormat.QUOTES)]
        [DataRow("35002", EdifactFormat.REQOTE)]
        [DataRow("33001", EdifactFormat.REMADV)]
        [DataRow("11042", EdifactFormat.UTILMD)]
        [DataRow("44001", EdifactFormat.UTILMDG)]
        [DataRow("55001", EdifactFormat.UTILMDS)]
        public void TestPruefiToFormat(string pruefi, EdifactFormat expectedFormat)
        {
            var actualFormat = EdifactFormatHelper.FromPruefidentifikator(pruefi);
            Assert.AreEqual(expectedFormat, actualFormat);
        }
        [TestMethod]
        [Obsolete]
        public void TestCompare()
        {
            const EdifactFormatVersion a = EdifactFormatVersion.FV1710;
            const EdifactFormatVersion b = EdifactFormatVersion.FV1904;
            Assert.IsTrue(a.CompareToVersion(b) == -1);
            Assert.IsTrue(b > a);
            Assert.IsFalse(a > b);
        }

        [TestMethod]
        public void TestFormatVersionOrder()
        {
            var expectedNaturalOrder = new List<EdifactFormatVersion>
            {
                // this is the chronological order as it should be
                EdifactFormatVersion.FV1710,
                EdifactFormatVersion.FV1904,
                EdifactFormatVersion.FV1912,
                EdifactFormatVersion.FV2004,
                EdifactFormatVersion.FV2104,
                EdifactFormatVersion.FV2110,
                EdifactFormatVersion.FV2210,
                EdifactFormatVersion.FV2304,
                EdifactFormatVersion.FV2310,
                EdifactFormatVersion.FV2404,
            };
            var comparer = new EdifactFormatVersionComparer();
            for (int i = 0; i < expectedNaturalOrder.Count - 1; i++)
            {
                for (int j = i + 1; j < expectedNaturalOrder.Count; j++)
                {
                    Assert.IsTrue(expectedNaturalOrder[i] < expectedNaturalOrder[j]);
                    Assert.IsTrue(comparer.Compare(expectedNaturalOrder[i], expectedNaturalOrder[j]) < 0);
                }
            }
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
        [DataRow("10/22", EdifactFormatVersion.FV2210)]
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
        [DataRow("FV2404", EdifactFormatVersion.FV2404)]
        [DataRow("04/24", EdifactFormatVersion.FV2404)]
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

        /// <summary>
        /// this is just a placeholder for actual business logic
        /// </summary>
        /// <param name="versionProvider"></param>
        /// <returns></returns>
        private EdifactFormatVersion ActualCode(IEdifactFormatVersionProvider versionProvider)
        {
            return versionProvider.GetCurrent();
        }

        /// <summary>
        /// This test is just to show how the <see cref="EdifactFormatHelper"/> is thought to be used: behind an interface!
        /// </summary>
        [TestMethod]
        public void TestMockingVersionProvider()
        {
            var versionProviderMock = new Moq.Mock<IEdifactFormatVersionProvider>();
            versionProviderMock.Setup(vp => vp.GetCurrent()).Returns(EdifactFormatVersion.FV1904);
            Assert.AreEqual(EdifactFormatVersion.FV1904, ActualCode(versionProviderMock.Object));
        }

        /// <summary>
        /// Test that the <see cref="EdifactFormatVersionHelper"/> returns <see cref="EdifactFormatVersion.FV2110"/> in for 2021-10-01&lt;=dt&lt;2022-10-01 (german local time)
        /// </summary>
        [TestMethod]
        public void TestFV2110()
        {
            IEdifactFormatVersionProvider versionProvider = new EdifactFormatVersionHelper();
            Assert.AreEqual(EdifactFormatVersion.FV2104, versionProvider.GetFormatVersion(new DateTimeOffset(2021, 9, 30, 21, 59, 59, TimeSpan.Zero)));
            Assert.AreEqual(EdifactFormatVersion.FV2110, versionProvider.GetFormatVersion(new DateTimeOffset(2021, 9, 30, 22, 0, 0, TimeSpan.Zero)));
            Assert.AreEqual(EdifactFormatVersion.FV2110, versionProvider.GetFormatVersion(new DateTimeOffset(2022, 3, 31, 22, 0, 0, TimeSpan.Zero)));
            Assert.AreEqual(EdifactFormatVersion.FV2110, versionProvider.GetFormatVersion(new DateTimeOffset(2022, 9, 30, 21, 59, 59, TimeSpan.Zero)));
            Assert.AreEqual(EdifactFormatVersion.FV2210, versionProvider.GetFormatVersion(new DateTimeOffset(2022, 9, 30, 22, 0, 0, TimeSpan.Zero)));
            Assert.AreEqual(EdifactFormatVersion.FV2310, versionProvider.GetFormatVersion(new DateTimeOffset(2023, 9, 30, 22, 0, 0, TimeSpan.Zero)));
            Assert.AreEqual(EdifactFormatVersion.FV2404, versionProvider.GetFormatVersion(new DateTimeOffset(2024, 3, 31, 22, 0, 0, TimeSpan.Zero)));
        }
    }
}
