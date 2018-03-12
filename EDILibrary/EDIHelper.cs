// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace EDILibrary
{
    public class EDIPartner
    {
        public string ID;
        public string CodeList;
        public string Rolle;
        public override string ToString()
        {
            return ID;
        }
    }
    public class EDIFileInfo
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
            return String.Join("_", new List<string>() { Format, Referenz, Sender != null ? Sender.ToString() : "", Empfänger != null ? Empfänger.ToString() : "", DateTime.UtcNow.ToString("yyyyMMdd"), ID });
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
        public static string RemoveBOM(string edi)
        {
            if (edi[0] != 'U')
                return edi.Substring(1);
            else return edi;
        }
        public static string NormalizeEDIHeader(string edi)
        {


            if (edi == null)
                return null;
            edi = RemoveBOM(edi);
            string elementDelimiter = ":";
            string groupDelimiter = "+";
            string segmentDelimiter = "'";
            string decimalChar = ".";
            string escapeChar = "?";
            int UNAoffset = -1;
            if (edi.StartsWith("UNA"))
            {


                string UNA = edi.Substring(0, 9);
                UNAoffset = 8;
                elementDelimiter = UNA.Substring(3, 1);
                groupDelimiter = UNA.Substring(4, 1);
                decimalChar = UNA.Substring(5, 1);
                escapeChar = UNA.Substring(6, 1);
                segmentDelimiter = UNA.Substring(8, 1);
                if (segmentDelimiter == "\r")
                    segmentDelimiter = "\r\n";
            }
            string message = edi.Substring(UNAoffset + segmentDelimiter.Length, edi.Length - (UNAoffset + segmentDelimiter.Length));
            if (escapeChar != "?")
            {
                if (elementDelimiter != ":")
                {
                    message = message.Replace(escapeChar + ":", "?:");
                }
                if (groupDelimiter != "+")
                {
                    message = message.Replace(escapeChar + "+", "?+");
                }
                if (decimalChar != ".")
                {
                    message = message.Replace(escapeChar + ".", "?.");
                }
            }
            if (decimalChar != ".")
            {
                message = message.Replace(decimalChar, ".");
            }

            return "UNA:+.? '" + message;
        }
        public static EDIFileInfo GetEDIFileInfo(string edi)
        {
            if (edi == null)
                return null;
            try
            {
                edi = RemoveBOM(edi);
                string elementDelimiter = ":";
                string groupDelimiter = "+";
                string segmentDelimiter = "'";
                int UNAoffset = -1;
                int segDelimiterLength = 1;
                if (edi.StartsWith("UNA"))
                {


                    string UNA = edi.Substring(0, 9);
                    UNAoffset = 8;
                    elementDelimiter = UNA.Substring(3, 1);
                    groupDelimiter = UNA.Substring(4, 1);
                    segmentDelimiter = UNA.Substring(8, 1);
                    segDelimiterLength = segmentDelimiter.Length;
                    if (segmentDelimiter == "\r" && edi.IndexOf("\r\n") > -1)
                        segmentDelimiter = "\r\n";
                }

                string message = edi.Substring(UNAoffset + segDelimiterLength, edi.Length - (UNAoffset + segDelimiterLength));
                String[] Segments = message.LowMemSplit(segmentDelimiter).Take(2).ToArray();
                string UNB = Segments[0];
                string UNH = Segments[1];
                string[] UNBParts = UNB.Split(groupDelimiter.ToCharArray());
                string[] UNHParts = UNH.Split(groupDelimiter.ToCharArray());

                EDIPartner sender = new EDIPartner() { CodeList = UNBParts[2].Split(elementDelimiter.ToCharArray()).Count() > 1 ? UNBParts[2].Split(elementDelimiter.ToCharArray())[1] : "500", ID = UNBParts[2].Split(elementDelimiter.ToCharArray())[0] };
                EDIPartner empfänger = new EDIPartner() { CodeList = UNBParts[3].Split(elementDelimiter.ToCharArray()).Count() > 1 ? UNBParts[3].Split(elementDelimiter.ToCharArray())[1] : "500", ID = UNBParts[3].Split(elementDelimiter.ToCharArray())[0] };
                EDIFileInfo file = new EDIFileInfo()
                {
                    Empfänger = empfänger,
                    Sender = sender
                };
                if (UNBParts.Count() >= 7)
                    file.Referenz = UNBParts[7].Split(elementDelimiter.ToCharArray())[0];
                file.ID = UNBParts[5].Split(elementDelimiter.ToCharArray())[0];
                file.Format = UNHParts[2].Split(elementDelimiter.ToCharArray())[0];
                file.Version = UNHParts[2].Split(elementDelimiter.ToCharArray())[4];
                file.Freigabenummer = UNHParts[2].Split(elementDelimiter.ToCharArray())[2];
                file.Nachrichtenversion = UNHParts[2].Split(elementDelimiter.ToCharArray())[1];


                return file;
            }
            catch (Exception)
            {
                return new EDIFileInfo() { Format = "ERROR", Referenz = Guid.NewGuid().ToString() };
            }
        }

    }
}
