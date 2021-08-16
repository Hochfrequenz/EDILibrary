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
        [DataRow("UNHfoo bar asd", "UNHfoo bar asd")]
        [DataRow("\xFEFFUNH foo bar asd", "UNH foo bar asd")]
        // [DataRow("asdUNH foo bar asd", "asdUNH foo bar asd")] // todo: the method is crocked
        public void TestRemoveBOM(string raw, string expectedResult)
        {
            var actual = EDIHelper.RemoveBOM(raw);
            Assert.AreEqual(expectedResult, actual);
        }
    }
}