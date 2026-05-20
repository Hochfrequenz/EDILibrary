using AwesomeAssertions;
using EDILibrary.Constants.German;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDILibraryTests
{
    [TestClass]
    public class PruefidentifikatorenTests
    {
        [TestMethod]
        public void TestMissingFv2610Pruefidentifikatoren()
        {
            Pruefidentifikatoren.UTILMD_44183_WimGas_EndeMsbVonNb.Should().Be("44183");
            Pruefidentifikatoren
                .UTILMD_55693_GpkeTeil4_AenderungDatenDerTrLfAnNb.Should()
                .Be("55693");
            Pruefidentifikatoren
                .UTILMD_55694_GpkeTeil4_Rueckmeldung_AnfrageDatenDerTrNbAnLf.Should()
                .Be("55694");
        }
    }
}
