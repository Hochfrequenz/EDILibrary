using System.Collections.Generic;
using AwesomeAssertions;
using EDILibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDILibraryTests;

/// <summary>
/// Tests <see cref="GenericEDIWriter"/> using a synthetic, non-proprietary create.template.
/// These templates use the writer's own "&lt;...&gt;" micro-templating syntax and have no
/// resemblance to any real Hochfrequenz Message Implementation Guide.
/// </summary>
[TestClass]
public class GenericEDIWriterTests
{
    private static EdiObject CreateDocument(string key = "DOC1")
    {
        var doc = new EdiObject("Dokument", null, key);
        doc.Fields["Nachrichtenreferenz"] = new List<string> { "1" };
        doc.Fields["Belegnummer"] = new List<string> { "DOC123" };
        return doc;
    }

    [TestMethod]
    public void CompileTemplate_SubstitutesSimpleFields_AndKeepsUnaHeader()
    {
        var doc = CreateDocument();
        var template =
            "UNA:+.? 'UNH+<Nachrichtenreferenz>+TESTMSG:D:1:UN:1.0'\nBGM+380+<Belegnummer>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+1+TESTMSG:D:1:UN:1.0'BGM+380+DOC123'");
    }

    [TestMethod]
    public void CompileTemplate_ForeachOverChildren_RendersOneSegmentPerChild()
    {
        var doc = CreateDocument();
        var lin1 = new EdiObject("LIN", null, "1");
        lin1.Fields["Positionsnummer"] = new List<string> { "1" };
        lin1.Fields["Menge"] = new List<string> { "100" };
        var lin2 = new EdiObject("LIN", null, "2");
        lin2.Fields["Positionsnummer"] = new List<string> { "2" };
        lin2.Fields["Menge"] = new List<string> { "200" };
        doc.AddChild(lin1);
        doc.AddChild(lin2);

        var template =
            "UNA:+.? 'UNH+<Nachrichtenreferenz>+TESTMSG:D:1:UN:1.0'\n"
            + "BGM+380+<Belegnummer>'\n"
            + "<foreach LIN>LIN+<Positionsnummer>'\nQTY+220:<Menge>'\n</foreach LIN>\n"
            + "UNZ+1+<Nachrichtenreferenz>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result
            .Should()
            .Be(
                "UNA:+.? 'UNH+1+TESTMSG:D:1:UN:1.0'BGM+380+DOC123'"
                    + "LIN+1'QTY+220:100'"
                    + "LIN+2'QTY+220:200'"
                    + "UNZ+1+1'"
            );
    }

    [TestMethod]
    public void CompileTemplate_ForeachWithNoMatchingChildren_RendersNothing()
    {
        var doc = CreateDocument();
        var template =
            "UNA:+.? 'UNH+1'\n<foreach LIN>LIN+<Positionsnummer>'\n</foreach LIN>\nUNZ+1'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+1'UNZ+1'");
    }

    [TestMethod]
    public void CompileTemplate_ForeachOverRepeatedField_FallsBackToFieldValues()
    {
        var doc = CreateDocument();
        doc.Fields["Referenz"] = new List<string> { "A", "B", "C" };
        var template =
            "UNA:+.? 'UNH+1'\n<foreach Referenz>RFF+<Referenz>'\n</foreach Referenz>\nUNZ+1'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+1'RFF+A'RFF+B'RFF+C'UNZ+1'");
    }

    [TestMethod]
    public void CompileTemplate_IfWithPresentValue_RendersInnerContent()
    {
        var doc = CreateDocument();
        doc.Fields["Freitext"] = new List<string> { "Hinweis" };
        var template = "UNA:+.? 'UNH+1'\n<if Freitext>FTX+AAI+++<Freitext>'\n</if>\nUNZ+1'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+1'FTX+AAI+++Hinweis'UNZ+1'");
    }

    [TestMethod]
    public void CompileTemplate_IfWithMissingValue_OmitsInnerContent()
    {
        var doc = CreateDocument();
        var template = "UNA:+.? 'UNH+1'\n<if Freitext>FTX+AAI+++<Freitext>'\n</if>\nUNZ+1'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+1'UNZ+1'");
    }

    [TestMethod]
    public void CompileTemplate_NameKeyPlaceholder_UsesObjectKey_WhenNoKeyFieldPresent()
    {
        var doc = CreateDocument("MY-KEY");
        var template = "UNA:+.? 'UNH+<Dokument:Key>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+MY-KEY'");
    }

    [TestMethod]
    public void CompileTemplate_NameKeyPlaceholder_PrefersKeyField_OverObjectKey()
    {
        var doc = CreateDocument("MY-KEY");
        doc.Fields["Key"] = new List<string> { "FIELD-KEY" };
        var template = "UNA:+.? 'UNH+<Dokument:Key>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+FIELD-KEY'");
    }

    [TestMethod]
    public void CompileTemplate_EscapesEdifactSpecialCharacters()
    {
        var doc = CreateDocument();
        doc.Fields["Freitext"] = new List<string> { "A+B:C'D" };
        var template = "UNA:+.? 'FTX+AAI+++<Freitext>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'FTX+AAI+++A?+B?:C?'D'");
    }

    [TestMethod]
    public void CompileTemplate_EscapesTypographicApostropheVariants()
    {
        var doc = CreateDocument();
        doc.Fields["Freitext"] = new List<string> { "O’Brien ‘x’ ‛x’ ′x" };
        var template = "UNA:+.? 'FTX+AAI+++<Freitext>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'FTX+AAI+++O?'Brien ?'x?' ?'x?' ?'x'");
    }

    [TestMethod]
    public void CompileTemplate_SplitsFieldIntoEqualLengthParts()
    {
        var doc = CreateDocument();
        doc.Fields["Name"] = new List<string> { "ABCDE" };
        var template = "UNA:+.? 'NAD+MS+<Name[3,2]>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'NAD+MS+ABC:DE'");
    }

    /// <summary>
    /// The semicolon-separated "fixed field lengths" syntax (e.g. "[2;3]") is parsed into
    /// <c>fieldLengths</c>, but the surrounding splitting logic only runs when the (comma-syntax-only)
    /// <c>length</c> variable is set, which never happens on this branch. The value therefore passes
    /// through unsplit. This documents the current (seemingly unintended) behavior rather than the
    /// documented-by-comma-variant intent; not fixed here since this is a test-only change.
    /// </summary>
    [TestMethod]
    public void CompileTemplate_SemicolonLengthSyntax_IsCurrentlyANoOp()
    {
        var doc = CreateDocument();
        doc.Fields["Name"] = new List<string> { "ABCDE" };
        var template = "UNA:+.? 'NAD+MS+<Name[2;3]>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'NAD+MS+ABCDE'");
    }

    [TestMethod]
    public void CompileTemplate_DateFormat_UtcConversion()
    {
        var doc = CreateDocument();
        doc.Fields["Erstellungsdatum"] = new List<string> { "20240115120000" };
        var template = "UNA:+.? 'DTM+137:<date Erstellungsdatum;204>:204'";
        var writer = new GenericEDIWriter { helper = { LocalTimeZone = System.TimeZoneInfo.Utc } };

        var result = writer.CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'DTM+137:20240115120000:204'");
    }

    [TestMethod]
    public void CompileTemplate_DateFormatTag_PicksShortFormat_ForFourCharValue()
    {
        var doc = CreateDocument();
        doc.Fields["Geburtstag"] = new List<string> { "0102" };
        var template = "UNA:+.? 'DTM+329:0102:<dateformat Geburtstag>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'DTM+329:0102:106'");
    }

    [TestMethod]
    public void CompileTemplate_DateFormatTag_PicksLongFormat_ForLongerValue()
    {
        var doc = CreateDocument();
        doc.Fields["Erstellungsdatum"] = new List<string> { "20240115" };
        var template = "UNA:+.? 'DTM+137:20240115:<dateformat Erstellungsdatum>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'DTM+137:20240115:104'");
    }

    [TestMethod]
    public void CompileTemplate_ScriptPlaceholder_IsRemovedWithoutError()
    {
        var doc = CreateDocument();
        var template = "UNA:+.? 'UNH+1'\n<§ SomeVar SomeExpression>UNZ+1'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+1'UNZ+1'");
    }

    [TestMethod]
    public void CompileTemplate_CollapsesEmptyElement_BeforeGroupSeparator()
    {
        var doc = new EdiObject("Dokument", null, "DOC1");
        // Belegnummer intentionally left unset -> empty value, which should
        // collapse the trailing ":+" produced by "<Belegnummer>+..." into "+".
        var template = "UNA:+.? 'RFF+Z13:<Belegnummer>+NextValue'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'RFF+Z13+NextValue'");
    }
}
