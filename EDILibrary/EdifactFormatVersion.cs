using System;
using System.Collections.Generic;
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
        /// Handelsunstimmigkeit
        /// </summary>
        COMDIS = 29,

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
        /// Bestelländerung
        /// </summary>
        ORDCHG = 39,

        /// <summary>
        /// Bestellung
        /// </summary>
        ORDERS = 17,

        /// <summary>
        /// Bestellantwort
        /// </summary>
        ORDRSP = 19,

        /// <summary>
        /// Partnerstammdaten
        /// </summary>
        PARTIN = 37,

        /// <summary>
        /// price catalogues
        /// </summary>
        PRICAT = 27,

        /// <summary>
        /// quotes
        /// </summary>
        QUOTES = 15,

        /// <summary>
        /// Zahlungsavis
        /// </summary>
        REMADV = 33,

        /// <summary>
        /// Anfrage
        /// </summary>
        REQOTE = 35,

        /// <summary>
        /// master data
        /// </summary>
        UTILMD = 11,

        /// <summary>
        /// master data gas
        /// </summary>
        UTILMDG = 44,

        /// <summary>
        /// master data electricity/ strom
        /// </summary>
        UTILMDS = 55,

        /// <summary>
        /// Netznutzungszeiten-Nachricht
        /// </summary>
        UTILTS = 25,

        /// <summary>
        /// CONTRL-Nachrichten
        /// </summary>
        CONTRL = 91,

        /// <summary>
        /// APERAK-Messages
        /// </summary>
        APERAK = 92,

        /// <summary>
        /// Custom pruefis
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
        /// <param name="maskUTILMDX">is true, UTILMD is returned instead of UTILMDG bzw. UTILMDS</param>
        /// <returns>the EdifactFormat, e.g. <see cref="EdifactFormat.UTILMD"/> or throws a NotImplementedException iff EdiFormat was found</returns>
        public static EdifactFormat FromPruefidentifikator(string pruefidentifikator, bool maskUTILMDX = true)
        {
            if (string.IsNullOrWhiteSpace(pruefidentifikator))
            {
                throw new ArgumentNullException(nameof(pruefidentifikator));
            }

            foreach (EdifactFormat ef in Enum.GetValues(typeof(EdifactFormat)))
            {
                if (pruefidentifikator.StartsWith(((int)ef).ToString()))
                {
                    return maskUTILMDX && ef is EdifactFormat.UTILMDG or EdifactFormat.UTILMDS ? EdifactFormat.UTILMD : ef;
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
        /// Format Version October 2022 (aka MaKo2022)
        /// </summary>
        FV2210,
        /// <summary>
        /// Format Version April 2023
        /// </summary>
        FV2304,
        /// <summary>
        /// Format Version October 2023 (aka MaKo2023)
        /// </summary>
        FV2310,

        /// <summary>
        /// Format Version April 2024
        /// </summary>
        FV2404,
    }

    public class EdifactFormatVersionComparer : IComparer<EdifactFormatVersion>
    {
        public int Compare(EdifactFormatVersion x, EdifactFormatVersion y)
        {
            return ((int)x).CompareTo((int)y);
        }
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
                EdifactFormatVersion.FV2210 => "10/22",
                EdifactFormatVersion.FV2304 => "04/23",
                EdifactFormatVersion.FV2310 => "10/23",
                EdifactFormatVersion.FV2404 => "04/24",
                _ => throw new NotImplementedException($"The legacy format for {edifactFormatVersion} is not yet implemented.")
            };
        }

        /// <summary>
        /// Compares two edifact versions alphanumerically
        /// </summary>
        /// <param name="edifactFormatVersion"></param>
        /// <param name="compare"></param>
        /// <returns>-1 = smaller, 0 equal, 1 greater</returns>
        [Obsolete("Use the IComparer class " + nameof(EdifactFormatVersionComparer) + " instead")]
        public static int CompareToVersion(this EdifactFormatVersion edifactFormatVersion, EdifactFormatVersion compare)
        {
            return new EdifactFormatVersionComparer().Compare(edifactFormatVersion, compare);
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
            {
                new EdifactFormatVersionHelper().GetCurrent();
            }
#if NETSTANDARD2_1
            foreach (EdifactFormatVersion efv in Enum.GetValues(typeof(EdifactFormatVersion)).Cast<EdifactFormatVersion>())
            {
                if (legacyFormatString == efv.ToLegacyVersionString())
                {
                    return efv;
                }
            }
#else
            foreach (var efv in Enum.GetValues<EdifactFormatVersion>())
            {
                if (legacyFormatString == efv.ToLegacyVersionString())
                {
                    return efv;
                }
            }
#endif
            if (Enum.TryParse<EdifactFormatVersion>(legacyFormatString, out var result))
            {
                // may we'll ever receive it in the new format. then transformer bee will be the last system to complain ;)
                return result;
            }

            throw new NotImplementedException($"The legacy format string '{legacyFormatString}' could not be mapped.");
        }
    }

    public interface IEdifactFormatVersionProvider
    {
        /// <summary>
        /// returns the valid Edifact Format Version at the given <paramref name="dto"/>
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public EdifactFormatVersion GetFormatVersion(DateTimeOffset dto);

        /// <summary>
        /// returns the valid Edifact Format Version for a given format and version/>
        /// </summary>
        /// <param name="format">Edifact format to look for</param>
        /// <param name="version">the version of the specified format (e.g. 5.2c)</param>
        /// <returns></returns>
        public EdifactFormatVersion GetFormatVersion(EdifactFormat format, string version);

        /// <summary>
        /// returns the valid Edifact Format Version used now
        /// </summary>
        /// <returns></returns>
        public EdifactFormatVersion GetCurrent();
    }

    public class EdifactFormatVersionHelper : IEdifactFormatVersionProvider
    {
        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV1904"/>
        /// </summary>
        private static readonly DateTime Keydate1904 = new(2019, 3, 31, 22, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV1912"/>
        /// </summary>
        private static readonly DateTime Keydate1912 = new(2019, 11, 30, 23, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2004"/>
        /// </summary>
        private static readonly DateTime Keydate2004 = new(2020, 03, 31, 22, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2104"/>
        /// </summary>
        private static readonly DateTime Keydate2104 = new(2021, 03, 31, 22, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2110"/>
        /// </summary>
        private static readonly DateTime KeyDate2110 = new(2021, 09, 30, 22, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2210"/>
        /// </summary>
        private static readonly DateTime KeyDate2210 = new(2022, 09, 30, 22, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2304"/>
        /// </summary>
        private static readonly DateTime KeyDate2304 = new(2023, 03, 31, 22, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2310"/>
        /// </summary>
        private static readonly DateTime KeyDate2310 = new(2023, 09, 30, 22, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// validity date of <see cref="EdifactFormatVersion.FV2404"/>
        /// </summary>
        /// <remarks>Note that this is _not_ April 1st</remarks>
        private static readonly DateTime KeyDate2404 = new(2024, 04, 02, 22, 0, 0, DateTimeKind.Utc);
        public EdifactFormatVersion GetFormatVersion(DateTimeOffset keydate)
        {
            if (keydate >= KeyDate2404)
            {
                return EdifactFormatVersion.FV2404;
            }
            if (keydate >= KeyDate2310)
            {
                return EdifactFormatVersion.FV2310;
            }
            if (keydate >= KeyDate2304)
            {
                return EdifactFormatVersion.FV2304;
            }
            if (keydate >= KeyDate2210)
            {
                return EdifactFormatVersion.FV2210;
            }

            if (keydate >= KeyDate2110)
            {
                return EdifactFormatVersion.FV2110;
            }

            if (keydate >= Keydate2104)
            {
                return EdifactFormatVersion.FV2104;
            }

            if (keydate >= Keydate2004)
            {
                return EdifactFormatVersion.FV2004;
            }

            if (keydate >= Keydate1912)
            {
                return EdifactFormatVersion.FV1912;
            }

            if (keydate >= Keydate1904)
            {
                return EdifactFormatVersion.FV1904;
            }

            return EdifactFormatVersion.FV1710;
        }
        /// <summary>
        /// returns the format version valid as of now.
        /// </summary>
        /// <returns>currently valid EdiFormatVersion (the oldest one implemented is <see cref="EdifactFormatVersion.FV1710"/></returns>
        public EdifactFormatVersion GetCurrent()
        {
            var keydate = DateTime.UtcNow;
            return GetFormatVersion(keydate);
        }
        /// <summary>
        /// <see cref="IEdifactFormatVersionProvider.GetFormatVersion(EdifactFormat, string)"/>
        /// does not support anything older than 2110 at the moment
        /// </summary>
        /// <param name="format"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public EdifactFormatVersion GetFormatVersion(EdifactFormat format, string version)
        {
            EdifactFormatVersion determinedVersion = format switch
            {
                EdifactFormat.UTILMD => version switch
                {
                    "5.2e" => EdifactFormatVersion.FV2210,
                    "5.2c" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.UTILMDG => version switch
                {
                    "G1.0a" => EdifactFormatVersion.FV2310,
                    _ => GetCurrent()
                },
                EdifactFormat.UTILMDS => version switch
                {
                    "S1.1" => EdifactFormatVersion.FV2310,
                    _ => GetCurrent()
                },
                EdifactFormat.MSCONS => version switch
                {
                    "2.4b" => EdifactFormatVersion.FV2310,
                    "2.4a" => EdifactFormatVersion.FV2210,
                    "2.3c" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.PARTIN => version switch
                {
                    "1.0c" => EdifactFormatVersion.FV2310,
                    "1.0a" => EdifactFormatVersion.FV2210,
                    _ => GetCurrent()
                },
                EdifactFormat.IFTSTA => version switch
                {
                    "2.0e" => EdifactFormatVersion.FV2310,
                    "2.0d" => EdifactFormatVersion.FV2210,
                    "2.0c" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.APERAK => version switch
                {
                    "2.1h" => EdifactFormatVersion.FV2210,
                    "2.1f" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.COMDIS => version switch
                {
                    "1.0c" => EdifactFormatVersion.FV2310,
                    "1.0b" => EdifactFormatVersion.FV2210,
                    "1.0a" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.CONTRL => version switch
                {
                    "2.0b" => EdifactFormatVersion.FV2210,
                    "2.0a" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.INSRPT => version switch
                {
                    "1.1a" => EdifactFormatVersion.FV2210,
                    "1.1" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.INVOIC => version switch
                {
                    "2.8b" => EdifactFormatVersion.FV2310,
                    "2.8a" => EdifactFormatVersion.FV2304,
                    "2.8" => EdifactFormatVersion.FV2210,
                    "2.7a" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.ORDCHG => version switch
                {
                    "1.1" => EdifactFormatVersion.FV2310,
                    "1.0" => EdifactFormatVersion.FV2210,
                    _ => GetCurrent()
                },
                EdifactFormat.ORDERS => version switch
                {
                    "1.3" => EdifactFormatVersion.FV2310,
                    "1.2b" => EdifactFormatVersion.FV2304,
                    "1.2a" => EdifactFormatVersion.FV2210,
                    "1.1m" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.ORDRSP => version switch
                {
                    "1.3" => EdifactFormatVersion.FV2310,
                    "1.2b" => EdifactFormatVersion.FV2304,
                    "1.2a" => EdifactFormatVersion.FV2210,
                    "1.1k" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.PRICAT => version switch
                {
                    "2.0c" => EdifactFormatVersion.FV2310,
                    "2.0b" => EdifactFormatVersion.FV2304,
                    "2.0a" => EdifactFormatVersion.FV2210,
                    "1.1b" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.QUOTES => version switch
                {
                    "1.3" => EdifactFormatVersion.FV2310,
                    "1.2" => EdifactFormatVersion.FV2210,
                    "1.1b" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.REMADV => version switch
                {
                    "2.9b" => EdifactFormatVersion.FV2310,
                    "2.9a" => EdifactFormatVersion.FV2304,
                    "2.9" => EdifactFormatVersion.FV2210,
                    "2.8" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.REQOTE => version switch
                {
                    "1.3" => EdifactFormatVersion.FV2310,
                    "1.2" => EdifactFormatVersion.FV2210,
                    "1.1d" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                EdifactFormat.UTILTS => version switch
                {
                    "1.1b" => EdifactFormatVersion.FV2310,
                    "1.1a" => EdifactFormatVersion.FV2210,
                    "1.0a" => EdifactFormatVersion.FV2110,
                    _ => GetCurrent()
                },
                _ => GetCurrent()
            };
            return determinedVersion;
        }
    }
}
