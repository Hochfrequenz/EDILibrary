using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using EDILibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace EDILibraryTests;

/// <summary>
/// Tests <see cref="EdiJsonMapper.ParseToJsonWithTemplates"/> - the core EDIFACT-to-JSON parsing
/// entry point - using <see cref="SyntheticEdifactFixture"/>. The JSON mapping template is derived
/// from the same tree/XML template via <see cref="TemplateHelper.ConvertToJSON"/> so the fixture
/// stays internally consistent by construction.
/// </summary>
[TestClass]
public class EdiJsonMapperTests
{
    private static string BuildMappingJson(string xmlTemplate, string treeTemplate)
    {
        return new TemplateHelper().ConvertToJSON(
            xmlTemplate,
            treeTemplate,
            SyntheticEdifactFixture.FormatVersion
        );
    }

    [TestMethod]
    public async Task ParseToJsonWithTemplates_ProducesJsonWithExpectedFlatFields()
    {
        var mapper = new EdiJsonMapper(loader: null);

        var result = await mapper.ParseToJsonWithTemplates(
            SyntheticEdifactFixture.Edi,
            packageVersion: null,
            SyntheticEdifactFixture.XmlTemplate,
            SyntheticEdifactFixture.TreeTemplate,
            BuildMappingJson(
                SyntheticEdifactFixture.XmlTemplate,
                SyntheticEdifactFixture.TreeTemplate
            )
        );

        result.Format.Should().Be(EdifactFormat.MSCONS);
        result.Sender.Should().Be("SENDERID");
        result.Receiver.Should().Be("RECEIVERID");

        var doc = JObject.Parse(result.EDI)["Dokument"]!.First!;
        doc["Nachrichtenreferenz"]!.Value<string>().Should().Be("1");
        doc["Belegart"]!.Value<string>().Should().Be("380");
        doc["Belegnummer"]!.Value<string>().Should().Be("DOC123");
        doc["Erstellungsdatum"]!.Value<string>().Should().Be("202401011200");
    }

    [TestMethod]
    public async Task ParseToJsonWithTemplates_RepeatingSegmentGroup_ProducesOneEntryPerRepetition()
    {
        var mapper = new EdiJsonMapper(loader: null);
        var mappingJson = BuildMappingJson(
            SyntheticEdifactFixture.XmlTemplateWithRepeatingGroup,
            SyntheticEdifactFixture.TreeTemplateWithRepeatingGroup
        );

        var result = await mapper.ParseToJsonWithTemplates(
            SyntheticEdifactFixture.EdiWithRepeatingGroup,
            packageVersion: null,
            SyntheticEdifactFixture.XmlTemplateWithRepeatingGroup,
            SyntheticEdifactFixture.TreeTemplateWithRepeatingGroup,
            mappingJson
        );

        var doc = JObject.Parse(result.EDI)["Dokument"]!.First!;
        var positions = (JArray)doc["Position"]!;
        positions.Should().HaveCount(2);
        positions[0]["Positionsnummer"]!.Value<string>().Should().Be("1");
        positions[0]["Menge"]!.Value<string>().Should().Be("100");
        positions[1]["Positionsnummer"]!.Value<string>().Should().Be("2");
        positions[1]["Menge"]!.Value<string>().Should().Be("200");
    }

    /// <summary>
    /// The JSON produced for a message must list its properties in template order.
    /// </summary>
    /// <remarks>
    /// This test exists for the sake of performance work, not to cover a functional requirement.
    /// <see cref="EdiJsonMapper"/> used to build its result as an
    /// <see cref="System.Dynamic.ExpandoObject"/>, which enumerates in insertion order; that was
    /// replaced by <c>InsertionOrderedDictionary</c> because ExpandoObject dominated the cost of
    /// parsing wide messages. The bag's enumeration order is what determines the property order of
    /// the emitted JSON, so this pins that order down explicitly rather than leaving it to the
    /// replacement type's implementation details. It is green both before and after the
    /// optimisation - it measures nothing.
    /// </remarks>
    [TestMethod]
    public async Task ParseToJsonWithTemplates_EmitsPropertiesInTemplateOrder()
    {
        var mapper = new EdiJsonMapper(loader: null);

        var result = await mapper.ParseToJsonWithTemplates(
            SyntheticEdifactFixture.Edi,
            packageVersion: null,
            SyntheticEdifactFixture.XmlTemplate,
            SyntheticEdifactFixture.TreeTemplate,
            BuildMappingJson(
                SyntheticEdifactFixture.XmlTemplate,
                SyntheticEdifactFixture.TreeTemplate
            )
        );

        var dokument = JObject.Parse(result.EDI)["Dokument"]!.First as JObject;
        dokument.Should().NotBeNull();
        dokument!
            .Properties()
            .Select(property => property.Name)
            .Should()
            .Equal("Nachrichtenreferenz", "Belegart", "Belegnummer", "Erstellungsdatum");
    }
}
