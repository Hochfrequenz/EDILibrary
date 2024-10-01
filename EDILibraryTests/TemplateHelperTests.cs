using System.IO;
using EDILibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDILibraryTests;

[TestClass]
public class TemplateHelperTests
{
    [TestMethod]
    [DataRow("COMDIS1.0c.template", "1.0c")]
    [DataRow("UTILMDGG1.0a.template", "G1.0a")]
    [DataRow("UTILMDWG1.0a.template", "G1.0a")]
    public void Test_RetrieveFormatVersionFromInputFileName(
        string filename,
        string expectedFormatVersion
    )
    {
        var actualFormatVersion = TemplateHelper.RetrieveFormatVersionFromInputFileName(filename);
        Assert.AreEqual(expectedFormatVersion, actualFormatVersion);
    }

    [TestMethod]
    public void Test_RetrieveFormatVersionFromInputFileName_ForPaths()
    {
        var filepathAsString =
            $"Path{Path.DirectorySeparatorChar}To{Path.DirectorySeparatorChar}My{Path.DirectorySeparatorChar}Favourite{Path.DirectorySeparatorChar}Templates{Path.DirectorySeparatorChar}folder{Path.DirectorySeparatorChar}COMDIS1.0c.template";
        var actualFormatVersion = TemplateHelper.RetrieveFormatVersionFromInputFileName(
            filepathAsString
        );
        Assert.AreEqual("1.0c", actualFormatVersion);
    }
}
