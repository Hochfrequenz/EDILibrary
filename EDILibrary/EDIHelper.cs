using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using EDILibrary.Repositories;

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
        public static string GetPrüfindikator(EDIFileInfo edi_info, IEdiObject nachricht, IEdiObject vorgang)
        {
            string indikator = null;
            if (edi_info.Format == "UTILMD")
            {
                return vorgang.Child(EDIEnums.Prüfidentifikator).Key;
            }
            else if (edi_info.Format == "IFTSTA")
            {
                return vorgang.Child(EDIEnums.Status).Field(EDIEnums.Prüfidentifikator);
            }
            else if (edi_info.Format == "INSRPT")
            {
                return vorgang.Child(EDIEnums.Prüfidentifikator).Key;
            }
            else if (edi_info.Format == "REQOTE")
            {
                return nachricht.Parent.Child(EDIEnums.Prüfidentifikator).Key;
            }
            else if (edi_info.Format == "QUOTES")
            {
                return nachricht.Parent.Child(EDIEnums.Prüfidentifikator).Key;
            }
            else if (edi_info.Format == "CONTRL")
            {
                return null;
            }
            else if (edi_info.Format == "APERAK")
            {
                return null;
            }
            else if (edi_info.Format == "ORDERS")
            {
                return vorgang.Child(EDIEnums.Prüfidentifikator).Key;
            }
            else if (edi_info.Format == "ORDRSP")
            {
                return nachricht.Child(EDIEnums.Prüfidentifikator).Key;
            }
            else if (edi_info.Format == "MSCONS")
            {
                return nachricht.Child(EDIEnums.Prüfidentifikator).Key;
            }
            else if (edi_info.Format == "INVOIC")
            {
                return vorgang.Child(EDIEnums.Prüfidentifikator).Key;
            }
            else if (edi_info.Format == "REMADV")
            {
                return nachricht.Parent.Field(EDIEnums.Prüfidentifikator);
            }
            return indikator;
        }
        public static void SetPrüfindikator(EDIFileInfo edi_info, IEdiObject nachricht, IEdiObject vorgang, string identifikator)
        {

            if (edi_info.Format == "UTILMD")
            {
                IEdiObject prüf = new IEdiObject(EDIEnums.Prüfidentifikator, null, identifikator);
                prüf.AddField(EDIEnums.ID, identifikator);
                vorgang.AddChild(prüf);

            }
            else if (edi_info.Format == "IFTSTA")
            {
                vorgang.Child(EDIEnums.Status).AddField(EDIEnums.Prüfidentifikator, identifikator);

            }
            else if (edi_info.Format == "INSRPT")
            {
                IEdiObject prüf = new IEdiObject(EDIEnums.Prüfidentifikator, null, identifikator);
                prüf.AddField(EDIEnums.ID, identifikator);
                vorgang.AddChild(prüf);
            }
            else if (edi_info.Format == "REQOTE")
            {
                IEdiObject prüf = new IEdiObject(EDIEnums.Prüfidentifikator, null, identifikator);
                prüf.AddField(EDIEnums.ID, identifikator);
                nachricht.AddChild(prüf);
            }
            else if (edi_info.Format == "QUOTES")
            {
                IEdiObject prüf = new IEdiObject(EDIEnums.Prüfidentifikator, null, identifikator);
                prüf.AddField(EDIEnums.ID, identifikator);
                nachricht.AddChild(prüf);
            }
            else if (edi_info.Format == "CONTRL")
            {

            }
            else if (edi_info.Format == "APERAK")
            {

            }
            else if (edi_info.Format == "ORDERS")
            {
                IEdiObject prüf = new IEdiObject(EDIEnums.Prüfidentifikator, null, identifikator);
                prüf.AddField(EDIEnums.ID, identifikator);
                nachricht.AddChild(prüf);
            }
            else if (edi_info.Format == "ORDRSP")
            {
                IEdiObject prüf = new IEdiObject(EDIEnums.Prüfidentifikator, null, identifikator);
                prüf.AddField(EDIEnums.ID, identifikator);
                nachricht.AddChild(prüf);
            }
            else if (edi_info.Format == "MSCONS")
            {
                IEdiObject prüf = new IEdiObject(EDIEnums.Prüfidentifikator, null, identifikator);
                prüf.AddField(EDIEnums.ID, identifikator);
                nachricht.AddChild(prüf);
            }
            else if (edi_info.Format == "INVOIC")
            {
                IEdiObject prüf = new IEdiObject(EDIEnums.Prüfidentifikator, null, identifikator);
                prüf.AddField(EDIEnums.ID, identifikator);
                nachricht.AddChild(prüf);
            }
            else if (edi_info.Format == "REMADV")
            {
                nachricht.AddField(EDIEnums.Prüfidentifikator, identifikator);
            }

        }
        public static string BerechnePrüfindikator(EDIFileInfo edi_info, IEdiObject nachricht, IEdiObject vorgang, string referenceProcess)
        {
            string Process = "";
            if (edi_info.Format == "UTILMD")
            {
                string Kategorie = nachricht.Field(Repositories.EDIEnums.Kategorie);
                string Leistungsumfang = vorgang.Field(Repositories.EDIEnums.Leistung_MSB_MDL);
                string Transaktionsgrund = vorgang.Field(EDIEnums.Transaktionsgrund);
                bool storno = false;

                if (vorgang.ContainsChild(Repositories.EDIEnums.Referenz_Storno))
                {
                    storno = true;
                }
                bool? antwort_positiv = null;
                List<string> AntwortStati = new List<string>();
                if (vorgang.ContainsField(Repositories.EDIEnums.Antwortstatus))
                {
                    antwort_positiv = true;
                    string[] Antwortstati = vorgang.FieldList(EDIEnums.Antwortstatus);

                    foreach (string status in Antwortstati)
                    {
                        AntwortStati.Add(status);
                        if (!AnswerStatusRepository.GetAnswerStatusByCode("", status).Zustimmung)
                        {
                            antwort_positiv = false;
                        }
                    }
                }
                if (storno)
                {
                    if (Leistungsumfang != "Z07")
                    {
                        if (antwort_positiv.HasValue == false)
                            Process = "11022";
                        else if (antwort_positiv.Value == true)
                            Process = "11023";
                        else
                            Process = "11024";
                    }
                }
                else if (Kategorie == "E01" && Leistungsumfang == "Z02")
                {
                    if (antwort_positiv.HasValue == false)
                        Process = "11048";
                    else if (antwort_positiv.Value == true)
                        Process = "11049";
                    else
                        Process = "11050";
                }
                else if (Kategorie == "E01" && (Leistungsumfang == "Z01" || Leistungsumfang == "Z03"))
                {
                    if (antwort_positiv.HasValue == false)
                        Process = "11042";
                    else if (antwort_positiv.Value == true)
                        Process = "11043";
                    else
                        Process = "11044";
                }
                else if (Kategorie == "E02" && Leistungsumfang == "Z02")
                {
                    if (antwort_positiv.HasValue == false)
                        Process = "11054";
                    else if (antwort_positiv.Value == true)
                        Process = "11055";
                    else
                        Process = "11056";
                }
                else if (Kategorie == "E02" && (Leistungsumfang == "Z01" || Leistungsumfang == "Z03"))
                {
                    if (antwort_positiv.HasValue == false)
                        Process = "11051";
                    else if (antwort_positiv.Value == true)
                        Process = "11052";
                    else
                        Process = "11053";
                }
                else if (Kategorie == "E35" && Leistungsumfang == "Z02")
                {
                    if (antwort_positiv.HasValue == false)
                        Process = "11045";
                    else if (antwort_positiv.Value == true)
                        Process = "11046";
                    else
                        Process = "11047";
                }
                else if (Kategorie == "E35" && (Leistungsumfang == "Z01" || Leistungsumfang == "Z03"))
                {
                    if (antwort_positiv.HasValue == false)
                        Process = "11039";
                    else if (antwort_positiv.Value == true)
                        Process = "11040";
                    else
                        Process = "11041";
                }
                else if (Kategorie == "E01")
                {
                    if (new string[] { "Z36", "Z37", "Z38", "Z39", "ZC6", "ZC7", "E06" }.Contains(Transaktionsgrund))
                    {
                        if (antwort_positiv.HasValue == false)
                            Process = "11013";
                        else if (antwort_positiv.Value == true)
                            Process = "11014";
                        else
                            Process = "11015";
                    }
                    else
                    {
                        if (antwort_positiv.HasValue == false)
                            Process = "11001";
                        else if (antwort_positiv.Value == true)
                            Process = "11002";
                        else
                            Process = "11003";
                    }
                }
                else if (Kategorie == "E02")
                {
                    if (Transaktionsgrund == "Z33")
                    {
                        if (antwort_positiv.HasValue == false)
                            Process = "11007";
                        else if (antwort_positiv.Value == true)
                            Process = "11008";
                        else
                            Process = "11009";
                    }

                    else
                    {
                        if (vorgang.ContainsField(Repositories.EDIEnums.Antwortstatus))
                        {
                            if (edi_info != null && edi_info.Empfänger != null && edi_info.Empfänger.Rolle == "VNB")
                            {
                                if (antwort_positiv.HasValue == false)
                                    Process = "11010";
                                else if (antwort_positiv.Value == true)
                                    Process = "11011";
                                else
                                    Process = "11012";
                            }
                            else
                            {
                                if (antwort_positiv.HasValue == false)
                                    Process = "11004";
                                else if (antwort_positiv.Value == true)
                                    Process = "11005";
                                else
                                    Process = "11006";
                            }
                        }
                        else
                        {
                            if (edi_info.Sender.Rolle == "VNB")
                            {
                                if (antwort_positiv.HasValue == false)
                                    Process = "11010";
                                else if (antwort_positiv.Value == true)
                                    Process = "11011";
                                else
                                    Process = "11012";
                            }
                            else
                            {
                                if (antwort_positiv.HasValue == false)
                                    Process = "11004";
                                else if (antwort_positiv.Value == true)
                                    Process = "11005";
                                else
                                    Process = "11006";
                            }
                        }
                    }
                }
                else if (Kategorie == "E35")
                {
                    if (antwort_positiv.HasValue == false)
                        Process = "11016";
                    else if (antwort_positiv.Value == true)
                        Process = "11017";
                    else
                        Process = "11018";
                }
                else if (Kategorie == "E44")
                {
                    if (Transaktionsgrund == "Z26")
                        Process = "11036";
                    else if (Transaktionsgrund == "ZC8")
                        Process = "11037";
                    else if (Transaktionsgrund == "ZC9")
                        Process = "11038";
                }
                else if (Kategorie == "Z13")
                {
                    if (AntwortStati.Count == 0)
                        Process = "11057";
                    else if (AntwortStati.Contains("ZB4"))
                        Process = "11058";
                    else
                        Process = "11059";
                }
                else if (Kategorie == "E03")
                {
                    //Stammdatenänderung
                    if (Transaktionsgrund == "ZE6")
                    { //Nicht bila.rel. Änderung vom LF: 11109 (LF an Verteiler) 11110 (Verteiler an MDL) 11111 (Antwort, MDL->NB->LF)
                        if (edi_info.Empfänger.Rolle == "VNB")
                        {
                            if (edi_info.Sender.Rolle.StartsWith("MDL"))
                                Process = "11111";
                            else
                                Process = "11109";
                        }
                        else if (edi_info.Empfänger.Rolle == "MDL")
                            Process = "11110";
                        else if (edi_info.Empfänger.Rolle == "LIEF")
                            Process = "11111";
                    }
                    else
                    if (Transaktionsgrund == "ZE7")
                    { //Nicht bila.rel. Änderung vom NB 11112 (NB an LF) 11113 (NB an MSB) 11114 (NB an MDL) 11115 (Antwort)
                        if (edi_info.Empfänger.Rolle == "MSB")
                            Process = "11113";
                        else if (edi_info.Empfänger.Rolle == "MDL")
                            Process = "11114";
                        else if (edi_info.Empfänger.Rolle == "LIEF")
                            Process = "11112";
                        else if (edi_info.Empfänger.Rolle == "VNB")
                            Process = "11115";
                    }
                    else
                    if (Transaktionsgrund == "ZE8")
                    { //Änderung vom MSB 11116 MSB->VT 11117 VT-LF 11118 VT-> MDL 11119 Antwort MDL,LF->VT->MSB
                        if (edi_info.Empfänger.Rolle == "MSB")
                            Process = "11119";
                        else if (edi_info.Empfänger.Rolle == "MDL")
                            Process = "11118";
                        else if (edi_info.Empfänger.Rolle == "LIEF")
                            Process = "11117";
                        else if (edi_info.Empfänger.Rolle == "VNB")
                        {
                            if (edi_info.Sender.Rolle.StartsWith("MSB"))
                                Process = "11116";
                            else
                                Process = "11119";
                        }
                    }
                    else
                    if (Transaktionsgrund == "ZE9")
                    { //Bila.rel. Änderung vom LF 11120 LF-> NB 11121 Antwort NB-> LF
                        if (edi_info.Empfänger.Rolle == "LIEF")
                            Process = "11121";
                        else if (edi_info.Empfänger.Rolle == "VNB")
                            Process = "11120";
                    }
                    else
                    if (Transaktionsgrund == "ZF0")
                    { //Bila.rel. Änderung vom NB mit Abhängigkeiten 11123 NB-> LF 11124 Antwort LF-> NB
                        if (edi_info.Empfänger.Rolle == "LIEF")
                            Process = "11123";
                        else if (edi_info.Empfänger.Rolle == "VNB")
                            Process = "11124";
                    }
                    else
                    if (Transaktionsgrund == "ZF1")
                    { //Änderung des Zählverfahrens 11126 NB-> LF 11127 Antwort LF->NB
                        if (edi_info.Empfänger.Rolle == "LIEF")
                            Process = "11126";
                        else if (edi_info.Empfänger.Rolle == "VNB")
                            Process = "11127";

                    }
                    else
                    if (Transaktionsgrund == "ZF2")
                    { //Korrektur der Zählpunktbezeichnung vom NB 11129 LF 11130 MSB 11131 MDL 11132 Antwort
                        if (edi_info.Empfänger.Rolle == "MSB")
                            Process = "11130";
                        else if (edi_info.Empfänger.Rolle == "MDL")
                            Process = "11131";
                        else if (edi_info.Empfänger.Rolle == "LIEF")
                            Process = "11129";
                        else if (edi_info.Empfänger.Rolle == "VNB")
                            Process = "11132";
                    }
                    else
                    // Stammdatenanfrage
                    if (Transaktionsgrund == "ZF3")
                    { //Nicht bila.rel. Anfrage an LF 11133 MDL->VT 11134 Antwort VT->MDL 11135 Ablehnung VT-> MDL 11136 VT->LF 11137 NB (berechtigt)-> LF 11138 Antwort LF->NB
                        if (edi_info.Empfänger.Rolle == "MDL")
                        {
                            //11134 Antwort VT->MDL 
                            //11135 Ablehnung VT-> MDL
                            if (antwort_positiv.Value == false)
                                Process = "11135";
                            else
                                Process = "11134";

                        }
                        else if (edi_info.Empfänger.Rolle == "LIEF")
                        {
                            // Unterscheidung ob VNB Verteiler oder Berechtigter!!
                            //11136 VT->LF
                            Process = "11136";
                            //11137 NB (berechtigt)-> LF
                        }
                        else if (edi_info.Empfänger.Rolle == "VNB")
                        {
                            if (antwort_positiv.HasValue == false)
                                Process = "11133";
                            else
                                Process = "11138";
                            //11133 MDL->VT
                            //11138 Antwort LF->NB
                        }

                    }
                    else
                    if (Transaktionsgrund == "ZF4")
                    { //Nicht bila.rel. Anfrage an NB 11139 LF 11140 MSB 11141 MDL 11142 Antwort
                        if (edi_info.Sender.Rolle.StartsWith("MSB"))
                            Process = "11140";
                        else if (edi_info.Sender.Rolle.StartsWith("MDL"))
                            Process = "11141";
                        else if (edi_info.Sender.Rolle == "LIEF")
                            Process = "11139";
                        else if (edi_info.Sender.Rolle == "VNB")
                            Process = "11142";
                    }
                    else
                    if (Transaktionsgrund == "ZF5")
                    { //Anfrage an MSB mit Abhängigkeiten 11143 LF->VT 11144 MDL->VT 11145 Antwort 11146 Ablehnung 11147 VT->MSB 11148 NB(berechtigt)->MSB 11149 Antwort
                        if (edi_info.Sender.Rolle.StartsWith("MSB"))
                            Process = "11149";
                        else if (edi_info.Sender.Rolle.StartsWith("MDL"))
                            Process = "11144";
                        else if (edi_info.Sender.Rolle == "LIEF")
                            Process = "11143";
                        else if (edi_info.Sender.Rolle == "VNB")
                        {
                            if (antwort_positiv.HasValue)
                            {
                                //11145 Antwort 
                                //11146 Ablehnung

                                if (antwort_positiv.Value == false)
                                    Process = "11146";
                                else
                                    Process = "11145";
                            }
                            else // Unterscheidung ob VNB Verteiler oder Berechtigter!!
                                Process = "11147";
                            //11147 VT->MSB 
                            //11148 NB(berechtigt)->MSB

                        }
                    }
                    else
                    if (Transaktionsgrund == "ZF6")
                    { //Bila.rel. Anfrage an LF 11150 NB->LF 11151 Antwort 11152 Ablehnung
                        if (edi_info.Sender.Rolle == "VNB")
                            Process = "11150";
                        else if (edi_info.Sender.Rolle == "LIEF")
                        {
                            if (antwort_positiv.Value)
                                Process = "11151";
                            else
                                Process = "11152";
                        }
                    }
                    else
                    if (Transaktionsgrund == "ZF7")
                    { //Bila.rel. Anfrage an NB ohne Abhängigkeiten 11153 LF->NB 11154 Antwort 11155 Ablehnung
                        if (edi_info.Sender.Rolle == "LIEF")
                            Process = "11153";
                        else if (edi_info.Sender.Rolle == "VNB")
                        {
                            if (antwort_positiv.Value)
                                Process = "11154";
                            else
                                Process = "11155";
                        }
                    }
                    else
                    if (Transaktionsgrund == "ZF8")
                    { //Bila.rel. Anfrage an NB mit Abhängigkeiten 11156 LF->NB 11157 Antwort
                        if (edi_info.Sender.Rolle == "LIEF")
                            Process = "11156";
                        else if (edi_info.Sender.Rolle == "VNB")
                        {
                            Process = "11157";
                        }
                    }
                    else
                                    if (Transaktionsgrund == "Z46") // die alten TA-Gründe, aus Kompatibilitätsgründen nicht gelöscht
                    {
                        if (antwort_positiv.HasValue == false)
                        {
                            if (edi_info.Sender.Rolle == "VNB")
                            {
                                Process = "11028";
                            }
                            else if (edi_info.Sender.Rolle.StartsWith("MSB"))
                            {
                                Process = "11026";
                            }
                            else if (edi_info.Sender.Rolle.StartsWith("MDL"))
                            {
                                Process = "11027";
                            }
                            else
                                Process = "11025";
                        }
                        else
                        {
                            Process = "11029";
                        }
                    }
                    else if (Transaktionsgrund == "Z47") // die alten TA-Gründe, aus Kompatibilitätsgründen nicht gelöscht
                    {
                        if (antwort_positiv.HasValue == false)
                        {
                            if (edi_info.Sender.Rolle == "VNB")
                            {
                                Process = "11031";
                            }
                            else
                            {
                                Process = "11030";
                            }
                        }
                        else
                        {
                            Process = "11032";
                        }
                    }
                }
                else if (Kategorie == "Z14")
                {
                    if (edi_info.Empfänger.Rolle == "MSB")
                        Process = "11060";
                    else if (edi_info.Empfänger.Rolle == "MDL")
                        Process = "11061";
                    else
                        Process = "11035";
                }


                return Process.Trim();


            }
            else if (edi_info.Format == "IFTSTA")
            {
                string msgKategorie = nachricht.Field(Repositories.EDIEnums.Kategorie);
                string Kategorie = vorgang.Child(EDIEnums.Status).Field(EDIEnums.Kategorie);
                string Status = vorgang.Child(EDIEnums.Status).Field(EDIEnums.Code);
                string Anlass = vorgang.Child(EDIEnums.Status).Field(EDIEnums.Anlass);
                if (Kategorie == "Z12")
                {
                    Process = "21028";
                }
                else if (msgKategorie == "Z09")
                {
                    if (Kategorie == "")
                    {
                        if (edi_info.Empfänger.Rolle.Contains("MSB"))
                            Process = "21007";
                        else if (edi_info.Empfänger.Rolle.Contains("MDL"))
                            Process = "21008";
                    }
                    else if (Kategorie == "Z09" || Kategorie == "Z10")
                    {
                        if (Status == "Z13")
                        {
                            if (edi_info.Sender.Rolle == "MSB")
                            {
                                Process = "21009";
                            }
                            else if (edi_info.Sender.Rolle == "VNB")
                            {
                                if (edi_info.Empfänger.Rolle == "MSBA")
                                    Process = "21014";
                                else if (edi_info.Empfänger.Rolle == "MDLA")
                                    Process = "21016";
                                else if (edi_info.Empfänger.Rolle == "MSBN")
                                {
                                    if (Anlass == "Z66")
                                    {
                                        Process = "21011";
                                    }
                                    else if (Anlass == "E17")
                                    {
                                        Process = "21013";
                                    }
                                    else
                                    {
                                    }
                                }

                            }
                        }
                        else if (Status == "Z14")
                        {
                            if (edi_info.Sender.Rolle.Contains("MSB"))
                            {
                                Process = "21010";
                            }
                            else if (edi_info.Sender.Rolle.Contains("VNB"))
                            {
                                if (edi_info.Empfänger.Rolle.Contains("MSB"))
                                {
                                    if (!vorgang.ContainsChild(EDIEnums.Messstellenbetreiber))
                                    {
                                        if (edi_info.Empfänger.Rolle == "MSBN")
                                            Process = "21012";
                                        else
                                            Process = "21018";
                                    }
                                    else
                                    {
                                        Process = "21015";
                                    }

                                }
                                else if (edi_info.Empfänger.Rolle.Contains("MDL"))
                                {
                                    if (edi_info.Empfänger.Rolle == "MDLA")
                                        Process = "21017";
                                    else
                                        Process = "21019";
                                }
                            }
                        }
                        else
                        {
                            /*  if (edi_info.Empfänger.Rolle.Contains("MSB")) // vollendet
                                  Process = "21018";
                              else if (edi_info.Empfänger.Rolle.Contains("MDL"))
                                  Process = "21019";*/
                            if (edi_info.Empfänger.Rolle == "MSBA") // geplant
                                Process = "21007";
                            else if (edi_info.Empfänger.Rolle.Contains("MDL"))
                                Process = "21008";
                        }
                    }
                    else if (Kategorie == "Z11")
                    {
                        if (edi_info.Empfänger.Rolle.Contains("MSB"))
                        {
                            if (referenceProcess.Contains("Beginn"))
                                Process = "21021";
                            else
                                Process = "21023";
                        }
                        else if (edi_info.Empfänger.Rolle.Contains("MDL"))
                        {
                            if (referenceProcess.Contains("Beginn"))
                                Process = "21020";
                            else
                                Process = "21022";
                        }
                    }
                    else if (Kategorie == "Z15")
                    {
                        if (Status == "Z13")
                        {
                            if (edi_info.Empfänger.Rolle == "LIEF")
                                Process = "21025";
                            else
                                Process = "21027";

                        }
                        else if (Status == "Z14")
                        {
                            if (edi_info.Empfänger.Rolle == "LIEF")
                                Process = "21024";
                            else
                                Process = "21026";
                        }
                    }

                }


            }
            else if (edi_info.Format == "INSRPT")
            {
                string docname = vorgang.Field(EDIEnums.Dokumentenname);
                if (docname == "21")
                    Process = "23001";
                if (docname == "22")
                {

                    string[] Antwortstati = vorgang.Child(EDIEnums.Position, "1").FieldList(EDIEnums.Antwortstatus);
                    if (Antwortstati != null)
                    {
                        bool antwort_positiv = true;
                        foreach (string status in Antwortstati)
                        {
                            if (!AnswerStatusRepository.GetAnswerStatusByCode("Störungsmeldung", status).Zustimmung)
                            {
                                antwort_positiv = false;
                            }
                        }
                        if (antwort_positiv)
                        {
                            Process = "23004";
                        }
                        else
                        {
                            Process = "23003";
                        }
                    }
                }
                if (docname == "23")
                {
                    if (edi_info.Empfänger.Rolle == "VNB")
                        Process = "23005";
                    else
                        Process = "23006";
                }
                if (docname == "293")
                {
                    string status = vorgang.Child(EDIEnums.Position, "1").Child(EDIEnums.Gerätestatus).Field(EDIEnums.Status);
                    if (status == "Z09")
                    {
                        if (edi_info.Empfänger.Rolle == "VNB")
                            Process = "23009";
                        else
                            Process = "23010";
                    }
                    else
                        Process = "23008";
                }
            }
            else if (edi_info.Format == "REQOTE")
            {
                Process = "35001";
            }
            else if (edi_info.Format == "QUOTES")
            {
                Process = "15001";
            }
            else if (edi_info.Format == "CONTRL")
            {
                return null;
            }
            else if (edi_info.Format == "APERAK")
            {
                return null;
            }
            else if (edi_info.Format == "ORDERS")
            {
                string msgKategorie = nachricht.Field(Repositories.EDIEnums.Kategorie);
                string leistung = nachricht.Field(EDIEnums.Leistungsbeschreibung);
                if (msgKategorie == "Z11")
                {
                    Process = "17002";
                }
                else if (msgKategorie == "Z10")
                {
                    Process = "17001";
                }
                else if (msgKategorie == "7")
                {
                    if (leistung == "Z10")
                        Process = "17103";
                    else if (leistung == "Z11" || leistung == "Z12")
                        Process = "17102";
                    else if (leistung == "Z13")
                        Process = "17004";
                }
                else if (msgKategorie == "Z12")
                {
                    Process = "17003";
                }
                else if (msgKategorie == "Z14")
                {
                    Process = "17101";
                }
            }
            else if (edi_info.Format == "ORDRSP")
            {
                string msgKategorie = nachricht.Field(Repositories.EDIEnums.Kategorie);
                string leistung = nachricht.Field(EDIEnums.Leistungsbeschreibung);
                string antwortkategorie = "";
                if (nachricht.ContainsChild(EDIEnums.Antwortkategorie))
                    antwortkategorie = nachricht.Child(EDIEnums.Antwortkategorie).Key;
                if (msgKategorie == "Z11")
                {
                    if (antwortkategorie == "Z22")
                        Process = "19004";
                    else
                        Process = "19003";
                }
                else if (msgKategorie == "Z10")
                {
                    if (antwortkategorie == "Z13")
                        Process = "19001";
                    else
                        Process = "19002";
                }
                else if (msgKategorie == "7")
                {
                    if (leistung == "Z10")
                        Process = "19103";
                    else if (leistung == "Z11" || leistung == "Z12")
                        Process = "19102";
                    else
                    {
                        if (antwortkategorie == "Z19")
                            Process = "19007";
                        else
                            Process = "19008";
                    }
                }
                else if (msgKategorie == "Z12")
                {
                    if (antwortkategorie == "Z13" || antwortkategorie == "Z14")
                        Process = "19005";
                    else
                        Process = "19006";
                }
                else if (msgKategorie == "Z14")
                {
                    Process = "19101";
                }
            }
            else if (edi_info.Format == "MSCONS")
            {
                string Funktion = nachricht.Field(Repositories.EDIEnums.Nachrichtenfunktion);
                if (Funktion == "1")
                    Process = "13006";
                else
                {
                    Process = "13002";
                }

            }
            else if (edi_info.Format == "INVOIC")
            {
                if (edi_info.Empfänger.Rolle == "LIEF")
                    Process = "31002";
                else
                    Process = "31003";
            }
            else if (edi_info.Format == "REMADV")
            {
                string msgKategorie = nachricht.Field(Repositories.EDIEnums.Kategorie);
                if (msgKategorie == "239")
                    Process = "33002";
                else
                    Process = "33001";
            }
            return Process.Trim();
        }
        public static string GetProcessName(EDIFileInfo edi_info, IEdiObject nachricht, IEdiObject vorgang, string referenceProcess)
        {
            string Process = "";
            if (edi_info.Format == "UTILMD")
            {
                string Kategorie = nachricht.Field(Repositories.EDIEnums.Kategorie);
                string Leistungsumfang = vorgang.Field(Repositories.EDIEnums.Leistung_MSB_MDL);
                string Transaktionsgrund = vorgang.Field(EDIEnums.Transaktionsgrund);
                if (Kategorie == "E01" && Leistungsumfang == "Z02")
                    Process = "Anmeldung MDL";
                else if (Kategorie == "E01" && (Leistungsumfang == "Z01" || Leistungsumfang == "Z03"))
                    Process = "Anmeldung MSB";
                else if (Kategorie == "E02" && Leistungsumfang == "Z02")
                    Process = "Abmeldung MDL";
                else if (Kategorie == "E02" && (Leistungsumfang == "Z01" || Leistungsumfang == "Z03"))
                    Process = "Abmeldung MSB";
                else if (Kategorie == "E35" && Leistungsumfang == "Z02")
                    Process = "Kündigung MDL";
                else if (Kategorie == "E35" && (Leistungsumfang == "Z01" || Leistungsumfang == "Z03"))
                    Process = "Kündigung MSB";
                else if (Kategorie == "E01")
                {
                    if (new string[] { "Z36", "Z37", "Z38", "Z39", "ZC6", "ZC7", "E06" }.Contains(Transaktionsgrund))
                    {
                        Process = "Anmeldung EoG";
                    }
                    else
                        Process = "Anmeldung";
                }
                else if (Kategorie == "E02")
                {
                    if (Transaktionsgrund == "Z33")
                    {
                        Process = "Stilllegung";
                    }
                    else
                    {
                        if (vorgang.ContainsField(Repositories.EDIEnums.Antwortstatus))
                        {
                            if (edi_info != null && edi_info.Empfänger != null && edi_info.Empfänger.Rolle == "VNB")
                                Process = "Abmeldungsanfrage";
                            else
                                Process = "Abmeldung";
                        }
                        else
                        {
                            if (edi_info.Sender.Rolle == "VNB")
                                Process = "Abmeldungsanfrage";
                            else
                                Process = "Abmeldung";
                        }
                    }
                }
                else if (Kategorie == "E35")
                    Process = "Kündigung";
                else if (Kategorie == "E44")
                    Process = "Informationsmeldung";
                else if (Kategorie == "Z13")
                    Process = "Anzeige Gerätewechsel";
                else if (Kategorie == "E03")
                {
                    if (Transaktionsgrund == "ZE8" || vorgang.ContainsChild(EDIEnums.Zählernummer) || vorgang.ContainsChild(EDIEnums.Zählwerke))
                        Process = "Stammdatenänderung Gerätewechsel";
                    else
                        if (new string[] { "ZF3", "ZF4", "ZF5", "ZF6", "ZF7", "ZF8" }.Contains(Transaktionsgrund))
                        Process = "Stammdatenanfrage";
                    else
                        Process = "Stammdatenänderung";
                }
                else if (Kategorie == "Z14")
                {
                    Process = "Antwort Geschäftsdatenanfrage";
                }
                if (String.IsNullOrEmpty(referenceProcess) == false && (referenceProcess != "Anmeldung" && Process == "Informationsmeldung"))
                    Process = referenceProcess;
                bool storno = false;

                if (vorgang.ContainsChild(Repositories.EDIEnums.Referenz_Storno))
                {
                    storno = true;
                }
                if (vorgang.ContainsField(Repositories.EDIEnums.Antwortstatus))
                {
                    string[] Antwortstati = vorgang.FieldList(EDIEnums.Antwortstatus);
                    bool antwort_positiv = true;
                    foreach (string status in Antwortstati)
                    {
                        if (!AnswerStatusRepository.GetAnswerStatusByCode("", status).Zustimmung)
                        {
                            antwort_positiv = false;
                        }
                    }
                    if (vorgang.Field(EDIEnums.Transaktionsgrund) == "E05")
                    {
                        Process = "Antwort";
                    }
                    else if (Process.Contains("Storno") || Process == "" || Process.Contains("Weiterverpflichtung") || Process.Contains("Stammdatenänderung"))
                    {
                        Process = "Antwort " + Process;
                    }
                    else if (antwort_positiv)
                    {
                        if (Process.Contains("Stammdatenanfrage"))
                            Process = "Antwort " + Process;
                        else
                            Process = "Bestätigung " + Process;
                    }
                    else
                    {
                        Process = "Ablehnung " + Process;
                    }

                }
                else if (storno)
                {
                    Process = "Storno " + Process;
                }
                //Sonderfall für Storno AA bzw. Stilllegung. Diese beiden lassen sich von der Nachricht her nicht unterscheiden, daher gemeinsamer Storno
                if (Process.Trim() == "Storno Abmeldungsanfrage")
                    return "Storno AA/Stilllegung";
                return Process.Trim();


            }
            else if (edi_info.Format == "IFTSTA")
            {
                string msgKategorie = nachricht.Field(Repositories.EDIEnums.Kategorie);
                string Kategorie = vorgang.Child(EDIEnums.Status).Field(EDIEnums.Kategorie);
                string Status = vorgang.Child(EDIEnums.Status).Field(EDIEnums.Status);
                string Anlass = vorgang.Child(EDIEnums.Status).Field(EDIEnums.Anlass);
                if (Kategorie == "Z12")
                {
                    Process = "Scheitern Ablesung";
                }
                else if (msgKategorie == "Z09")
                {

                    Process = "Information Zuordnung";
                }

            }
            else if (edi_info.Format == "INSRPT")
            {
                string docname = vorgang.Field(EDIEnums.Dokumentenname);
                if (docname == "21")
                    Process = "Störungsmeldung";
                if (docname == "22")
                {
                    Process = "Störungsmeldung";
                    string[] Antwortstati = vorgang.Child(EDIEnums.Position, "1").FieldList(EDIEnums.Antwortstatus);
                    if (Antwortstati != null)
                    {
                        bool antwort_positiv = true;
                        foreach (string status in Antwortstati)
                        {
                            if (!AnswerStatusRepository.GetAnswerStatusByCode("Störungsmeldung", status).Zustimmung)
                            {
                                antwort_positiv = false;
                            }
                        }
                        if (antwort_positiv)
                        {
                            Process = "Bestätigung " + Process;
                        }
                        else
                        {
                            Process = "Ablehnung " + Process;
                        }
                    }
                }
                if (docname == "23")
                {
                    Process = "Störungsinformation";
                }
                if (docname == "293")
                {
                    Process = "Störungsbehebung";
                }
            }
            else if (edi_info.Format == "REQOTE")
            {
                Process = "Anforderung Geräteübernahme";
            }
            else if (edi_info.Format == "QUOTES")
            {
                if (nachricht.Field(EDIEnums.Leistungsbeschreibung) == "Z07")
                {
                    Process = "Angebot Kauf";
                }
                else
                    Process = "Angebot Überlassung";
            }
            else if (edi_info.Format == "CONTRL")
            {
                return "CONTRL";
            }
            else if (edi_info.Format == "APERAK")
            {
                if (nachricht.Field(EDIEnums.Kategorie) == "ERR")
                    return "APERAK - Verarbeitung";
                else
                    return "APERAK - Modell";
            }
            else if (edi_info.Format == "ORDERS")
            {
                string msgKategorie = nachricht.Field(Repositories.EDIEnums.Kategorie);
                if (msgKategorie == "Z11")
                {
                    Process = "Weiterverpflichtung MSBA";
                }
                else if (msgKategorie == "Z10")
                {
                    if (nachricht.Field(EDIEnums.Leistungsbeschreibung) == "Z07")
                    {
                        Process = "Bestellung Kauf";
                    }
                    else
                        Process = "Bestellung Überlassung";
                }
                else if (msgKategorie == "7")
                {
                    if (nachricht.Field(EDIEnums.Leistungsbeschreibung) == "Z13")
                    {
                        Process = "Anforderung Messwerte";
                    }
                    else
                    {
                        // Process = "Anforderung Messwerte";
                        // Process = "Anforderung Geschäftsdaten";
                        Process = "Anforderung Geschäftsdaten Messwerte";
                    }
                }
                else if (msgKategorie == "Z12")
                {
                    Process = "Anforderung Messstellenänderung";
                }
                else if (msgKategorie == "Z14")
                {
                    Process = "Anforderung Geschäftsdaten";
                }
            }
            else if (edi_info.Format == "ORDRSP")
            {
                string msgKategorie = nachricht.Field(Repositories.EDIEnums.Kategorie);
                if (msgKategorie == "Z11")
                {
                    Process = "Antwort Weiterverpflichtung MSBA";
                }
                else if (msgKategorie == "Z10")
                {
                    if (nachricht.Field(EDIEnums.Leistungsbeschreibung) == "Z07")
                    {
                        Process = "Antwort Bestellung Kauf";
                    }
                    else
                        Process = "Antwort Bestellung Überlassung";
                }
                else if (msgKategorie == "7")
                {
                    Process = "Ablehnung Anforderung Messwerte";
                }
                else if (msgKategorie == "Z12")
                {
                    Process = "Antwort Anforderung Messstellenänderung";
                }
                else if (msgKategorie == "Z14")
                {
                    Process = "Ablehnung Geschäftsdatenanfrage";
                }
            }
            else if (edi_info.Format == "MSCONS")
            {
                string Funktion = nachricht.Field(Repositories.EDIEnums.Nachrichtenfunktion);
                if (Funktion == "1")
                    Process = "Storno Zählerstand";
                else
                {
                    Process = "Zählerstand";
                }

            }
            else if (edi_info.Format == "INVOIC")
            {
                Process = "Rechnung";
            }
            else if (edi_info.Format == "REMADV")
            {
                string msgKategorie = nachricht.Field(Repositories.EDIEnums.Kategorie);
                if (msgKategorie == "239")
                    Process = "Reklamation Rechnung";
                else
                    Process = "Zahlung Rechnung";
            }
            return Process.Trim();
        }
        public static string NormalizeEDIHeader(string edi)
        {


            if (edi == null)
                return null;
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
