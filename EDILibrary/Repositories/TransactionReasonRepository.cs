using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
    public class TransactionReason : BaseKeyValue
    {
    };
    public class TransactionReasonRepository
    {
        public static TransactionReason GetTransactionReasonByKey(string mask, string key)
        {
            return (from TransactionReason reason in GetTransactionReasons(mask) where reason.Key == key select reason).FirstOrDefault();
        }
        public static List<TransactionReason> GetTransactionReasons(string mask)
        {
            if (mask == null)
            {
                //komplette Liste
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "Z36",Value="EoG aus Ein-/Auszug (Umzug)"},
                    new TransactionReason(){ Key = "Z37",Value="EoG aus Einzug/Neuanlage"},
                    new TransactionReason(){ Key = "Z38",Value="EoG aus Wechsel"},
                    new TransactionReason(){ Key = "Z39",Value="EoG aus vorübergehendem Anschluss"},
                    new TransactionReason(){ Key = "ZC6",Value="EoG aus Bilanzkreisschließung"},
                    new TransactionReason(){ Key = "ZC7",Value="EoG aufgrund Erlöschung der Zuordnungsermächtigung"},
                    new TransactionReason(){ Key = "E06",Value="Ersatzbelieferung"},
                    new TransactionReason(){ Key = "E04",Value="Vorübergehender Anschluss"},
                    new TransactionReason(){ Key = "ZD2",Value="Lieferbeginn und Abmeldung aus der Ersatzversorgung"},
                    new TransactionReason(){ Key = "E01",Value="Umzug"},
                    new TransactionReason(){ Key = "E02",Value="Einzug"},
                    new TransactionReason(){ Key = "E03",Value="Wechsel"},
                    new TransactionReason(){ Key = "ZB1",Value="Ende der Messdienstleistung"},
                    new TransactionReason(){ Key = "Z40",Value="Geschäftsdatenanfrage"},
                    new TransactionReason(){ Key = "Z41",Value="Ende der ESV ohne Folgebelieferung"},
                    new TransactionReason(){ Key = "Z46",Value="Änderungsmitteilung nicht bilanzierungsrel. Daten"},
                    new TransactionReason(){ Key = "Z47",Value="Änderungsmitteilung von bilanzierungsrel. Daten"},
                    new TransactionReason(){ Key = "ZB2",Value="Gerätewechsel sämtlicher technischen Einrichtungen"},
                    new TransactionReason(){ Key = "ZB3",Value="Teilweiser Gerätewechsel"},
                    new TransactionReason(){ Key = "Z33",Value="Auszug/Stilllegung"},
                    new TransactionReason(){ Key = "Z26",Value="Information über existierende Zuordnung"},
                    new TransactionReason(){ Key = "ZC8",Value="Beendigung der Zuordnung"},
                    //new TransactionReason(){ Key = "ZC9",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 weggefallen
                    new TransactionReason(){ Key = "ZG9",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZH1",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZH2",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZE0",Value="Ablehnung Stammdaten richtig"},
                    new TransactionReason(){ Key = "ZE6 Nicht bila.rel. Änderung vom LF"},
                    new TransactionReason(){ Key = "ZE7",Value="Nicht bila.rel. Änderung vom NB"},
                    new TransactionReason(){ Key = "ZE8",Value="Änderung vom MSB"},
                    new TransactionReason(){ Key = "ZE9",Value="Bila.rel. Änderung vom LF"},
                    new TransactionReason(){ Key = "ZF0",Value="Bila.rel. Änderung vom NB mit Abhängigkeiten"},
                    new TransactionReason(){ Key = "ZF1",Value="Änderung des Zählverfahrens"},
                    new TransactionReason(){ Key = "ZF2",Value="Korrektur der Zählpunktbezeichnung vom NB"},
                    new TransactionReason(){ Key = "ZF3",Value="Nicht bila.rel. Anfrage an LF"},
                    new TransactionReason(){ Key = "ZF4",Value="Nicht bila.rel. Anfrage an NB"},
                    new TransactionReason(){ Key = "ZF5",Value="Anfrage an MSB mit Abhängigkeiten"},
                    new TransactionReason(){ Key = "ZF6",Value="Bila.rel. Anfrage an LF"},
                    new TransactionReason(){ Key = "ZF7",Value="Bila.rel. Anfrage an NB ohne Abhängigkeiten"},
                    new TransactionReason(){ Key = "ZF8",Value="Bila.rel. Anfrage an NB mit Abhängigkeiten"},
                    //neu FU 04/16:
                    new TransactionReason(){ Key = "ZG7",Value="nicht bilanzierungsrelevante Änderung vom MDL"},
                    new TransactionReason(){ Key = "ZG8",Value="nicht bilanzierungsrelevante Anfrage an den MDL"},
                };
            }
            else if (mask.Contains("EoG"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "Z36",Value="EoG aus Ein-/Auszug (Umzug)"},
                    new TransactionReason(){ Key = "Z37",Value="EoG aus Einzug/Neuanlage"},
                    new TransactionReason(){ Key = "Z38",Value="EoG aus Wechsel"},
                    new TransactionReason(){ Key = "Z39",Value="EoG aus vorübergehendem Anschluss"},
                    new TransactionReason(){ Key = "ZC6",Value="EoG aus Bilanzkreisschließung"},
                    new TransactionReason(){ Key = "ZC7",Value="EoG aufgrund Erlöschung der Zuordnungsermächtigung"},
                    new TransactionReason(){ Key = "E06",Value="Ersatzbelieferung"},

                };
            }
            if (mask.Contains("Anmeldung"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "E01",Value="Umzug"},
                    new TransactionReason(){ Key = "E02",Value="Einzug"},
                    new TransactionReason(){ Key = "E03",Value="Wechsel"},

                };
            }
            if (mask.Contains("LieferbeginnBefristet"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = null,Value="keine Befristung"},
                    new TransactionReason(){ Key = "E01",Value="Ein-/Auszug (Umzug)"},
                    new TransactionReason(){ Key = "E03",Value="Lieferantenwechsel"},
                    new TransactionReason(){ Key = "E04",Value="Vorübergehender Anschluss"},
                };
            }
            if (mask.Contains("Lieferbeginn"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "E01",Value="Umzug"},
                    new TransactionReason(){ Key = "E02",Value="Einzug"},
                    new TransactionReason(){ Key = "E03",Value="Wechsel"},
                    new TransactionReason(){ Key = "E04",Value="Vorübergehender Anschluss"},
                    new TransactionReason(){ Key = "ZD2",Value="Lieferbeginn und Abmeldung aus der Ersatzversorgung"}

                };
            }
            else if (mask.Contains("Abmeldung MDL"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "ZB1",Value="Ende der Messdienstleistung"},
                    //new TransactionReason(){ Key = "ZC9",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 weggefallen
                    new TransactionReason(){ Key = "ZG9",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZH1",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZH2",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                };
            }
            else if (mask.Contains("Kündigung"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "E03",Value="Wechsel"}
                };
            }
            else if (mask.Contains("Abmeldung MSB"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "E01",Value="Umzug"},
                    new TransactionReason(){ Key = "E03",Value="Wechsel"},
                    new TransactionReason(){ Key = "Z33",Value="Auszug/Stilllegung"},
                    //new TransactionReason(){ Key = "ZC9",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 weggefallen
                    new TransactionReason(){ Key = "ZG9",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZH1",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZH2",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                };
            }
            else if (mask.Contains("Abmeldungsanfrage"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "E01",Value="Ein-/Auszug (Umzug)"},
                    new TransactionReason(){ Key = "E03",Value="Lieferantenwechsel"},
                    new TransactionReason(){ Key = "ZD2",Value="Lieferbeginn und Abmeldung aus der Ersatzversorgung"}

                };
            }
            else if (mask.Contains("Informationsmeldung"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "Z26",Value="Information über existierende Zuordnung"},
                    new TransactionReason(){ Key = "ZC8",Value="Beendigung der Zuordnung"},
                    //new TransactionReason(){ Key = "ZC9",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 weggefallen
                    new TransactionReason(){ Key = "ZG9",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZH1",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZH2",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9

                };
            }
            else if (mask.Contains("Abmeldung"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "E01",Value="Ein-/Auszug (Umzug)"},
                    new TransactionReason(){ Key = "E03",Value="Lieferantenwechsel"},
                    new TransactionReason(){ Key = "E04",Value="Vorübergehender Anschluss"},
                    //new TransactionReason(){ Key = "Z33",Value="Auszug/Stilllegung"},
                    new TransactionReason(){ Key = "Z41",Value="Ende der ESV ohne Folgebelieferung"},
                    //new TransactionReason(){ Key = "ZC9",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 weggefallen
                    new TransactionReason(){ Key = "ZG9",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZH1",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                    new TransactionReason(){ Key = "ZH2",Value="Aufhebung einer zukünftigen Zuordnung"}, // FU1016 statt ZC9
                };
            }
            else if (mask.Contains("Stilllegung"))
            {
                return new List<TransactionReason>()
                {

                    new TransactionReason(){ Key = "Z33",Value="Auszug/Stilllegung"},

                };
            }
            else if (mask.Contains("Stammdatenänderung"))
            {
                return new List<TransactionReason>()
                {
                   // new TransactionReason(){ Key = "Z46",Value="Änderungsmitteilung nicht bilanzierungsrel. Daten"},
                  //  new TransactionReason(){ Key = "Z47",Value="Änderungsmitteilung von bilanzierungsrel. Daten"},
                    new TransactionReason(){ Key = "ZE6 Nicht bila.rel. Änderung vom LF"},
                    new TransactionReason(){ Key = "ZE7",Value="Nicht bila.rel. Änderung vom NB"},
                    new TransactionReason(){ Key = "ZE8",Value="Änderung vom MSB"},
                    new TransactionReason(){ Key = "ZE9",Value="Bila.rel. Änderung vom LF"},
                    new TransactionReason(){ Key = "ZF0",Value="Bila.rel. Änderung vom NB mit Abhängigkeiten"},
                    new TransactionReason(){ Key = "ZF1",Value="Änderung des Zählverfahrens"},
                    new TransactionReason(){ Key = "ZF2",Value="Korrektur der Zählpunktbezeichnung vom NB"},
                };
            }
            else if (mask.Contains("Stammdatenanfrage"))
            {
                // Stammdatenanfrage
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "ZF3",Value="Nicht bila.rel. Anfrage an LF"},
    new TransactionReason(){ Key = "ZF4",Value="Nicht bila.rel. Anfrage an NB"},
    new TransactionReason(){ Key = "ZF5",Value="Anfrage an MSB mit Abhängigkeiten"},
    new TransactionReason(){ Key = "ZF6",Value="Bila.rel. Anfrage an LF"},
    new TransactionReason(){ Key = "ZF7",Value="Bila.rel. Anfrage an NB ohne Abhängigkeiten"},
    new TransactionReason(){ Key = "ZF8",Value="Bila.rel. Anfrage an NB mit Abhängigkeiten"},
                };
            }
            else if (mask.Contains("Gerätewechsel"))
            {
                return new List<TransactionReason>()
                {
                    new TransactionReason(){ Key = "ZB2",Value="Gerätewechsel sämtlicher technischen Einrichtungen"},
                    new TransactionReason(){ Key = "ZB3",Value="Teilweiser Gerätewechsel"},
                };
            }

            return new List<TransactionReason>();
        }
    }
}
