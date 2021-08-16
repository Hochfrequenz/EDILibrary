using EDILibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDILibraryTests
{
    /// <summary>
    /// Tests <see cref="EDIHelper"/>
    /// </summary>
    [TestClass]
    public class EDIHelperTests
    {
        /// <summary>
        /// Tests <see cref="EDIHelper.RemoveBOM"/>
        /// </summary>
        [TestMethod]
        [DataRow("UNHfoo bar asd", "UNHfoo bar asd", true)]
        [DataRow("\xFEFFUNH foo bar asd", "UNH foo bar asd", true)]
        [DataRow("asdUNH foo bar asd", "asdUNH foo bar asd", false)]
        [DataRow("", "", false)]
        [DataRow(null, null, false)]
        public void TestRemoveBOM(string raw, string expectedResult, bool legacySupported)
        {
            var actual = EDIHelper.RemoveByteOrderMark(raw);
            Assert.AreEqual(expectedResult, actual);
            if (legacySupported)
            {
#pragma warning disable 618
                var actualLegacy = EDIHelper.RemoveBOM(raw);
#pragma warning restore 618
                Assert.AreEqual(expectedResult, actualLegacy);
            }
        }

        /// <summary>
        /// Tests <see cref="EDIHelper.NormalizeEDIHeader"/>
        /// </summary>
        [TestMethod]
        [DataRow(null, null)]
        [DataRow("foo", "UNA:+.? 'oo")] // todo: I doubt this is the correct behaviour. it's probably because of the deprecated "RemoveBOM" function
        [DataRow("asd", "UNA:+.? 'sd")] // todo: I doubt this is the correct behaviour. it's probably because of the deprecated "RemoveBOM" function
        [DataRow("Usd", "UNA:+.? 'Usd")] // todo: I doubt this is the correct behaviour. it's probably because of the deprecated "RemoveBOM" function
        [DataRow("UNH+WIM00000000901+UTILMD:D:11A:UN:5.2b", "UNA:+.? 'UNH+WIM00000000901+UTILMD:D:11A:UN:5.2b")]
        [DataRow("UNA:+.? 'UNB+UNOC:3+123456789012345:500+123456789:500+210326:1553+WIM00000000901'UNH+WIM00000000901+UTILMD:D:11A:UN:5.2b'",
            "UNA:+.? 'UNB+UNOC:3+123456789012345:500+123456789:500+210326:1553+WIM00000000901'UNH+WIM00000000901+UTILMD:D:11A:UN:5.2b'")]
        [DataRow("UNA!;,= &UNB;UNOC!3;123456789012345!500;123456789!500;210326!1553;WIM00000000901&UNH;WIM00000000901;UTILMD!D!11A!UN!5,2b&",
            "UNA:+.? 'UNB;UNOC!3;123456789012345!500;123456789!500;210326!1553;WIM00000000901&UNH;WIM00000000901;UTILMD!D!11A!UN!5.2b&")] // todo: I doubt this is the correct behaviour
        public void TestNormalizeEdiHeader(string input, string expectedResult)
        {
            var actual = EDIHelper.NormalizeEDIHeader(input);
            Assert.AreEqual(expectedResult, actual);
        }


        /// <summary>
        /// Tests <see cref="EDIHelper.NormalizeEDIHeader"/>
        /// </summary>
        [TestMethod]
        public void TestNormalizeEdiHeader()
        {
        }
    }
}