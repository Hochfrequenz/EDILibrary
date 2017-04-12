using System;
using System.Linq;
using System.Net;
using System.Windows;

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDILibrary.Repositories
{
    [KnownType(typeof(List<AnswerStatus>))]
    public class AnswerStatus
    {
        public string Status_Code;
        public string Status_Text;
        public bool Zustimmung;
        public bool Freitext_noetig;
        public bool Daten_noetig = false;
        public AnswerStatus()
        {
            Zustimmung = false;
            Freitext_noetig = false;
            Daten_noetig = false;
            Status_Text = "";
            Status_Code = "";
        }
        public override string ToString()
        {
            return Status_Code + " - " + Status_Text;
        }
        public string FullText
        {
            get { return ToString(); }
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);

            if (obj.GetType() == typeof(AnswerStatus))
                return (obj as AnswerStatus).Status_Code == Status_Code;

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            if (Status_Code == null)
                return base.GetHashCode();
            return Status_Code.GetHashCode();
        }
    }
    public class AnswerStatusRepository
    {
        public static AnswerStatus GetAnswerStatusByCode(string mask, string code)
        {
            if (mask == null || mask == "")
            {
                var query = from AnswerStatus status in Get_Complete_Status_List() where status.Status_Code == code select status;
                return query.First();
            }
            else
            {
                var query = from AnswerStatus status in GetListByMask(mask) where status.Status_Code == code select status;
                return query.First();
            }

        }

        public static List<AnswerStatus> GetListByMask(string mask, bool includeNoSelection = false)
        {
            switch (mask)
            {
                case "Ablehnung Geschäftsdatenanfrage":
                case "Anforderung_Geschaeftsdaten":
                    {
                        var query = from AnswerStatus status in
                                        Get_ORDRSP_Status_List()
                                    where new List<string>() { "Z15", "Z21" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Antwort Stammdatenänderung":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                        //where new List<string>() { "E11", "E13", "E17", "Z07",  "Z09", "E14" }.Contains(status.Status_Code)
                                    where new List<string>() { "E13", "E17", "ZE2", "ZF9", "ZG1" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();

                    }
                case "Stammdatenänderung":
                case "Stammdatenaenderung":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                        //where new List<string>() { "E15", "E11", "E13", "E17", "Z07",  "Z09", "E14" ,"ZD3","ZD4"}.Contains(status.Status_Code)
                                    where new List<string>() { "E15", "E13", "E17", "ZE2", "ZF9", "ZG1" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();

                    }
                case "Stammdatenanfrage":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "ZD3", "ZG0", "ZG2", "ZG4" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Antwort Bestellung":
                    {
                        var query = from AnswerStatus status in
                                        Get_ORDRSP_Status_List()
                                    select status;
                        return query.ToList();
                    }
                case "REMADV":
                    {
                        var query = from AnswerStatus status in
                                        Get_REMADV_Status_List()
                                    select status;
                        return query.ToList();
                    }
                case "Reklamation":
                    {
                        var query = from AnswerStatus status in
                                        Get_REMADV_Status_List()
                                    //where new List<string>() { "5", "9", "28", "53", "Z01", "Z02", "Z03", "Z04", "Z05", "Z06", "Z07", "Z08", "Z10" }.Contains(status.Status_Code)
                                    where new List<string>() { "5", "9", "28", "53", "Z01", "Z02", "Z03", "Z04", "Z06", "Z07", "Z08", "Z10", "Z43", "Z35", "Z36", "Z37", "Z38", "Z39", "Z40", "Z41", "Z42", "Z44", "Z45" }.Contains(status.Status_Code) // FU 04/16
                                    select status;
                        return query.ToList();
                    }
                case "NichtIdentifizierung_INVOIC":
                    {
                        var query = from AnswerStatus status in
                                        Get_REMADV_Status_List()
                                    where new List<string>() { "14", "28", "Z09" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Störungsmeldung":
                    {
                        var query = from AnswerStatus status in
                                        Get_INSRPT_Status_List()

                                    select status;
                        return query.ToList();
                    }
                case "Stoerung":
                    {
                        var query = from AnswerStatus status in
                                        Get_INSRPT_Status_List()
                                    where new List<string>() { "Z29", "ZB8" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Ablehnung_Messstellenaenderung":
                    {
                        var query = from AnswerStatus status in
                                        Get_ORDRSP_Status_List()
                                    where new List<string>() { "Z15", "Z17", "Z18" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Antwort Anforderung Messstellenänderung":
                case "Messstellenaenderung":
                    {
                        var query = from AnswerStatus status in
                                        Get_ORDRSP_Status_List()
                                    where new List<string>() { "Z13", "Z14", "Z15", "Z17", "Z18" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Antwort Weiterverpflichtung":
                    {
                        var query = from AnswerStatus status in
                                        Get_ORDRSP_Status_List()
                                    where new List<string>() { "Z13", "Z14", "Z22" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "ORDRSP":
                    {
                        var query = from AnswerStatus status in
                                        Get_ORDRSP_Status_List()
                                    where new List<string>() { "Z13", "5", "Z32" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Kuendigung_MSB":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "Z12", "Z29", "Z34", "E11" }.Contains(status.Status_Code)
                                    //  where new List<string>()  {"Z29","Z07","Z09","E11" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Kuendigung_MDL":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "Z12", "Z29", "Z34" }.Contains(status.Status_Code)
                                    // where new List<string>() { "Z29", "Z07", "Z09" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Ablehnung_Kuendigung":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "Z29", "E14", "Z34" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Kuendigung":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "E15", "Z44", "Z01", "Z08", "Z09", "Z12", "Z14", "Z29", "Z34", "E14" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Zustimmung_Lieferbeginn":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "Z43", "Z44" }.Contains(status.Status_Code)
                                    select status;
                        if (includeNoSelection)
                        {
                            List<AnswerStatus> retList = new List<AnswerStatus>() { new AnswerStatus() { Status_Code = null, Status_Text = "keine Angabe" } };
                            retList.AddRange(query.ToList());
                            return retList;
                        }
                        return query.ToList();
                    }
                case "Ablehnung_Lieferbeginn":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "Z07", "Z08", "Z09", "Z14", "Z35", "ZC5", "E13", "E14", "ZE2" }.Contains(status.Status_Code)
                                    select status;
                        if (includeNoSelection)
                        {
                            List<AnswerStatus> retList = new List<AnswerStatus>() { new AnswerStatus() { Status_Code = null, Status_Text = "keine Angabe" } };
                            retList.AddRange(query.ToList());
                            return retList;
                        }
                        return query.ToList();
                    }
                case "Ablehnung_EoG":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "Z08", "Z09", "Z14", "E14", "Z30" }.Contains(status.Status_Code)
                                    select status;
                        if (includeNoSelection)
                        {
                            List<AnswerStatus> retList = new List<AnswerStatus>() { new AnswerStatus() { Status_Code = null, Status_Text = "keine Angabe" } };
                            retList.AddRange(query.ToList());
                            return retList;
                        }
                        return query.ToList();
                    }
                case "Ablehnung_Abmeldungsanfrage_ohneTermin":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "Z08", "Z09", "Z12", "Z14" }.Contains(status.Status_Code)
                                    select status;
                        if (includeNoSelection)
                        {
                            List<AnswerStatus> retList = new List<AnswerStatus>() { new AnswerStatus() { Status_Code = null, Status_Text = "keine Angabe" } };
                            retList.AddRange(query.ToList());
                            return retList;
                        }
                        return query.ToList();
                    }
                case "Ablehnung_Abmeldungsanfrage":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "Z08", "Z09", "Z12", "Z14" }.Contains(status.Status_Code)
                                    select status;
                        if (includeNoSelection)
                        {
                            List<AnswerStatus> retList = new List<AnswerStatus>() { new AnswerStatus() { Status_Code = null, Status_Text = "keine Angabe" } };
                            retList.AddRange(query.ToList());
                            return retList;
                        }
                        return query.ToList();
                    }
                case "Ablehnung_Lieferende":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "Z07", "Z08", "Z09", "Z14", "E14" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Ablehnung_Stilllegung":
                    {
                        var query = from AnswerStatus status in
                                        Get_Complete_Status_List()
                                    where new List<string>() { "Z07", "Z08", "Z09", "Z14", "E14" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
                case "Anforderung_Messwerte":
                    {
                        // "Z15" ist nicht mehr gültig
                        var query = from AnswerStatus status in
                                        Get_ORDRSP_Status_List()
                                    where new List<string>() { "Z19", "Z23", "Z24", "Z25", "Z26", "Z27", "Z28", "Z29", "Z30", "Z31", "ZD7", "ZD8" }.Contains(status.Status_Code)
                                    select status;
                        return query.ToList();
                    }
            };
            return null;
        }
        public static List<AnswerStatus> Get_REMADV_Status_List()
        {
            return new List<AnswerStatus>()
            {


                 new AnswerStatus(){Status_Code ="5", Status_Text="Preis/Rechenregel falsch", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="9", Status_Text="Falscher Abrechnungszeitraum", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="14", Status_Text="Unbekannter Zählpunkt", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="28", Status_Text="Sonstiges", Zustimmung = false, Freitext_noetig=true},
                 new AnswerStatus(){Status_Code ="53", Status_Text="doppelte Rechnung", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z01", Status_Text="Abrechnungsbeginn ungleich Vertragsbeginn", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z02", Status_Text="Abrechnungsende ungleich Vertragsende", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z03", Status_Text="Betrag der Abschlagsrechnung falsch", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z04", Status_Text="Vorausbezahlter Betrag falsch", Zustimmung = false},
                 //new AnswerStatus(){Status_Code ="Z05", Status_Text="Pflichtfelder nicht gefüllt", Zustimmung = false}, // entfallen FU04/16
                 new AnswerStatus(){ Status_Code="Z06", Status_Text="Artikel unbekannt", Zustimmung = false},
                 new AnswerStatus(){ Status_Code="Z07", Status_Text="Messwerte/Energiemengen fehlen", Zustimmung = false},
                 new AnswerStatus(){ Status_Code="Z08", Status_Text="Rechnungsnummer bereits erhalten", Zustimmung = false},
                 new AnswerStatus(){ Status_Code="Z09", Status_Text="Zählpunkt OK, Kunde nicht identifizierbar", Zustimmung = false},
                 new AnswerStatus(){ Status_Code="Z10", Status_Text="Messwerte/Energiemengen falsch", Zustimmung = false},
                 //new AnswerStatus(){ Status_Code="Z11", Status_Text="Artikel nicht vereinbart", Zustimmung = false}, // entfallen FU04/16
                 //neu FU04/16:
                 new AnswerStatus(){ Status_Code="Z43", Status_Text="ungültiges Rechnungsdatum", Zustimmung = false},
                 //neu FU04/16 MMMA:
                 new AnswerStatus(){Status_Code ="Z35", Status_Text="falscher Bilanzierungsbeginn", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z36", Status_Text="falsches Netznutzungsende", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z37", Status_Text="bilanzierte Menge fehlt", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z38", Status_Text="bilanzierte Menge falsch", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z39", Status_Text="Netznutzungsabrechnung fehlt", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z40", Status_Text="Reverse Charge Anwendung fehlt oder unzulässig", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z41", Status_Text="Allokationsliste fehlt", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z42", Status_Text="Mehr-Mindermenge falsch", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z44", Status_Text="Zeitintervall der bilanzierten Menge inkonsistent", Zustimmung = false},
                 new AnswerStatus(){Status_Code ="Z45", Status_Text="Rechnungsempfänger widerspricht der steuerrechtlichen Einschätzung des Rechnungsstellers", Zustimmung = false},


            };
        }
        public static List<AnswerStatus> Get_INSRPT_Status_List()
        {
            return new List<AnswerStatus>()
            {
                 new AnswerStatus(){Status_Code ="E15", Status_Text="Zustimmung ohne Korrekturen", Zustimmung = true},

                 new AnswerStatus(){Status_Code ="Z29", Status_Text="Ablehnung (kein Vertragsverhältnis)", Zustimmung = false},
                 new AnswerStatus(){ Status_Code="ZB8", Status_Text="keine Störung feststellbar", Zustimmung = false},


            };
        }
        public static List<AnswerStatus> Get_ORDRSP_Status_List()
        {
            return new List<AnswerStatus>()
            {
            new AnswerStatus(){ Status_Code="5", Status_Text="Preis/Rechenregel falsch", Zustimmung = false},
            new AnswerStatus(){Status_Code ="Z13", Status_Text="Zustimmung ohne Korrekturen", Zustimmung = true},
            new AnswerStatus(){Status_Code ="Z14", Status_Text="Zustimmung mit Terminänderung", Zustimmung = true},
            new AnswerStatus(){ Status_Code="Z15", Status_Text="Ablehnung keine Berechtigung", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z16", Status_Text="Ablehnung Marktpartner nicht zuständig", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z17", Status_Text="Ablehnung Änderung technisch nicht möglich", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z18", Status_Text="Ablehnung Umbau vertraglich nicht möglich (MSB <> MDL)", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z19", Status_Text="Ablehnung Unzulässiger Sollablesezeitpunkt", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z21", Status_Text="Ablehnung Daten nicht vorhanden", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z22", Status_Text="Ablehnung wegen Überschreiten des Weiterverpflichtungszeitraums", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z23", Status_Text="Messung gescheitert (kein Zugang)", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z24", Status_Text="Messung gescheitert (Kommunikationsstörung)", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z25", Status_Text="Messung gescheitert (Netzausfall)", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z26", Status_Text="Messung gescheitert (Spannungsausfall)", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z27", Status_Text="Messung gescheitert (Gerätewechsel)", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z28", Status_Text="Messung gescheitert (Kalibrierung)", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z29", Status_Text="Messung gescheitert (Gerät arbeitet außerhalb der Betriebsbedingungen)", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z30", Status_Text="Messung gescheitert (Messeinrichtung gestört/defekt)", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z31", Status_Text="Messung gescheitert (Unsicherheit Messung)", Zustimmung = false},
            new AnswerStatus(){ Status_Code="ZD7", Status_Text="In der Messstelle ist kein Gerät vorhanden", Zustimmung = false},
            new AnswerStatus(){ Status_Code="ZD8", Status_Text="Messstelle/Lokation real nicht auffindbar", Zustimmung = false},
            new AnswerStatus(){ Status_Code="Z32", Status_Text="Ablehnung Bestellumfang übersteigt Angebotsumfang", Zustimmung = false},
            };
        }
        public static List<AnswerStatus> Get_Complete_Status_List()
        {
            return new List<AnswerStatus>()
            {
            new AnswerStatus(){ Status_Code="E15", Status_Text="Zustimmung ohne Korrekturen", Zustimmung = true},
                new AnswerStatus(){ Status_Code="Z44", Status_Text="Zustimmung mit Korrektur von nicht bilanzierungsrel. Daten", Zustimmung = true},
                new AnswerStatus(){ Status_Code="Z01", Status_Text="Zustimmung mit Terminänderung", Zustimmung = true},
                new AnswerStatus(){ Status_Code="Z07", Status_Text="Ablehnung keine Berechtigung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z11", Status_Text="Ablehnung Termin fehlt", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z12", Status_Text="Ablehnung Vertragsbindung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="E17", Status_Text="Ablehnung wg. Fristüberschreitung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z09", Status_Text="Ablehnung Transaktionsgrund unplausibel", Zustimmung = false},
                new AnswerStatus(){ Status_Code="E11", Status_Text="Ablehnung (Messproblem)" , Zustimmung = false},
                new AnswerStatus(){ Status_Code="ZB4", Status_Text="Eigenausbau wird erfolgen", Zustimmung = true},
                new AnswerStatus(){ Status_Code="ZB5", Status_Text="Kein Eigenausbau des MSBA", Zustimmung = true},
                new AnswerStatus(){ Status_Code="ZB6", Status_Text="Erforderliche Versicherung fehlt in der Meldung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="ZB7", Status_Text="Angemeldeter Leistungsumfang ist nicht stimmig", Zustimmung = false},

                new AnswerStatus(){ Status_Code="ZC5", Status_Text="Ablehnung (andere Anmeldung in Bearbeitung)", Zustimmung = false, Daten_noetig = true},

                new AnswerStatus(){ Status_Code="Z43", Status_Text="Zustimmung mit Korrektur von bilanzierungsrel. Daten", Zustimmung = true},

                new AnswerStatus(){ Status_Code="E09", Status_Text="Ablehnung (Lieferadresse nicht im Verteilnetz)", Zustimmung = false},

                new AnswerStatus(){ Status_Code="Z08", Status_Text="Ablehnung Transaktion schon stattgefunden", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z14", Status_Text="Ablehnung Doppelmeldung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z13", Status_Text="Ablehnung Meldung nicht identifizierbar", Zustimmung = false},
                new AnswerStatus(){ Status_Code="E13", Status_Text="Ablehnung (Bilanzierungsproblem)", Zustimmung = false},
                
                
                //new AnswerStatus(){ Status_Code="E12", Status_Text="Ablehnung (unklares Lieferverhältnis)", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z10", Status_Text="Ablehnung Abmeldung fehlt", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z34", Status_Text="Ablehnung Mehrfachkündigung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z35", Status_Text="Ablehnung Zwangsabmeldung", Zustimmung = false},

                new AnswerStatus(){ Status_Code="E14", Status_Text="Ablehnung (Sonstiges)", Zustimmung = false,Freitext_noetig = true},

                new AnswerStatus(){ Status_Code="Z22", Status_Text="Ablehnung wegen Überschreiten des Weiterverpflichtungszeitraums", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="Z29", Status_Text="Ablehnung kein Vertragsverhältnis", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="Z30", Status_Text="Ablehnung kein Grund-/Ersatzversorgungsfall", Zustimmung = false,Freitext_noetig = false},
                //neu zum 01.10.2013
                //MaBiS
                new AnswerStatus(){ Status_Code="Z56", Status_Text="Ablehnung Deaktivierung, Messwerte vorhanden", Zustimmung = false,Freitext_noetig = false},
                //Stammdatenänderung
                new AnswerStatus(){ Status_Code="ZD3", Status_Text="Ablehnung Verantwortlicher hat nicht geantwortet", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZD4", Status_Text="Ablehnung Verantwortlicher hat Änderung abgelehnt", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZG1", Status_Text="Ablehnung Zählpunkttyp entspricht nicht dem Ausgetauschtem", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZF9", Status_Text="Ablehnung Zählverfahren entspricht nicht dem Ausgetauschten", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZG0", Status_Text="Ablehnung Stammdaten am Zählpunkt nicht vorhanden", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZG2", Status_Text="Gültiges Ergebnis nach der Datenprüfung", Zustimmung = true,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZG4", Status_Text="Frist nicht eingehalten, Übermittlung Datenstand", Zustimmung = true,Freitext_noetig = false},

                //Einspeiser
                new AnswerStatus(){ Status_Code="ZD5", Status_Text="Ablehnung untermonatlicher Wechseltermin", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZD6", Status_Text="Ablehnung unplausible Dateninhalte", Zustimmung = false,Freitext_noetig = false},
                //neu zum 01.10.2014
                new AnswerStatus(){ Status_Code="ZE1", Status_Text="Ablehnung zu stornierender Vorgang wurde schon beantwortet", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZE2", Status_Text="Ablehnung Kapazitätsproblem vorhanden", Zustimmung = false,Freitext_noetig = false},
            };
        }
        public static List<AnswerStatus> Get_Storno_Status_Valid()
        {
            return new List<AnswerStatus>()
            {
                new AnswerStatus(){ Status_Code="E15", Status_Text="Zustimmung ohne Korrekturen", Zustimmung = true},
                new AnswerStatus(){ Status_Code="E17", Status_Text="Ablehnung wg. Fristüberschreitung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z07", Status_Text="Ablehnung keine Berechtigung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z08", Status_Text="Ablehnung Transaktion schon stattgefunden", Zustimmung = false},
           //     new AnswerStatus(){ Status_Code="Z13", Status_Text="Ablehnung Meldung nicht identifizierbar", Zustimmung = false}, // entfällt durch Formatwechsel 04/15 
                new AnswerStatus(){ Status_Code="Z14", Status_Text="Ablehnung Doppelmeldung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="E14", Status_Text="Ablehnung (Sonstiges)", Zustimmung = false,Freitext_noetig = true},
                new AnswerStatus(){ Status_Code="ZE1", Status_Text="Ablehnung zu stornierender Vorgang wurde schon beantwortet", Zustimmung = false,Freitext_noetig = false},
            };
        }
        public static List<AnswerStatus> Get_UTILMD_GPKE_VNB_E01_Status_Valid()
        {
            return new List<AnswerStatus>()
            {
                new AnswerStatus(){ Status_Code="E15", Status_Text="Zustimmung ohne Korrekturen", Zustimmung = true},
                new AnswerStatus(){ Status_Code="Z43", Status_Text="Zustimmung mit Korrektur von bilanzierungsrel. Daten", Zustimmung = true},
                new AnswerStatus(){ Status_Code="Z44", Status_Text="Zustimmung mit Korrektur von nicht bilanzierungsrel. Daten", Zustimmung = true},
                new AnswerStatus(){ Status_Code="Z01", Status_Text="Zustimmung mit Terminänderung", Zustimmung = true},
                new AnswerStatus(){ Status_Code="E09", Status_Text="Ablehnung (Lieferadresse nicht im Verteilnetz)", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z07", Status_Text="Ablehnung keine Berechtigung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z08", Status_Text="Ablehnung Transaktion schon stattgefunden", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z14", Status_Text="Ablehnung Doppelmeldung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="E13", Status_Text="Ablehnung (Bilanzierungsproblem)", Zustimmung = false},
                //new AnswerStatus(){ Status_Code="Z11", Status_Text="Ablehnung Termin fehlt", Zustimmung = false},
                new AnswerStatus(){ Status_Code="E17", Status_Text="Ablehnung wg. Fristüberschreitung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z09", Status_Text="Ablehnung Transaktionsgrund unplausibel", Zustimmung = false},
                //new AnswerStatus(){ Status_Code="E12", Status_Text="Ablehnung (unklares Lieferverhältnis)", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z10", Status_Text="Ablehnung Abmeldung fehlt", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z35", Status_Text="Ablehnung Zwangsabmeldung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="E11", Status_Text="Ablehnung (Messproblem)", Zustimmung = false},
                new AnswerStatus(){ Status_Code="E14", Status_Text="Ablehnung (Sonstiges)", Zustimmung = false,Freitext_noetig = true},
                new AnswerStatus(){ Status_Code="ZE2", Status_Text="Ablehnung Kapazitätsproblem vorhanden", Zustimmung = false,Freitext_noetig = false},
                // Stammdatenänderung
                /*new AnswerStatus(){ Status_Code="ZG1", Status_Text="Ablehnung Zählpunkttyp entspricht nicht dem Ausgetauschtem", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZF9", Status_Text="Ablehnung Zählverfahren entspricht nicht dem Ausgetauschten", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZG0", Status_Text="Ablehnung Stammdaten am Zählpunkt nicht vorhanden", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZG2", Status_Text="Gültiges Ergebnis nach der Datenprüfung", Zustimmung = true,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZG4", Status_Text="Frist nicht eingehalten, Übermittlung Datenstand", Zustimmung = true,Freitext_noetig = false},
*/
            };
        }

        public static List<AnswerStatus> Get_UTILMD_WIM_VNB_E01_MSB_Status_Valid()
        {
            return new List<AnswerStatus>()
            {
                new AnswerStatus(){ Status_Code="E15", Status_Text="Zustimmung ohne Korrekturen", Zustimmung = true},
                new AnswerStatus(){ Status_Code="Z44", Status_Text="Zustimmung mit Korrektur von nicht bilanzierungsrel. Daten", Zustimmung = true},
                new AnswerStatus(){ Status_Code="Z01", Status_Text="Zustimmung mit Terminänderung", Zustimmung = true},
                //new AnswerStatus(){ Status_Code="Z07", Status_Text="Ablehnung keine Berechtigung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z11", Status_Text="Ablehnung Termin fehlt", Zustimmung = false},
                new AnswerStatus(){ Status_Code="E17", Status_Text="Ablehnung wg. Fristüberschreitung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="Z09", Status_Text="Ablehnung Transaktionsgrund unplausibel", Zustimmung = false},
                new AnswerStatus(){ Status_Code="E11", Status_Text="Ablehnung (Messproblem)", Zustimmung = false},
                new AnswerStatus(){ Status_Code="ZB6", Status_Text="Erforderliche Versicherung fehlt in der Meldung", Zustimmung = false},
                new AnswerStatus(){ Status_Code="ZB7", Status_Text="Angemeldeter Leistungsumfang ist nicht stimmig", Zustimmung = false},
                /*20121129: Einbindung Ablehnung wg. Doppelmeldung auf Wunsch Fr. Flamme (STAWAG)*/
                new AnswerStatus(){ Status_Code="Z14", Status_Text="Ablehnung Doppelmeldung", Zustimmung = false},
                 // Stammdatenänderung
                /*new AnswerStatus(){ Status_Code="ZG1", Status_Text="Ablehnung Zählpunkttyp entspricht nicht dem Ausgetauschtem", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZF9", Status_Text="Ablehnung Zählverfahren entspricht nicht dem Ausgetauschten", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZG0", Status_Text="Ablehnung Stammdaten am Zählpunkt nicht vorhanden", Zustimmung = false,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZG2", Status_Text="Gültiges Ergebnis nach der Datenprüfung", Zustimmung = true,Freitext_noetig = false},
                new AnswerStatus(){ Status_Code="ZG4", Status_Text="Frist nicht eingehalten, Übermittlung Datenstand", Zustimmung = true,Freitext_noetig = false},*/
            };
        }
    }
}
