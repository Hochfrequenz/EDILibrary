using System;
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

    /// <summary>
    /// A foreach block that matches nothing and sits at the very end of the template, followed only
    /// by whitespace, must render as nothing, leaving the segments before it intact.
    /// </summary>
    /// <remarks>
    /// This is the case where the foreach branch's <c>TrimEnd</c> shortens the template past the
    /// position the scan would resume at, which is why that position is clamped to the template
    /// length. Unlike
    /// <see cref="CompileTemplate_ForeachWithNoMatchingChildren_RendersNothing"/>, nothing follows
    /// the block here, so the trim actually cuts below the resume position.
    /// </remarks>
    [TestMethod]
    public void CompileTemplate_ForeachWithNoMatchesAtEndOfTemplate_StillRendersPrecedingSegments()
    {
        var doc = CreateDocument();
        var template =
            "UNA:+.? 'UNH+1'\n<foreach LIN>LIN+<Positionsnummer>'\n</foreach LIN>\n\r\n\t";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+1'");
    }

    /// <summary>
    /// A combining mark directly after a tag's closing "&gt;" must not stop the writer from finding
    /// that tag.
    /// </summary>
    /// <remarks>
    /// The structural scan looks for "&lt;", "&gt;" and "&lt;/foreach …&gt;" ordinally. A
    /// culture-sensitive search treats a delimiter followed by a combining mark as a single
    /// grapheme and fails to match it at all, which used to make this template throw
    /// <see cref="System.ArgumentOutOfRangeException"/>. Combining marks reach the template through
    /// field values, which are spliced into it as the render proceeds, so this is data-reachable
    /// rather than merely theoretical.
    /// </remarks>
    [TestMethod]
    public void CompileTemplate_ClosingTagFollowedByCombiningMark_IsStillFound()
    {
        var doc = CreateDocument();
        var lin1 = new EdiObject("LIN", null, "1");
        lin1.Fields["Positionsnummer"] = new List<string> { "1" };
        var lin2 = new EdiObject("LIN", null, "2");
        lin2.Fields["Positionsnummer"] = new List<string> { "2" };
        doc.AddChild(lin1);
        doc.AddChild(lin2);
        var template =
            "UNA:+.? 'UNH+1'<foreach LIN>LIN+<Positionsnummer>'</foreach LIN>\u0301BGM+<Belegnummer>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+1'LIN+1'LIN+2'\u0301BGM+DOC123'");
    }

    /// <summary>
    /// An ICU-ignorable character *inside* a closing tag makes that tag unfindable, and the render
    /// fails loudly instead of emitting corrupt EDIFACT.
    /// </summary>
    /// <remarks>
    /// This pins the other side of the ordinal trade-off and is deliberately not a "nicer"
    /// behaviour than before: a culture-sensitive search treats a soft hyphen as invisible and so
    /// still matched "&lt;/foreach LIN&gt;", which used to render but left a stray "&gt;" in the
    /// payload - silently malformed EDIFACT handed to a market partner. Ordinally the closer is not
    /// found and the splice throws. Failing loudly on malformed template text is preferable to
    /// shipping a corrupt message, but it IS a behaviour change; do not "fix" it by reverting the
    /// comparison to culture-sensitive.
    /// </remarks>
    [TestMethod]
    public void CompileTemplate_IgnorableCharacterInsideClosingTag_ThrowsRatherThanCorruptingOutput()
    {
        var doc = CreateDocument();
        var template =
            "UNA:+.? 'UNH+1'<foreach LIN>LIN+<Positionsnummer>'</\u00ADforeach LIN>BGM+<Belegnummer>'";

        var act = () => new GenericEDIWriter().CompileTemplate(template, doc);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// The "&lt;/if&gt;" closer behaves the same way as the foreach closer for an ignorable
    /// character inside it.
    /// </summary>
    [TestMethod]
    public void CompileTemplate_IgnorableCharacterInsideIfCloser_Throws()
    {
        var doc = CreateDocument();
        doc.Fields["Flag"] = new List<string> { "Y" };
        var template = "UNA:+.? 'UNH+1'<if Flag>X'</\u00ADif>BGM+<Belegnummer>'";

        var act = () => new GenericEDIWriter().CompileTemplate(template, doc);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// An "&lt;if&gt;" block with a present value renders its inner content and consumes its closer.
    /// </summary>
    /// <remarks>
    /// Plain ASCII, so this passes under either comparison - it is coverage for the "&lt;/if&gt;"
    /// closer lookup, which had none, not a probe of the ordinal change. The ordinal behaviour of
    /// that closer is pinned by
    /// <see cref="CompileTemplate_IgnorableCharacterInsideClosingTag_ThrowsRatherThanCorruptingOutput"/>.
    /// </remarks>
    [TestMethod]
    public void CompileTemplate_IfBlock_RendersInnerContentAndConsumesCloser()
    {
        var doc = CreateDocument();
        doc.Fields["Flag"] = new List<string> { "Y" };
        var template = "UNA:+.? 'UNH+1'<if Flag>X'</if>BGM+<Belegnummer>'";

        var result = new GenericEDIWriter().CompileTemplate(template, doc);

        result.Should().Be("UNA:+.? 'UNH+1'X'BGM+DOC123'");
    }

    /// <summary>
    /// "&lt;!SegmentCounter&gt;" counts segment terminators back to the last "UNH+", and
    /// "&lt;$MessageNumber&gt;" counts "UNH+" occurrences.
    /// </summary>
    /// <remarks>
    /// Neither had any coverage before, yet the "UNH+" anchor search is one of the sites this
    /// change makes ordinal - and its failure mode is a wrong number in the UNT segment rather than
    /// an exception, which a market partner rejects rather than crashing on. Note both counters in
    /// a template receive the value computed at the first one, because the branch replaces every
    /// occurrence at once; that is pre-existing behaviour, pinned here rather than endorsed.
    /// </remarks>
    [TestMethod]
    public void CompileTemplate_SegmentCounterAndMessageNumber_AreCounted()
    {
        var doc = CreateDocument();

        new GenericEDIWriter()
            .CompileTemplate("UNA:+.? 'UNH+1'BGM+<Belegnummer>'UNT+<!SegmentCounter>+1'", doc)
            .Should()
            .Be("UNA:+.? 'UNH+1'BGM+DOC123'UNT+3+1'");

        new GenericEDIWriter()
            .CompileTemplate("UNA:+.? 'UNH+1'BGM+a'UNH+2'BGM+b'UNZ+<$MessageNumber>'", doc)
            .Should()
            .Be("UNA:+.? 'UNH+1'BGM+a'UNH+2'BGM+b'UNZ+2'");
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
