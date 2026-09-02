namespace EDILibraryTests;

/// <summary>
/// A minimal, entirely synthetic EDIFACT "MIG" (tree + class/field template + message) used to
/// exercise <see cref="EDILibrary.GenericEDILoader"/>, <see cref="EDILibrary.TemplateHelper"/> and
/// <see cref="EDILibrary.EdiJsonMapper"/> end-to-end. The segment structure and field selectors are
/// invented for testing purposes only and do not reproduce any real Hochfrequenz Message
/// Implementation Guide. "MSCONS" is used as the UNH message type identifier only because it is a
/// standard UN/EDIFACT identifier that <see cref="EDILibrary.EdifactFormat"/> can parse; the fields
/// mapped below have nothing to do with the real MSCONS format.
/// </summary>
internal static class SyntheticEdifactFixture
{
    /// <summary>
    /// Tree template: "/" has UNB, UNH and UNZ as direct (non-repeating) children; UNH itself
    /// (repeating per message, though only one occurrence is used here) has BGM, DTM and UNT as
    /// direct children.
    /// </summary>
    public const string TreeTemplate =
        "/:UNB[M;M],UNH[M;M],UNZ[M;M]\nUNH:BGM[M;M],DTM[M;M],UNT[M;M]";

    /// <summary>
    /// class/field XML template (parse direction): wrapped in an outer "Root" class so that
    /// <see cref="EDILibrary.TemplateHelper.ConvertToJSON"/> emits a "Dokument"-keyed mapping entry
    /// matching the "Dokument" wrapper that <see cref="EDILibrary.EdiObject.SerializeToJSON"/>
    /// always produces on the parse side. The "Dokument" class itself is anchored on the "UNH" tree
    /// node and exposes four flat fields.
    /// </summary>
    public const string XmlTemplate =
        "<class name=\"Root\">"
        + "<class name=\"Dokument\" ref=\"UNH\">"
        + "<field name=\"Nachrichtenreferenz\" ref=\"UNH:1:0\" />"
        + "<field name=\"Belegart\" ref=\"BGM:1:0\" />"
        + "<field name=\"Belegnummer\" ref=\"BGM:2:0\" />"
        + "<field name=\"Erstellungsdatum\" ref=\"DTM:1:1\" />"
        + "</class>"
        + "</class>";

    public const string FormatVersion = "1.0";

    /// <summary>A single, flat (non-repeating) synthetic message using standard EDIFACT envelope segments.</summary>
    public const string Edi =
        "UNA:+.? '\n"
        + "UNB+UNOC:3+SENDERID:500+RECEIVERID:500+240101:1200+REF1++TESTMSG'\n"
        + "UNH+1+MSCONS:D:1:UN:1.0'\n"
        + "BGM+380+DOC123'\n"
        + "DTM+137:202401011200:203'\n"
        + "UNT+4+1'\n"
        + "UNZ+1+REF1'";

    /// <summary>
    /// Variant of <see cref="TreeTemplate"/> that additionally nests a repeating "SG10" segment
    /// group (LIN + QTY) under UNH, to exercise the tree-copy/"dirty" repetition mechanism in
    /// <see cref="EDILibrary.TreeHelper"/>.
    /// </summary>
    public const string TreeTemplateWithRepeatingGroup =
        "/:UNB[M;M],UNH[M;M],UNZ[M;M]\nUNH:BGM[M;M],DTM[M;M],SG10[M;M],UNT[M;M]\nSG10:LIN[M;M],QTY[M;M]";

    /// <summary>
    /// class/field XML template variant with a repeating "Position" child class (anchored on
    /// "SG10") nested under "Dokument".
    /// </summary>
    public const string XmlTemplateWithRepeatingGroup =
        "<class name=\"Root\">"
        + "<class name=\"Dokument\" ref=\"UNH\">"
        + "<field name=\"Nachrichtenreferenz\" ref=\"UNH:1:0\" />"
        + "<field name=\"Belegart\" ref=\"BGM:1:0\" />"
        + "<field name=\"Belegnummer\" ref=\"BGM:2:0\" />"
        + "<field name=\"Erstellungsdatum\" ref=\"DTM:1:1\" />"
        + "<class name=\"Position\" ref=\"SG10\" key=\"LIN:1:0\">"
        + "<field name=\"Positionsnummer\" ref=\"LIN:1:0\" />"
        + "<field name=\"Menge\" ref=\"QTY:1:1\" />"
        + "</class>"
        + "</class>"
        + "</class>";

    /// <summary>A synthetic message with two repetitions of the "SG10" (LIN + QTY) group.</summary>
    public const string EdiWithRepeatingGroup =
        "UNA:+.? '\n"
        + "UNB+UNOC:3+SENDERID:500+RECEIVERID:500+240101:1200+REF1++TESTMSG'\n"
        + "UNH+1+MSCONS:D:1:UN:1.0'\n"
        + "BGM+380+DOC123'\n"
        + "DTM+137:202401011200:203'\n"
        + "LIN+1'\n"
        + "QTY+220:100'\n"
        + "LIN+2'\n"
        + "QTY+220:200'\n"
        + "UNT+8+1'\n"
        + "UNZ+1+REF1'";
}
