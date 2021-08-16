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
                var actualLegacy = EDIHelper.RemoveBOM(raw);
                Assert.AreEqual(expectedResult, actualLegacy);
            }
        }
    }
}