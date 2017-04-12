using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
namespace EDILibrary.Repositories
{
    public class APERAKDescriptionAttribute : Attribute
    {
        public APERAKDescriptionAttribute(string desc)
        {
            Description = desc;
        }
        public string Description { get; set; }
    }
    public class EDIDescriptionAttribute :Attribute
    {
        public EDIDescriptionAttribute(string desc)
        {
            Description = desc;
        }
        public string Description { get; set; }
    }
    public class EDIEnumHelper
    {
        public static Dictionary<string, string> DescriptionMap = null;
        public static string GetDescription(string name)
        {
            if (DescriptionMap == null)
            {
                DescriptionMap = new Dictionary<string, string>();
                FieldInfo[] fields = typeof(EDIEnums).GetRuntimeFields().ToArray();
                foreach (FieldInfo field in fields)
                {
                    object att = field.GetCustomAttributes(typeof(DescriptionAttribute),false).FirstOrDefault();
                    if (att != null)
                    {
                        DescriptionMap[(att as DescriptionAttribute).Description] = field.Name;
                    }
                }
            }
            object[] attrs =
                typeof(EDIEnums).GetRuntimeField(DescriptionMap[name]).GetCustomAttributes(typeof(EDIDescriptionAttribute), false).ToArray<object>();
            if (attrs != null)
            {
                return (attrs.Length > 0) ? ((EDIDescriptionAttribute)attrs[0]).Description : name;
            }
            return name;

        }
        public static string GetAPERAKDescription(EDIEnums enumValue)
        {
            string name = enumValue.ToString();
            object[] attrs =
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(APERAKDescriptionAttribute), false).ToArray<object>();
            if (attrs != null)
            {
                return (attrs.Length > 0) ? ((APERAKDescriptionAttribute)attrs[0]).Description : name;
            }
            else
            {
                attrs =
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(DescriptionAttribute), false).ToArray<object>();
                return (attrs.Length > 0) ? "FEHLER:"+((DescriptionAttribute)attrs[0]).Description : "FEHLER:"+name;
            }
        }
        public static string GetEDIDescription(EDIEnums enumValue)
        {
            string name = enumValue.ToString();
            object[] attrs =
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(EDIDescriptionAttribute), false).ToArray<object>();
            if (attrs != null)
            {
                return (attrs.Length > 0) ? ((EDIDescriptionAttribute)attrs[0]).Description : name;
            }
            else
            {
                attrs =
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(DescriptionAttribute), false).ToArray<object>();
                return (attrs.Length > 0) ? ((DescriptionAttribute)attrs[0]).Description : name;
            }
        }
        public static string GetDescription(EDIEnums enumValue)
        {
            string name = enumValue.ToString();
            object[] attrs =
                enumValue.GetType().GetRuntimeField(name).GetCustomAttributes(typeof(DescriptionAttribute), false).ToArray<object>();
            return (attrs.Length > 0) ? ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description : name;
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
        [Description("Leistung MSB_MDL")]
        Leistung_MSB_MDL,
        [Description("Ablesung Zaehler")]
        Ablesung_Zähler,
        [Description("Ablesung")]
        Ablesung,
        [Description("Ablesewert")]
        Ablesewert,
        [Description("Lieferdatum")]
        Lieferdatum,
        [Description("Uebergabedatum")]
        Übergabedatum,
        [Description("Vertragsbeginn")]
        Vertragsbeginn,
        [Description("Vertragsende")]
        Vertragsende,
        [Description("Änderung zum")]
        Änderung_zum,
        [Description("Ende zum (nächstmoeglichen Termin)")]
        Ende_zum_nächstmöglichen_Termin,
        [Description("Start Abrechnungsjahr (RLM)")]
        Start_Abrechnungsjahr,
        [Description("Nächste turnusmaeßige Ablesung")]
        Nächste_Turnusablesung,
        [Description("Turnusintervall")]
        Turnusintervall,
        [Description("Bilanzierungsbeginn")]
        Bilanzierungsbeginn,
        [Description("Bilanzierungsende")]
        Bilanzierungsende,
        [Description("Kuendigungsfrist")]
        Kündigungsfrist,
        [Description("Transaktionsgrund")]
        Transaktionsgrund,
        [Description("Antwortstatus")]
        Antwortstatus,
        [Description("Kontakt")]
        Kontakt,
        [Description("Konzessionsabgabe")]
        Konzessionsabgabe,
        [Description("Komplexe Messung")]
        Komplexe_Messung,
        [Description("Allgemeine Informationen")]
        Allgemeine_Informationen,
        [Description("Zusaetzliche Informationen")]
        Zusätzliche_Informationen,
        [Description("Information zum Zählerstand")]
        Informationen_zum_Zählerstand,
        [Description("Profilbeschreibung")]
        Profilbeschreibung,
        [Description("Netznutzungsvertrag")]
        Netznutzungsvertrag,
        [Description("Zahler der Netznutzung")]
        Zahler_der_Netznutzung,
        [Description("Beauftragung")]
        Beauftragung,
        [Description("Klimazone")]
        Klimazone,
        [Description("Temperaturmessstelle")]
        Temperaturmessstelle,
        [Description("Bilanzierungsgebiet")]
        Bilanzierungsgebiet,
        [Description("Regelzone")]
        Regelzone,
        [Description("Marktgebiet")]
        Marktgebiet,
        [Description("Bilanzkreis")]
        Bilanzkreis,
        [Description("Zaehlpunkt")]
        Zählpunkt,
        [Description("Sammelzaehlpunkt")]
        Sammelzählpunkt,
        [Description("Referenz Vorgangsnummer")]
        Referenz_Vorgangsnummer,
        [Description("Referenz Storno")]
        Referenz_Storno,
        [Description("korrigierter Zählpunkt")]
        korrigierter_Zählpunkt,
        [Description("Prozessschrittidentifikator")]
        Prozessschrittidentifikator,
        [Description("Version Summenzeitreihe")]
        Version_Summenzeitreihe,
        [Description("Zaehlernummer")]
        Zählernummer,
        [Description("Standardlastprofil (syn)")]
        Standardlastprofil_syn,
        [Description("Standardlastprofil (ana)")]
        Standardlastprofil_ana,
        [Description("Zaehlverfahren")]
        Zählverfahren,
        [Description("Spannungsebene Entnahme")]
        Spannungsebene_Entnahme,
        [Description("Druckebene Entnahme")]
        Druckebene_Entnahme,
        [Description("Spannungsebene Messung")]
        Spannungsebene_Messung,
        [Description("Betrag Konzessionsabgabe HT")]
        Betrag_Konzessionsabgabe_HT,
        [Description("Betrag Konzessionsabgabe NT")]
        Betrag_Konzessionsabgabe_NT,
        [Description("Konzessionsabgabe HT")]
        Konzessionsabgabe_HT,
        [Description("Konzessionsabgabe NT")]
        Konzessionsabgabe_NT,
        [Description("Kategorie Konzessionsabgabe HT")]
        Kategorie_Konzessionsabgabe_HT,
        [Description("Konzessionsabgabe NT")]
        Kategorie_Konzessionsabgabe_NT,
        [Description("Verbrauchsaufteilung")]
        Verbrauchsaufteilung,
        [Description("Profilschar")]
        Profilschar,
        [Description("Haushaltskunde")]
        Haushaltskunde,
        [Description("Kein Haushaltskunde")]
        Kein_Haushaltskunde,
        [Description("Gruppenzuordnung (GABi)")]
        Gruppenzuordnung_GABi,
        [Description("Verlustfaktor Trafo")]
        Verlustfaktor_Trafo,
        [Description("Zeitreihentyp (Z20)")]
        Zeitreihentyp_Z20,
        [Description("Zeitreihentyp (Z21)")]
        Zeitreihentyp_Z21,
        [Description("Zeitreihentyp (Z22)")]
        Zeitreihentyp_Z22,
        [Description("Zeitreihentyp (Z23)")]
        Zeitreihentyp_Z23,
        [Description("Zeitreihentyp (Z24)")]
        Zeitreihentyp_Z24,
        [Description("Zaehler")]
        Zähler,
        [Description("Zaehlwerk")]
        Zählwerk,
        [Description("Zaehlwerke")]
        Zählwerke,
        [Description("OBIS")]
        OBIS,
        [Description("Kennzeichnung")]
        Kennzeichnung,
        [Description("Schwachlast")]
        Schwachlast,
        [Description("Spezifische Arbeit")]
        Spezifische_Arbeit,
        [Description("Spezifische Arbeit_VDN")]
        Spezifische_Arbeit_VDN,
        [Description("Jahresverbrauchsprognose")]
        Jahresverbrauchsprognose,
        [Description("Kundenwert")]
        Kundenwert,
        [Description("Maximalleistung")]
        Maximalleistung,
        [Description("Tatsaechlich bilanzierte Energiemenge")]
        Tatsächlich_bilanzierte_Energiemenge,
        [Description("Zaehlertyp_Gas")]
        Zählertyp_Gas,
        [Description("Zaehlertyp")]
        Zählertyp,
        [Description("ID")]
        ID,
        [Description("Anbieter")]
        Anbieter,
        [Description("Priorität")]
        Priorität,
        [Description("Referenz")]
        Referenz,
        [Description("Version")]
        Version,
        [Description("Nummer")]
        Nummer,
        [Description("Profil")]
        Profil,
        [Description("Codeliste")]
        Codeliste,
        [Description("Normierungsfaktor")]
        Normierungsfaktor,
        [Description("Profiltyp")]
        Profiltyp,
        [Description("Melder")]
        Melder,
        [Description("Verfahren")]
        Verfahren,
        [Description("Ebene")]
        Ebene,
        [Description("Druckebene")]
        Druckebene,
        [Description("Gasqualität")]
        Gasqualitaet,
        [Description("Betrag")]
        Betrag,
        [Description("Aufteilung")]
        Aufteilung,
        [Description("Zuordnung")]
        Zuordnung,
        [Description("Code")]
        Code,
        [Description("Arbeit")]
        Arbeit,
        [Description("Prognose")]
        Prognose,
        [Description("Typ")]
        Typ,
        [Description("Tarifanzahl")]
        Tarifanzahl,
        [Description("Energierichtung")]
        Energierichtung,
        [Description("Wandler")]
        Wandler,
        [Description("Kommunikationseinrichtung")]
        Kommunikationseinrichtung,
        [Description("Einrichtung")]
        Einrichtung,
        [Description("Steuereinrichtung")]
        Steuereinrichtung,
        [Description("Messwerterfassung")]
        Messwerterfassung,
        [Description("Erfassung")]
        Erfassung,
        [Description("Kunde")]
        Kunde,
        [Description("Name und Adresse für die Ablesekarte")]
        Ablesekarte,
        [Description("Nachname/Firma")]
        Nachname,
        [Description("Vorname")]
        Vorname,
        [Description("Titel")]
        Titel,
        [Description("Strasse")]
        Strasse,
        [Description("Hausnummer")]
        Hausnummer,
        [Description("Hausnummernzusatz")]
        Hausnummernzusatz,
        [Description("Ort")]
        Ort,
        [Description("PLZ")]
        PLZ,
        [Description("Land")]
        Land,
        [Description("Kundennummer_Lief")]
        Kundennummer_Lief,
        [Description("Kundennummer_Lief_Alt")]
        Kundennummer_Lief_Alt,
        [Description("Vorlieferant")]
        Vorlieferant,
        [Description("Messstellenbetreiber")]
        Messstellenbetreiber,
        [Description("Messdienstleister")]
        Messdienstleister,
        [Description("Grundzuständig")]
        Grundzuständig,
        [Description("LZR_Lieferant")]
        LZR_Lieferant,
        [Description("Lieferant")]
        Lieferant,
        [Description("NB (GABi)")]
        NB_GABi,
        [Description("Netzanschlusseigentuemer")]
        Netzanschlusseigentümer,
        [Description("Lieferanschrift")]
        Lieferanschrift,
        [Description("Abweichende Messstellenadresse")]
        Abweichende_Messstellenadresse,
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
        [Description("Anlass")]
        Anlass,
        [Description("Zuordnungsdatum")]
        Zuordnungsdatum,
        [Description("Start Störung")]
        Start_Störung,
        [Description("Ende Störung")]
        End_Störung,
        [Description("Lastprofilbezeichnung")]
        Lastprofilbezeichnung,
        [Description("Beginn Messperiode")]
        Beginn_Messperiode,
        [Description("Messperiodendauer")]
        Messperiodendauer,
        [Description("Ende Messperiode")]
        Ende_Messperiode,
        [Description("Bilanzierungsmonat")]
        Bilanzierungsmonat,
        [Description("Versionsangabe")]
        Aggregationszeitpunkt,
        [Description("Erfassungszeitpunkt")]
        Erfassungszeitpunkt,
        [Description("Datum")]
        Datum,
        [Description("Datumsformat")]
        Datumsformat,
        [Description("Verantwortlicher")]
        Verantwortlicher,
        [Description("Ablesegrund")]
        Ablesegrund,
        [Description("Ablesedatum")]
        Ablesedatum,
        [Description("Parametereigenschaft")]
        Parametereigenschaft,
        [Description("Erfassungshinweis")]
        Erfassungshinweis,
        [Description("Menge")]
        Menge,
        [Description("Einheit")]
        Einheit,
        [Description("Status Vertrag")]
        Status_Vertrag,
        [Description("Messwertqualität")]
        Messwertqualität,
        [Description("Vorkommastellen")]
        Vorkommastellen,
        [Description("Nachkommastellen")]
        Nachkommastellen,
        [Description("Anzahl")]
        Anzahl,
        [Description("Zeitreihe")]
        Zeitreihe,
        [Description("Ausführungsdatum")]
        Ausführungsdatum,
        [Description("Verschobener Abmeldetermin")]
        Verschobener_Abmeldetermin,
        [Description("Gültigkeitszeitspanne")]
        Gültigkeitszeitspanne,
        [Description("Leistungsbeschreibung")]
        Leistungsbeschreibung,
        [Description("Angebotsnummer")]
        Angebotsnummer,
        [Description("Auftraggeber")]
        Auftraggeber,
        [Description("Währung")]
        Währung,
        [Description("Referenzwährung")]
        Referenzwährung,
        [Description("Position")]
        Position,
        [Description("Berechnungspreis")]
        Berechnungspreis,
        [Description("Referenznummer Angebot")]
        Referenznummer_Angebot,
        [Description("Gesamtbetrag")]
        Gesamtbetrag,
        [Description("Artikelnummer")]
        Artikelnummer,
        [Description("Auftragsnummer")]
        Auftragsnummer,
        [Description("Antwortkategorie")]
        Antwortkategorie,
        [Description("Referenznummer Bestellung")]
        Referenznummer_Bestellung,
        [Description("Baujahr")]
        Baujahr,
        [Description("Verkehrsjahr")]
        Verkehrsjahr,
        [Description("Größe")]
        Größe,
        [Description("Eichgültigkeit")]
        Eichgültigkeit,
        [Description("Serialnummer")]
        Serialnummer,
        [Description("Befestigungsart")]
        Befestigungsart,
        [Description("Preiskatalog")]
        Preiskatalog,
        [Description("Wandlerkonstante")]
        Wandlerkonstante,
        [Description("Dokumentenname")]
        Dokumentenname,
        [Description("Ansprechpartner")]
        Ansprechpartner,
        [Description("Kommunikationsverbindung")]
        Kommunikationsverbindung,
        [Description("Adresse")]
        Adresse,
        [Description("Leistungsdatum")]
        Leistungsdatum,
        [Description("Gerätestatus")]
        Gerätestatus,
        [Description("Fehlerbeschreibung")]
        Fehlerbeschreibung,
        [Description("Begründung")]
        Begründung,
        [Description("Beginn Rechnungsperiode")]
        Beginn_Rechnungsperiode,
        [Description("Ende Rechnungsperiode")]
        Ende_Rechnungsperiode,
        [Description("Reverse Charge")]
        Reverse_Charge,
        [Description("UmsatzsteuerID")]
        UmsatzsteuerID,
        [Description("Zahlungsbedingungen")]
        Zahlungsbedingungen,
        [Description("Fälligkeitsdatum")]
        Fälligkeitsdatum,
        [Description("Zeitliche Mengenangabe")]
        Zeitliche_Mengenangabe,
        [Description("Zeiteinheit")]
        Zeiteinheit,
        [Description("Steuer")]
        Steuer,
        [Description("Satz")]
        Satz,
        [Description("Art")]
        Art,
        [Description("Zuschlagsart")]
        Zuschlagsart,
        [Description("Prozent")]
        Prozent,
        [Description("Vorausbezahlter Betrag")]
        Vorausbezahlter_Betrag,
        [Description("Fälliger Betrag")]
        Fälliger_Betrag,
        [Description("Rechnungsbetrag")]
        Rechnungsbetrag,
        [Description("Vorausbezahlte Steuern")]
        Vorausbezahlte_Steuern,
        [Description("Besteuerungsgrundlage")]
        Besteuerungsgrundlage,
        [Description("Steuerangabe")]
        Steuerangabe,
        [Description("Steuerbetrag")]
        Steuerbetrag,
        [Description("Geforderter Betrag")]
        Geforderter_Betrag,
        [Description("Überweisungsbetrag")]
        Überweisungsbetrag,
        [Description("Rechnungsdatum")]
        Rechnungsdatum,
        [Description("Rechnungsnummer")]
        Rechnungsnummer,
        [Description("Steuernummer")]
        Steuernummer,
        [Description("Zahlungsdatum")]
        Zahlungsdatum,
        [Description("Bankangaben")]
        Bankangaben,
        [Description("Kontonummer")]
        Kontonummer,
        [Description("Kontoinhaber")]
        Kontoinhaber,
        [Description("Bankkennung")]
        Bankkennung,
        [Description("Bankcode")]
        Bankcode,
        [Description("Bankname")]
        Bankname,
        [Description("Avis")]
        Avis,
        [Description("Turnusmäßige Ablesung")]
        Turnusablesung,
        [Description("Counter")]
        Counter,
        [Description("Referenzen")]
        Referenzen,
        [Description("Kündigung Kunde")]
        Kündigung_Kunde,
        [Description("Kündigung Lieferant")]
        Kündigung_Lieferant,
        [Description("Vorjahresverbrauch")]
        Vorjahresverbrauch,
        [Description("Energiemenge")]
        Energiemenge,
        [Description("Leistung")]
        Leistung,
        [Description("Antwortstatus AA")]
        Antwortstatus_AA,
        [Description("Sperre Lieferbeginn")]
        [EDIDescription("Lieferbeginndatum in Bearbeitung")]
        Sperre_Lieferbeginn,
        [Description("Nächste Anmeldung")]
        [EDIDescription("Datum für nächste Bearbeitung")]
        Nächste_Anmeldung,
        [Description("Schar")]
        Schar,
        [Description("Netzbetreiber")]
        Netzbetreiber,
        [Description("Faktor")]
        Faktor,
        [Description("Bezeichnung")]
        Bezeichnung,
        [Description("Zaehlpunktdaten")]
        Zaehlpunktdaten,
        [Description("Genauigkeit")]
        Genauigkeit,
        [Description("Zählpunkttyp")]
        Zählpunkttyp,
        [Description("Dokumentennummer")]
        Dokumentennummer,
        [EDIDescription("Beteiligter Markpartner")]
        [Description("Beteiligter Markpartner")]
        Beteiligter_Marktpartner,
        [Description("DateFormat")]
        DateFormat,
        [Description("Kündigungsfrist zu festem Datum")]
        KuendigungsfristBezugDatum,
        [Description("Kündigungsfrist zu bestimmtem Termin")]
        KuendigungsfristBezugTermin,
        [Description("Lieferrichtung")]
        Lieferrichtung,
        [Description("Transaktionsgrund (befristet)")]
        Transaktionsgrund_befristet,
        [Description("Gruppe")]
        Gruppe,
        [Description("Art Erzeugungsanlage")]
        Art_Erzeugunsanlage,
        [Description("Status Erzeugungsanlage")]
        Status_Erzeugunsanlage,
        [Description("ErstmaligeAblesung")]
        ErstmaligeAblesung,
        [Description("Klassentyp (ana)")]
        Klassentyp_ana,
        [Description("Klassentyp (syn)")]
        Klassentyp_syn,
        [Description("Anlagenleistung")]
        Anlagenleistung,
        [Description("Stromsteuer")]
        Stromsteuer,
        [Description("Konzessionsabgabedaten")]
        Konzessionsabgabedaten,
        [Description("Zuordnung Konzessionsabgabe")]
        Zuordnung_Konzessionsabgabe,
        [Description("Mengenumwerter")]
        Mengenumwerter,
        [Description("Bilanzierungsbeginn lang")]
        Bilanzierungsbeginn_lang,
        [Description("Bilanzierungsende lang")]
        Bilanzierungsende_lang,
        [Description("Kuendigungstermin")]
        Kuendigungstermin,
        [Description("Größe Gas")]
        Groesse_Gas,
        [Description("Prüfidentifikator")]
        Prüfidentifikator,
        [Description("Ortsangabe AHB Fehler")]
        Ortsangabe_AHB_Fehler,
        [Description("Beschreibung")]
        Beschreibung,
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
        [Description("Gemeinderabatt")]
        Gemeinderabatt,
        [Description("Rabatt")]
        Rabatt,
        [Description("Gueltigkeit")]
        Gültigkeit,
        [Description("Nachrichtennummer")]
        Nachrichtennummer,
        [Description("Identifikationsnummer")]
        Identifikationsnummer,
        [Description("Energetische Mengenangabe")]
        Energetische_Mengenangabe,
        [Description("Bereits ausgetauschter Zählpunkttyp")]
        Bereits_ausgetauschter_Zählpunkttyp,
        [Description("BGMKey")]
        BGMKey,
        [Description("Leistungsperiode")]
        Leistungsperiode,
        [Description("SmartmeterGateway")]
        SmartmeterGateway,
        [Description("Steuerbox")]
        Steuerbox,
        [Description("Administrator")]
        Administrator,
        [Description("Name und Anschrift")]
        NameAnschrift,
        [Description("Name des Beteiligten")]
        NameBeteiligter,
        [Description("Name des Beteiligten1")]
        NameBeteiligter1,
        [Description("Name des Beteiligten2")]
        NameBeteiligter2,
        [Description("Name des Beteiligten3")]
        NameBeteiligter3,
        [Description("Name des Beteiligten4")]
        NameBeteiligter4,
        [Description("Strasse und Hausnummer oder Postfach")]
        StrasseHausnummerPostfach,
        [Description("Strasse und Hausnummer oder Postfach1")]
        StrasseHausnummerPostfach1,
        [Description("Strasse und Hausnummer oder Postfach2")]
        StrasseHausnummerPostfach2,
        [Description("Strasse und Hausnummer oder Postfach3")]
        StrasseHausnummerPostfach3,
        [Description("Korrespondenzanschrift des Endeverbrauchers/Kunden")]
        Korrespondenzanschrift,
    }
}
