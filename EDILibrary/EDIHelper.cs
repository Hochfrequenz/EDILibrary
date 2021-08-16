// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary
{
    /// <summary>
    /// an EDIFACT specific Market Partner / Marktteilnehmer
    /// </summary>
    public class EDIPartner : IEquatable<EDIPartner>
    {
        /// <summary>
        /// 13 digit ID
        /// </summary>
        public string ID;

        public string CodeList;
        public string Rolle;

        public override string ToString()
        {
            return ID;
        }

        public bool Equals(EDIPartner other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID == other.ID && CodeList == other.CodeList && Rolle == other.Rolle;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EDIPartner)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, CodeList, Rolle);
        }
    }

    public class EDIFileInfo : IEquatable<EDIFileInfo>
    {
        public string Version;
        public string Format;
        public EDIPartner Sender;
        public EDIPartner Empfänger;
        public string ID;
        public string Referenz;
        public string Freigabenummer;
        public string Nachrichtenversion;

        public override string ToString()
        {
            return string.Join("_",
                new List<string>
                {
                    Format, Referenz, Sender != null ? Sender.ToString() : "", Empfänger != null ? Empfänger.ToString() : "", DateTime.UtcNow.ToString("yyyyMMdd"), ID
                });
        }

        public bool Equals(EDIFileInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Version == other.Version && Format == other.Format && Equals(Sender, other.Sender) && Equals(Empfänger, other.Empfänger) && ID == other.ID &&
                   Referenz == other.Referenz && Freigabenummer == other.Freigabenummer && Nachrichtenversion == other.Nachrichtenversion;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EDIFileInfo)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Version, Format, Sender, Empfänger, ID, Referenz, Freigabenummer, Nachrichtenversion);
        }
    }

    public partial class EDIHelper
    {
        public class FormatNotSupportedException : Exception
        {
            protected EDIFileInfo _info;

            public FormatNotSupportedException(EDIFileInfo info)
            {
                _info = info;
            }

            public override string ToString()
            {
                return "Die Dateien zum Format " + _info.Format + " " + _info.Version + " konnten nicht geladen werden.";
            }
        }

        /// <summary>
        /// attempts to remove a leading byte order mark
        /// </summary>
        /// <remarks>actually removes the first character whenever it is not "U" :D</remarks>
        /// <param name="edi">an edifact string</param>
        /// <returns>sanitized string</returns>
        /// <see cref="RemoveByteOrderMark"/>
        [Obsolete("Use 'RemoveByteOrderMark' instead")]
        public static string RemoveBOM(string edi)
        {
            // for a better implementation use RemoveByteOrderMark
            if (edi[0] != 'U')
                return edi.Substring(1);
            return edi;
        }

        private static readonly string ByteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

        /// <summary>
        /// Removes a byte order mark from <paramref name="text"/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns>null if <paramref name="text"/> is null; BOM sanitized string other wise</returns>
        public static string RemoveByteOrderMark(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            if (text.StartsWith(ByteOrderMarkUtf8) && text[0] == ByteOrderMarkUtf8[0])
            {
                return text.Remove(0, ByteOrderMarkUtf8.Length);
            }

            return text;
        }

        private const string DefaultElementDelimiter = ":";
        private const string DefaultGroupDelimiter = "+";
        private const string DefaultSegmentDelimiter = "'";
        private const string DefaultDecimalChar = ".";
        private const string DefaultEscapeChar = "?";

        /// <summary>
        /// a record to hold information about edifact delimiters and separator chars
        /// </summary>
        internal class EdifactSpecialChars // todo use record in .net5
        {
            public string ElementDelimiter { get; set; }
            public string GroupDelimiter { get; set; }
            public string SegmentDelimiter { get; set; }
            public string DecimalChar { get; set; }
            public string EscapeChar { get; set; }
            public int UnaOffset { get; set; }
        }

        private static EdifactSpecialChars GetSpecialChars(string edi)
        {
            if (edi.StartsWith("UNA"))
            {
                var una = edi[..9];
                var segmentDelimiter = una.Substring(8, 1);
                return new EdifactSpecialChars
                {
                    UnaOffset = 8,
                    ElementDelimiter = una.Substring(3, 1),
                    GroupDelimiter = una.Substring(4, 1),
                    DecimalChar = una.Substring(5, 1),
                    EscapeChar = una.Substring(6, 1),
                    SegmentDelimiter = segmentDelimiter == "\r" ? Environment.NewLine : segmentDelimiter
                };
            }

            return new EdifactSpecialChars
            {
                UnaOffset = -1,
                ElementDelimiter = DefaultElementDelimiter,
                GroupDelimiter = DefaultGroupDelimiter,
                SegmentDelimiter = DefaultSegmentDelimiter,
                DecimalChar = DefaultDecimalChar,
                EscapeChar = DefaultEscapeChar
            };
        }

        private static string GetActualMessage(string edi, EdifactSpecialChars specialChars)
        {
            return edi.Substring(specialChars.UnaOffset + specialChars.SegmentDelimiter.Length, edi.Length - (specialChars.UnaOffset + specialChars.SegmentDelimiter.Length));
        }

        /// <summary>
        /// bring the <paramref name="edi"/> to a normalized format using the default delimiters
        /// </summary>
        /// <param name="edi"></param>
        /// <returns></returns>
        public static string NormalizeEDIHeader(string edi)
        {
            if (edi == null)
                return null;
            edi = RemoveBOM(edi);
            var specialChars = GetSpecialChars(edi);
            var message = GetActualMessage(edi, specialChars);
            if (specialChars.EscapeChar != DefaultEscapeChar)
            {
                if (specialChars.ElementDelimiter != DefaultElementDelimiter)
                {
                    message = message.Replace(specialChars.EscapeChar + ":", "?:");
                }

                if (specialChars.GroupDelimiter != DefaultGroupDelimiter)
                {
                    message = message.Replace(specialChars.EscapeChar + "+", "?+");
                }

                if (specialChars.DecimalChar != DefaultDecimalChar)
                {
                    message = message.Replace(specialChars.EscapeChar + ".", "?.");
                }
            }

            if (specialChars.DecimalChar != DefaultDecimalChar)
            {
                message = message.Replace(specialChars.DecimalChar, ".");
            }

            return $"UNA{DefaultElementDelimiter}{DefaultGroupDelimiter}{DefaultDecimalChar}{DefaultEscapeChar} {DefaultSegmentDelimiter}{message}";
        }

        public static EDIFileInfo GetEdiFileInfo(string edi)
        {
            if (edi == null)
                return null;
            try
            {
                edi = RemoveBOM(edi);
                var specialChars = GetSpecialChars(edi);
                var message = GetActualMessage(edi, specialChars);
                string[] segments = message.LowMemSplit(specialChars.SegmentDelimiter).Take(2).ToArray();
                string unb = segments[0];
                string unh = segments[1];
                string[] unbParts = unb.Split(specialChars.GroupDelimiter.ToCharArray());
                string[] unhParts = unh.Split(specialChars.GroupDelimiter.ToCharArray());

                EDIPartner sender = new EDIPartner
                {
                    CodeList =
                        unbParts[2].Split(specialChars.ElementDelimiter.ToCharArray()).Length > 1 ? unbParts[2].Split(specialChars.ElementDelimiter.ToCharArray())[1] : "500",
                    ID = unbParts[2].Split(specialChars.ElementDelimiter.ToCharArray())[0]
                };
                EDIPartner empfänger = new EDIPartner
                {
                    CodeList =
                        unbParts[3].Split(specialChars.ElementDelimiter.ToCharArray()).Length > 1 ? unbParts[3].Split(specialChars.ElementDelimiter.ToCharArray())[1] : "500",
                    ID = unbParts[3].Split(specialChars.ElementDelimiter.ToCharArray())[0]
                };
                EDIFileInfo file = new EDIFileInfo
                {
                    Empfänger = empfänger,
                    Sender = sender
                };
                if (unbParts.Length >= 7)
                    file.Referenz = unbParts[7].Split(specialChars.ElementDelimiter.ToCharArray())[0];
                file.ID = unbParts[5].Split(specialChars.ElementDelimiter.ToCharArray())[0];
                file.Format = unhParts[2].Split(specialChars.ElementDelimiter.ToCharArray())[0];
                file.Version = unhParts[2].Split(specialChars.ElementDelimiter.ToCharArray())[4];
                file.Freigabenummer = unhParts[2].Split(specialChars.ElementDelimiter.ToCharArray())[2];
                file.Nachrichtenversion = unhParts[2].Split(specialChars.ElementDelimiter.ToCharArray())[1];
                return file;
            }
            catch (Exception)
            {
                return new EDIFileInfo { Format = "ERROR", Referenz = Guid.NewGuid().ToString() };
            }
        }
    }
}
