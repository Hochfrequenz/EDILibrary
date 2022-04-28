// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace EDILibrary
{
    public class APERAKDescriptionAttribute : Attribute
    {
        public APERAKDescriptionAttribute(string desc)
        {
            Description = desc;
        }
        public string Description { get; set; }
    }
    public class EDIDescriptionAttribute : Attribute
    {
        public EDIDescriptionAttribute(string desc)
        {
            Description = desc;
        }
        public string Description { get; set; }
    }
    public class EDIEnumHelper
    {
        public static Dictionary<string, string> DescriptionMap;
        public static string GetDescription(string name)
        {
            if (DescriptionMap == null)
            {
                DescriptionMap = new Dictionary<string, string>();
                var fields = typeof(EDIEnums).GetRuntimeFields().ToArray();
                foreach (var field in fields)
                {
                    var att = field.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
                    if (att != null)
                    {
                        DescriptionMap[(att as DescriptionAttribute).Description] = field.Name;
                    }
                }
            }
            var attrs = (IEnumerable<Attribute>)
                typeof(EDIEnums).GetRuntimeField(DescriptionMap[name]).GetCustomAttributes(typeof(EDIDescriptionAttribute), false);
            //if (attrs != null) // Always true
            {
                return attrs.Any() ? ((EDIDescriptionAttribute)attrs.First()).Description : name;
            }
            //return name;

        }
        public static string GetAPERAKDescription(EDIEnums enumValue)
        {
            var name = enumValue.ToString();
            var attrs = (IEnumerable<Attribute>)
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(APERAKDescriptionAttribute), false);
            //if (attrs != null) // expression is always true
            {
                return attrs.Any() ? ((APERAKDescriptionAttribute)attrs.First()).Description : name;
            }
            /*
            attrs = (IEnumerable<Attribute>)
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attrs.Count() > 0 ? "FEHLER:" + ((DescriptionAttribute)attrs.First()).Description : "FEHLER:" + name;
            */
        }
        public static string GetEDIDescription(EDIEnums enumValue)
        {
            var name = enumValue.ToString();
            var attrs = (IEnumerable<Attribute>)
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(EDIDescriptionAttribute), false);
            //if (attrs != null) // always true
            {
                return attrs.Any() ? ((EDIDescriptionAttribute)attrs.First()).Description : name;
            }
            /*
            attrs = (IEnumerable<Attribute>)
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attrs.Any() ? ((DescriptionAttribute)attrs.First()).Description : name;
            */
        }
        public static string GetDescription(EDIEnums enumValue)
        {
            var name = enumValue.ToString();
            var attrs = (IEnumerable<Attribute>)
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attrs.Any() ? ((DescriptionAttribute)attrs.First()).Description : name;
        }
    }
    public enum EDIEnums
    {
        [Description("Dokument")]
        Dokument,
        [Description("Nachricht")]
        Nachricht,
        [Description("Kategorie")]
        Kategorie,
        [Description("Nachrichtenfunktion")]
        Nachrichtenfunktion,
        [Description("Nachrichtendatum")]
        Nachrichtendatum,
        [Description("Zeitzone")]
        Zeitzone,
        [Description("Gueltigkeit")]
        Gueltigkeit,
        [Description("Referenz Anfrage")]
        Referenz_Anfrage,
        [Description("Absender_NAD")]
        Absender_NAD,
        [Description("Empfaenger_NAD")]
        [APERAKDescription("MP-ID Empfänger")]
        Empfänger_NAD,
        [Description("Absender_Code_UNB")]
        Absender_Code_UNB,
        [Description("Absender")]
        Absender,
        [Description("Empfaenger_Code_UNB")]
        Empfänger_Code_UNB,
        [Description("Empfaenger")]
        Empfänger,
        [Description("Vorgang")]
        Vorgang,
        [Description("Syntaxfehler")]
        Syntaxfehler,
        [Description("Servicesegment")]
        Servicesegment,
        [Description("Segmentposition")]
        Segmentposition,
        [Description("Gruppenposition")]
        Gruppenposition,
        [Description("Nachrichtentyp")]
        Nachrichtentyp,
        [Description("Nachrichtenversion")]
        Nachrichtenversion,
        [Description("Freigabenummer")]
        Freigabenummer,
        [Description("Anwendungscode")]
        Anwendungscode,
        [Description("Aktion")]
        Aktion,
        [Description("Herausgeber")]
        Herausgeber,
        [Description("Antwortsegment")]
        Antwortsegment,
        [Description("Fehlersegment")]
        Fehlersegment,
        [Description("Fehlerelement")]
        Fehlerelement,
        [Description("Segment")]
        Segment,
        [Description("Segmentname")]
        Segmentname,
        [Description("Dokumentendatum")]
        Dokumentendatum,
        [Description("Dokumentenuhrzeit")]
        Dokumentenuhrzeit,
        [Description("Referenzdatum")]
        Referenzdatum,
        [Description("Status Identifikation")]
        Status_Identifikation,
        [Description("Absenderreferenz")]
        Absenderreferenz,
        [Description("Status")]
        Status,
        [Description("Referenz")]
        Referenz,
        [Description("Version")]
        Version
    }
}
