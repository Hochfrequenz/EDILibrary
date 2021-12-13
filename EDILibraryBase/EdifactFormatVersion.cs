using System;
using System.Linq;

namespace EDILibrary
{
    /// <summary>
    /// A EDIFACT Format is one of the different formats used in German market communication.
    /// Each Format uses a different Message Implementation Guide ("MIG"). EdifactFormats are stable over time.
    /// To model different versions of the same format use <see cref="EdifactFormatVersion"/>.
    /// </summary>
    public enum EdifactFormat
    {
        /// <summary>
        /// Multimodaler Statusbericht
        /// </summary>
        IFTSTA = 21,

        /// <summary>
        /// Prüfbericht
        /// </summary>
        INSRPT = 23,

        /// <summary>
        /// invoices
        /// </summary>
        INVOIC = 31,

        /// <summary>
        /// meter readings and energy amounts
        /// </summary>
        MSCONS = 13,

        /// <summary>
        /// Bestellung
        /// </summary>
        ORDERS = 17,

        /// <summary>
        /// Bestellantwort
        /// </summary>
        ORDRSP = 19,

        /// <summary>
        /// price catalogues
        /// </summary>
        PRICAT = 27,

        /// <summary>
        /// quotes
        /// </summary>
        QUOTES = 35,

        /// <summary>
        /// Zahlungsavis
        /// </summary>
        REMADV = 33,

        /// <summary>
        /// Anfrage
        /// </summary>
        REQOTE = 15,

        /// <summary>
        /// master data
        /// </summary>
        UTILMD = 11,
        /// <summary>
        /// master data
        /// </summary>
        CUSTOM = 99
    }

    /// <summary>
    /// contains helper methods to derive the <see cref="EdifactFormat"/> from e.g. the Prüfidentifikator
    /// </summary>
    public static class EdifactFormatHelper
    {
        /// <summary>
        /// get fact for a message class ID ("Prüfidentifikator")
        /// </summary>
        /// <param name="pruefidentifikator">prüfidentifikator, e.g. '11042'</param>
        /// <returns>the EdifactFormat, e.g. <see cref="EdifactFormat.UTILMD"/> or throws a NotImplementedException iff EdiFormat was found</returns>
        public static EdifactFormat FromPruefidentifikator(string pruefidentifikator)
        {
            if (string.IsNullOrWhiteSpace(pruefidentifikator))
                throw new ArgumentNullException(nameof(pruefidentifikator));
            foreach (EdifactFormat ef in Enum.GetValues(typeof(EdifactFormat)))
            {
                if (pruefidentifikator.StartsWith(((int)ef).ToString()))
                {
                    return ef;
                }
            }

            throw new NotImplementedException($"No matching EdiFormat found for Prüfidentifikator {pruefidentifikator}");
        }
    }

    /// <summary>
    /// A Format Version represents a validity period of a EDIFACT format
    /// </summary>
    public enum EdifactFormatVersion
    {
        /// <summary>
        /// format version valid since 2017-10-01
        /// </summary>
        FV1710,

        /// <summary>
        /// format version valid since 2019-04-01
        /// </summary>
        FV1904,

        /// <summary>
        /// format version valid since 2019-12-01 ("Mako2020")
        /// </summary>
        FV1912,

        /// <summary>
        /// format version valid since 2020-04-01
        /// </summary>
        FV2004,

        /// <summary>
        /// Format Version April 2021 (valid since 2021-04-01)
        /// </summary>
        FV2104,

        /// <summary>
        /// Format Version October 2021 (valid since 2021-10-01)
        /// </summary>
        FV2110,

        /// <summary>
        /// Format Version April 2022 (aka MaKo2022)
        /// </summary>
        FV2204
    }

    /// <summary>
    /// Extension methods for <see cref="EdifactFormatVersion"/>
    /// </summary>
    public static class EdifactFormatVersionExtension
    {
        /// <summary>
        /// returns the legacy "MM/YY" string for a <see cref="EdifactFormatVersion"/>.
        /// </summary>
        /// <remarks>It's called legacy because its unhandy (modelling enums with leading digits or slashes is complicated and unintuitive to use the month before the year.</remarks>
        /// <param name="edifactFormatVersion"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string ToLegacyVersionString(this EdifactFormatVersion edifactFormatVersion)
        {
            return edifactFormatVersion switch
            {
                EdifactFormatVersion.FV1710 => "10/17",
                EdifactFormatVersion.FV1904 => "04/19",
                EdifactFormatVersion.FV1912 => "12/19",
                EdifactFormatVersion.FV2004 => "04/20",
                EdifactFormatVersion.FV2104 => "04/21",
                EdifactFormatVersion.FV2110 => "10/21",
                EdifactFormatVersion.FV2204 => "04/22",
                _ => throw new NotImplementedException($"The legacy format for {edifactFormatVersion} is not yet implemented.")
            };
        }

        /// <summary>
        /// parses the <paramref name="legacyFormatString"/> as <see cref="EdifactFormatVersion"/>.
        /// If <paramref name="legacyFormatString"/> is null or white space, the current version is returned.
        /// </summary>
        /// <param name="legacyFormatString"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static EdifactFormatVersion ToEdifactFormatVersion(this string legacyFormatString)
        {
            if (string.IsNullOrWhiteSpace(legacyFormatString))
                return EdifactFormatVersionHelper.GetCurrent();
            foreach (var efv in Enum.GetValues(typeof(EdifactFormatVersion)).Cast<EdifactFormatVersion>())
            {
                if (legacyFormatString == efv.ToLegacyVersionString())
                {
                    return efv;
                }
            }

            if (Enum.TryParse<EdifactFormatVersion>(legacyFormatString, out var result))
            {
                // may we'll ever receive it in the new format. then transformer bee will be the last system to complain ;)
                return result;
            }

            throw new NotImplementedException($"The legacy format string '{legacyFormatString}' could not be mapped.");
        }
    }

    public static class EdifactFormatVersionHelper
    {
        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV1904"/>
        /// </summary>
        private static readonly DateTime Keydate1904 = new DateTime(2019, 3, 31, 22, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV1912"/>
        /// </summary>
        private static readonly DateTime Keydate1912 = new DateTime(2019, 11, 30, 23, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2004"/>
        /// </summary>
        private static readonly DateTime Keydate2004 = new DateTime(2020, 03, 31, 22, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2104"/>
        /// </summary>
        private static readonly DateTime Keydate2104 = new DateTime(2021, 03, 31, 22, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2110"/>
        /// </summary>
        private static readonly DateTime KeyDate2110 = new DateTime(2021, 09, 30, 22, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2110"/>
        /// </summary>
        private static readonly DateTime KeyDate2204 = new DateTime(2022, 03, 31, 22, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// returns the format version valid as of now.
        /// </summary>
        /// <returns>currently valid EdiFormatVersion (the oldest one implemented is <see cref="EdifactFormatVersion.FV1710"/></returns>
        public static EdifactFormatVersion GetCurrent()
        {
            var now = DateTime.UtcNow;
            if (now > KeyDate2204)
            {
                return EdifactFormatVersion.FV2204;
            }
            if (now > KeyDate2110)
            {
                return EdifactFormatVersion.FV2110;
            }
            if (now > Keydate2104)
            {
                return EdifactFormatVersion.FV2104;
            }
            if (now > Keydate2004)
            {
                return EdifactFormatVersion.FV2004;
            }
            if (now > Keydate1912)
            {
                return EdifactFormatVersion.FV1912;
            }
            if (now > Keydate1904)
            {
                return EdifactFormatVersion.FV1904;
            }
            return EdifactFormatVersion.FV1710;
        }
    }
}
