using AwesomeAssertions;
using EDILibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDILibraryTests;

/// <summary>
/// Tests <see cref="GenericEDILoader"/> (tree parsing, EDIFACT segment matching and
/// template-driven field extraction) using <see cref="SyntheticEdifactFixture"/>.
/// </summary>
[TestClass]
public class GenericEDILoaderTests
{
    [TestMethod]
    public void LoadTree_BuildsExpectedHierarchy()
    {
        var loader = new GenericEDILoader();

        var tree = loader.LoadTree(SyntheticEdifactFixture.TreeTemplate);

        tree.Name.Should().Be("/");
        tree.FindElement("UNB", recursive: false).Should().NotBeNull();
        tree.FindElement("UNH", recursive: false).Should().NotBeNull();
        tree.FindElement("UNZ", recursive: false).Should().NotBeNull();
        var unh = tree.FindElement("UNH", recursive: false);
        unh!.Dirty.Should().BeTrue("UNH is always treated as a repeatable message container");
        unh.FindElement("BGM", recursive: false).Should().NotBeNull();
        unh.FindElement("DTM", recursive: false).Should().NotBeNull();
        unh.FindElement("UNT", recursive: false).Should().NotBeNull();
    }

    [TestMethod]
    public void LoadEDI_AttachesSegmentsToMatchingTreeNodes()
    {
        var loader = new GenericEDILoader();
        var tree = loader.LoadTree(SyntheticEdifactFixture.TreeTemplate);
        var normalized = EDIHelper.NormalizeEDIHeader(SyntheticEdifactFixture.Edi);

        loader.LoadEDI(normalized, tree);

        var unh = tree.FindElement("UNH", recursive: false)!;
        unh.Edi.Should().ContainSingle(s => s.StartsWith("UNH+1+MSCONS"));
        var bgm = unh.FindElement("BGM", recursive: false)!;
        bgm.Edi.Should().ContainSingle(s => s.StartsWith("BGM+380+DOC123"));
    }

    [TestMethod]
    public void LoadTemplateWithLoadedTree_ExtractsFlatFieldsFromSyntheticMessage()
    {
        var loader = new GenericEDILoader();
        var tree = loader.LoadTree(SyntheticEdifactFixture.TreeTemplate);
        var normalized = EDIHelper.NormalizeEDIHeader(SyntheticEdifactFixture.Edi);
        var ediTree = loader.LoadEDI(normalized, tree);
        new TreeHelper().RefreshDirtyFlags(tree);
        var template = loader.LoadTemplate(SyntheticEdifactFixture.XmlTemplate);

        var result = loader.LoadTemplateWithLoadedTree(template, ediTree);

        result.Name.Should().Be("Dokument");
        result.Field("Nachrichtenreferenz").Should().Be("1");
        result.Field("Belegart").Should().Be("380");
        result.Field("Belegnummer").Should().Be("DOC123");
        result.Field("Erstellungsdatum").Should().Be("202401011200");
    }

    [TestMethod]
    public void LoadTemplateWithLoadedTree_ExtractsAllRepetitionsOfSegmentGroup()
    {
        var loader = new GenericEDILoader();
        var tree = loader.LoadTree(SyntheticEdifactFixture.TreeTemplateWithRepeatingGroup);
        var normalized = EDIHelper.NormalizeEDIHeader(
            SyntheticEdifactFixture.EdiWithRepeatingGroup
        );
        var ediTree = loader.LoadEDI(normalized, tree);
        new TreeHelper().RefreshDirtyFlags(tree);
        var template = loader.LoadTemplate(SyntheticEdifactFixture.XmlTemplateWithRepeatingGroup);

        var result = loader.LoadTemplateWithLoadedTree(template, ediTree);

        var positions = result.Childs("Position");
        positions.Should().HaveCount(2);
        positions[0].Field("Positionsnummer").Should().Be("1");
        positions[0].Field("Menge").Should().Be("100");
        positions[1].Field("Positionsnummer").Should().Be("2");
        positions[1].Field("Menge").Should().Be("200");
    }
}
