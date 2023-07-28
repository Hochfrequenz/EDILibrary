using System.Text.Json;
using System.Text.Json.Serialization;
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

        private static readonly JsonSerializerOptions JsonFieldOptions = new()
        {
            IncludeFields = true,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>
        /// Tests <see cref="EDIHelper.GetEdiFileInfo"/>
        /// </summary>
        [TestMethod]
        [DataRow(null, null)]
        [DataRow("foo", "{\"Format\":null}")]
        [DataRow(
            "UNA:+.? 'UNB+UNOC:3+123456789012345:500+123456789:500+210326:1553+WIM00000000901'UNH+WIM00000000901+UTILMD:D:11A:UN:5.2b'BGM+E03+WIM00000000901'DTM+137:202103261553:203'NAD+MS+123456789012345::293'CTA+IC+:Max Mustermann'COM+max@mustermann.de:EM'NAD+MR+123456789::293'IDE+24+WIMP0000000459'DTM+92:20210401:102'DTM+157:20210401:102'STS+7++ZE8'LOC+172+41234567896'RFF+Z13:11116'SEQ+Z01'CCI+Z30++Z06'",
            "{\"Format\":\"UTILMD\", \"Nachrichtenversion\":\"D\",\"Version\":\"5.2b\", \"Sender\":{\"CodeList\":\"500\",\"ID\":\"123456789012345\"}, \"Freigabenummer\":\"11A\", \"Empfänger\":{\"CodeList\":\"500\",\"ID\":\"123456789\"}, \"ID\":\"WIM00000000901\"}")]
        [DataRow(
            "UNA:+.?'UNB+UNOC:3+123456789012345:500+123456789:500+210326:1553+WIM00000000901'UNH+WIM00000000901+UTILMD:D:11A:UN:5.2b'BGM+E03+WIM00000000901'DTM+137:202103261553:203'NAD+MS+123456789012345::293'CTA+IC+:Max Mustermann'COM+max@mustermann.de:EM'NAD+MR+123456789::293'IDE+24+WIMP0000000459'DTM+92:20210401:102'DTM+157:20210401:102'STS+7++ZE8'LOC+172+41234567896'RFF+Z13:11116'SEQ+Z01'CCI+Z30++Z06'",
            "{\"Format\":null}")] // wegen "UNA:+.?" ohne space vor "?"
        [DataRow(
            "UNA:+.? 'UNB+UNOC:3+123456789012345:500+123456789:500+210326:1553+WIM00000000901'UNH+WIM00000000901+UTILMD:D:11A:UN:S1.1'BGM+E03+WIM00000000901'DTM+137:202103261553:203'NAD+MS+123456789012345::293'CTA+IC+:Max Mustermann'COM+max@mustermann.de:EM'NAD+MR+123456789::293'IDE+24+WIMP0000000459'DTM+92:20210401:102'DTM+157:20210401:102'STS+7++ZE8'LOC+172+41234567896'RFF+Z13:11116'SEQ+Z01'CCI+Z30++Z06'",
            "{\"Format\":\"UTILMDS\", \"Nachrichtenversion\":\"D\",\"Version\":\"S1.1\", \"Sender\":{\"CodeList\":\"500\",\"ID\":\"123456789012345\"}, \"Freigabenummer\":\"11A\", \"Empfänger\":{\"CodeList\":\"500\",\"ID\":\"123456789\"}, \"ID\":\"WIM00000000901\"}")]

        public void TestGetEdiFileInfo(string input, string expectedOutput)
        {
            var actual = EDIHelper.GetEdiFileInfo(input);
            if (expectedOutput == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                var expected = JsonSerializer.Deserialize<EDIFileInfo>(expectedOutput, JsonFieldOptions);
                Assert.IsNotNull(expected);
                if (!expected.Format.HasValue)
                {
                    Assert.AreEqual(expected.Format, actual.Format);
                    Assert.IsTrue(System.Guid.TryParse(actual.Referenz, out _));
                }
                else
                {
                    Assert.AreEqual(expected, actual);
                }
            }
        }
    }
}
