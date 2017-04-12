using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EDILibrary.Repositories;

namespace EDILibrary
{
    public class CONTRLError
    {
        public string ErrorMessage;
        public string ErrorCode;
        public string Edi;
        public TreeElement EdiElement;
        public int Segmentposition;
        public int Gruppenposition;
    }
    public class CONTRLGeneratorExtended : CONTRLGenerator
    {
        public new List<CONTRLError> errors = new List<CONTRLError>();
        public Dictionary<string, Dictionary<TreeElement, List<CONTRLError>>> messageErrors = new Dictionary<string, Dictionary<TreeElement, List<CONTRLError>>>();
        protected Regex alphanumericRegex = new Regex(@"^[\p{IsLatinExtended-A}\p{IsBasicLatin}\p{IsLatin-1Supplement}\p{IsLatinExtended-B}]*$");
        protected Regex alphaRegex = new Regex(@"^[^\d]*$");
        protected Regex numericRegex = new Regex(@"^[\p{N}\p{P}]*$");
        protected string[] segments;
        protected string fullEDI;
        protected string lastEDI = null;
        public override IEdiObject GenerateControlObject(EDIFileInfo edi_info, TreeElement tree, IEdiObject edi_root, string edi, out bool result, out string edi_ref, string contrl_version = "2.0")
        {
            fullEDI = edi;

            string elementDelimiter = ":";
            string groupDelimiter = "+";
            string segmentDelimiter = "'";
            int UNAoffset = -1;

            if (fullEDI.StartsWith("UNA"))
            {


                string UNA = fullEDI.Substring(0, 9);
                UNAoffset = 8;
                elementDelimiter = UNA.Substring(3, 1);
                groupDelimiter = UNA.Substring(4, 1);
                segmentDelimiter = UNA.Substring(8, 1);
                if (segmentDelimiter == "\r")
                    segmentDelimiter = "\r\n";
            }

            string message = fullEDI.Substring(UNAoffset + segmentDelimiter.Length, fullEDI.Length - (UNAoffset + segmentDelimiter.Length));
            message = message.Replace("?'", "?$").Replace("\r\n", "");
            segments = message.LowMemSplit(segmentDelimiter).ToArray();
            errors.Clear();
            // check the mandatory elements in the tree
            result = true;
            result = CheckTreeElement(tree, true);

            EDIFileInfo contrl_info = new EDIFileInfo()
            {
                Empfänger = edi_info.Sender,
                Sender = edi_info.Empfänger,
                Format = "CONTRL",
                Version = contrl_version
            };
            EDILibrary.GenericEDIWriter writer = new GenericEDIWriter();
            IEdiObject newDoc = new IEdiObject(Repositories.EDIEnums.Dokument, null, Guid.NewGuid().ToString("N").Substring(0, 13).ToUpper());
            IEdiObject nachricht = new IEdiObject(EDIEnums.Nachricht, null, Guid.NewGuid().ToString("N").Substring(0, 13).ToUpper());
            EDIPartner sender = contrl_info.Sender;
            EDIPartner empfänger = contrl_info.Empfänger;

            newDoc.AddField(EDIEnums.Absender, contrl_info.Sender.ID);
            newDoc.AddField(EDIEnums.Absender_Code_UNB, contrl_info.Sender.CodeList);
            newDoc.AddField(EDIEnums.Empfänger, contrl_info.Empfänger.ID);
            newDoc.AddField(EDIEnums.Empfänger_Code_UNB, contrl_info.Empfänger.CodeList);

            nachricht.AddField(EDIEnums.Absender, contrl_info.Sender.ID);
            nachricht.AddField(EDIEnums.Absender_Code_UNB, contrl_info.Sender.CodeList);
            nachricht.AddField(EDIEnums.Empfänger, contrl_info.Empfänger.ID);
            nachricht.AddField(EDIEnums.Empfänger_Code_UNB, contrl_info.Empfänger.CodeList);

            if (edi_root != null)
                nachricht.AddField(EDIEnums.Referenz, edi_root.Key);
            else
                nachricht.AddField(EDIEnums.Referenz, "kein UNB-Segment");
            result = errors.Count() == 0 && messageErrors.Count == 0;
            nachricht.AddField(EDIEnums.Status, result ? "7" : "4");
            if (!result)
            {
                //Ebene 1

                if (errors.Count() > 0)
                {
                    //Fehler in Ebene 1
                    nachricht.AddField(EDIEnums.Syntaxfehler, errors.First().ErrorCode);
                    nachricht.AddField(EDIEnums.Servicesegment, errors.First().Edi.Substring(0, 3));
                    if (errors.First().Gruppenposition > 0)
                        nachricht.AddField(EDIEnums.Gruppenposition, errors.First().Gruppenposition.ToString());
                    nachricht.AddField(EDIEnums.Segmentposition, errors.First().Segmentposition.ToString());
                }
                else
                {
                    foreach (var msg in messageErrors.Keys)
                    {
                        //Ebene 2
                        IEdiObject antwort = new IEdiObject(EDIEnums.Antwortsegment, null, msg);
                        antwort.AddField(EDIEnums.Referenz, msg);
                        nachricht.AddChild(antwort);
                        antwort.AddField(EDIEnums.Nachrichtentyp, edi_info.Format);
                        antwort.AddField(EDIEnums.Nachrichtenversion, edi_info.Nachrichtenversion);
                        antwort.AddField(EDIEnums.Anwendungscode, edi_info.Version);
                        antwort.AddField(EDIEnums.Freigabenummer, edi_info.Freigabenummer);
                        var headersegments = messageErrors[msg].Keys.Where(s => s.Name.Contains("UNH") || s.Name.Contains("UNT"));
                        if (headersegments.Count() > 0)
                        {
                            //wenn UNH oder UNT in errorList enthalten, dann die entsprechenden Felder füllen
                            var err = messageErrors[msg][headersegments.First()].First();
                            antwort.AddField(EDIEnums.Syntaxfehler, err.ErrorCode);
                            if (err.Gruppenposition > 0)
                                antwort.AddField(EDIEnums.Gruppenposition, err.Gruppenposition.ToString());
                            antwort.AddField(EDIEnums.Segmentposition, err.Segmentposition.ToString());
                            antwort.AddField(EDIEnums.Servicesegment, err.Edi.Substring(0, 3));
                        }
                        else
                        {
                            foreach (var segment in messageErrors[msg].Keys)
                            {
                                //Ebene 3
                                var errorList = messageErrors[msg][segment];
                                //Ansonsten pro Segment ein UCS aufmachen
                                int segmentPosition = GetLineCounter(errorList.First());

                                IEdiObject ucs = new IEdiObject(EDIEnums.Fehlersegment, null, segmentPosition.ToString());
                                antwort.AddChild(ucs);
                                if (errorList.First().Segmentposition == 0)
                                    ucs.AddField(EDIEnums.Syntaxfehler, errorList.First().ErrorCode);

                                ucs.AddField(EDIEnums.Segmentposition, segmentPosition.ToString());

                                if (!segment.Name.Contains("UNH") && !segment.Name.Contains("UNT") && !(errorList.Count == 1 && errorList.First().ErrorCode == "13"))
                                {
                                    foreach (var err in errorList)
                                    {
                                        //Ebene 4
                                        //und pro Error dann noch ein UCD aufmachen
                                        IEdiObject ucd = new IEdiObject(EDIEnums.Fehlerelement, null, err.ErrorCode);
                                        if (err.Gruppenposition > 0)
                                            ucd.AddField(EDIEnums.Gruppenposition, err.Gruppenposition.ToString());
                                        ucd.AddField(EDIEnums.Syntaxfehler, err.ErrorCode);
                                        ucd.AddField(EDIEnums.Segmentposition, err.Segmentposition.ToString());
                                        ucs.AddChild(ucd);

                                    }
                                }
                                else if (errorList.Count == 1 && errorList.First().ErrorCode == "13")
                                {
                                    ucs.AddField(EDIEnums.Syntaxfehler, errorList.First().ErrorCode);
                                }
                            }
                        }
                    }
                }
            }
            newDoc.AddChild(nachricht);
            edi_ref = newDoc.Key;
            return newDoc;
        }
        protected int GetLineCounter(CONTRLError error)
        {
            string UNHRef;
            string UNBRef;
            int parentCounter = 0;

            if (error.EdiElement != null)
            {
                if (error.EdiElement.Parent != null && error.EdiElement.Parent.Children.First().Value.Edi.Count > 0)
                {
                    parentCounter = FindEDI(error.EdiElement.Parent.Children.First().Value.Edi.First().Replace("?<", "?+").Replace("?>", "?:").Replace("?$", "?'"), parentCounter, out UNHRef, out UNBRef);
                }
                else if (error.EdiElement.Parent != null && error.EdiElement.Parent.Parent != null && error.EdiElement.Parent.Parent.Edi.Count > 0)
                {
                    parentCounter = FindEDI(error.EdiElement.Parent.Parent.Edi.First().Replace("?<", "?+").Replace("?>", "?:").Replace("?$", "?'"), parentCounter, out UNHRef, out UNBRef);
                }
            }
            int lineCounter = 0;

            lineCounter = FindEDI(error.Edi.Replace("?<", "?+").Replace("?>", "?:").Replace("?$", "?'"), parentCounter, out UNHRef, out UNBRef);

            return lineCounter;
        }
        protected TreeElement FindUNH(TreeElement elem)
        {
            if (elem.Name.Contains("UNH"))
                return elem;
            else if (elem.Parent == null)
                return null;
            else return FindUNH(elem.Parent);
        }
        protected void AddCorrectError(CONTRLError err)
        {
            if (err.EdiElement.Name.Contains("UNA") || err.EdiElement.Name.Contains("UNB") || err.EdiElement.Name.Contains("UNZ"))
            {
                errors.Add(err);
            }
            else
            {
                TreeElement unhElem = FindUNH(err.EdiElement);

                if (unhElem != null)
                {
                    string UNHref = GetValueFromEdi("UNH:1:0", unhElem.Edi.First());
                    if (!messageErrors.ContainsKey(UNHref))
                    {
                        messageErrors.Add(UNHref, new Dictionary<TreeElement, List<CONTRLError>>());
                    }
                    if (!messageErrors[UNHref].ContainsKey(err.EdiElement))
                    {
                        messageErrors[UNHref].Add(err.EdiElement, new List<CONTRLError>());
                    }
                    messageErrors[UNHref][err.EdiElement].Add(err);
                }
                else
                {
                    throw new Exception("Konnte kein passendes UNH-Segment finden");
                }
            }

        }
        protected override bool CheckSegment(TreeElement testElement)
        {
            if (String.IsNullOrEmpty(testElement.APERAK_Check_String))
                return true;
            foreach (string sedi in testElement.Edi)
            {
                lastEDI = sedi;
                string edi = sedi;
                edi = edi.Replace("?+", "?<").Replace("?:", "?>");
                string[] Groups = edi.Split(new char[] { '+' }, StringSplitOptions.None);
                string[] valueGroups = testElement.APERAK_Check_String.Split(new char[] { '|' });
                bool foundValueGroup = false;
                foreach (string valueGroup in valueGroups)
                {
                    if (foundValueGroup)
                        continue;
                    string valueString = null;
                    int klammerIndex = valueGroup.IndexOf(')');
                    if (klammerIndex != -1)
                    {
                        string testString = valueGroup.Substring(1, klammerIndex - 1);
                        string[] testStringSplit = testString.Split("=".ToCharArray());
                        if (GetValue(testStringSplit[0].Replace('#', ':'), testElement, sedi) == testStringSplit[1])
                            valueString = valueGroup.Substring(klammerIndex + 1);
                    }
                    else
                    {
                        valueString = valueGroup;
                    }
                    if (valueString == null)
                        continue;
                    else
                        foundValueGroup = true;
                    string[] CheckGroups = valueString.Split(new char[] { '+' });
                    int iGroupIndex = 0;
                    foreach (string group in CheckGroups)
                    {
                        string groupString = null;
                        if (Groups.Length > iGroupIndex + 1)
                            groupString = Groups[iGroupIndex + 1];
                        if (group[0] == 'M' || group[0] == 'R')
                        {
                            if (String.IsNullOrWhiteSpace(groupString))
                            {
                                AddCorrectError(new CONTRLError() { ErrorCode = "13", ErrorMessage = group + " ("+(iGroupIndex +1) +"/"+valueString+") war leer", Edi = edi, EdiElement = testElement, Segmentposition = iGroupIndex + 2 });
                                iGroupIndex++;
                                continue;
                            }

                        }
                        else if (String.IsNullOrWhiteSpace(groupString))
                        {
                            iGroupIndex++;
                            continue; // optionales Segment ohne Inhalt
                        }
                        string[] GroupParts = groupString.Split(new char[] { ':' }, StringSplitOptions.None);
                        string[] CheckGroupParts = group.Split(new char[] { '*' }, StringSplitOptions.None);
                        int iGroupPartIndex = 0;
                        foreach (string groupPart in CheckGroupParts.Skip(1)) // das erste Element ist das C/R der Gruppe
                        {
                            string groupPartString = null;
                            if (GroupParts.Length > iGroupPartIndex)
                                groupPartString = GroupParts[iGroupPartIndex];
                            if (groupPart[0] == 'M' || groupPart[0] == 'R')
                            {
                                if (String.IsNullOrWhiteSpace(groupPartString))
                                {
                                    AddCorrectError(new CONTRLError() { ErrorCode = "13", ErrorMessage = groupPartString+" war leer", Edi = edi, EdiElement = testElement, Segmentposition = iGroupIndex + 2, Gruppenposition = iGroupPartIndex + 1 });
                                    iGroupPartIndex++;
                                    continue;
                                }
                            }
                            else if (groupPartString == null)
                            {
                                iGroupPartIndex++;
                                continue; // optionales Datenelement ohne Inhalt
                            }

                            if (groupPart == "N")
                            {
                                iGroupPartIndex++;
                                continue; //in dem Fall ist es egal, ob und wie das Datenelement gefüllt ist
                            }
                            //check for Length and data type
                            string dataType = groupPart.Substring(1, 2); //data type
                            //value check
                            int maxLength = groupPart.Length;
                            int valueIndex = groupPart.IndexOf('{');
                            if (valueIndex > -1)
                            {
                                maxLength = valueIndex;
                                string valueListString = groupPart.Substring(valueIndex + 1, groupPart.IndexOf('}', valueIndex) - valueIndex - 1);
                                List<string> possibleValues = new List<string>(valueListString.Split(".".ToCharArray()));
                                if (!possibleValues.Contains(groupPartString))
                                {
                                    AddCorrectError(new CONTRLError() { ErrorCode = "12", ErrorMessage = "Wert " + groupPartString + " nicht im gültigen Wertevorrat", Edi = edi, EdiElement = testElement, Segmentposition = iGroupIndex + 2, Gruppenposition = iGroupPartIndex + 1 });
                                    iGroupPartIndex++;
                                    continue;
                                }
                            }
                            int Length = 0;
                            try
                            {
                                Length = Int32.Parse(groupPart.Substring(3, maxLength - 3));
                            }
                            catch (Exception)
                            {
                                throw new Exception(String.Format("Could not parse {0}", groupPart));
                            }
                            string replacedString = groupPartString.Replace("?<", "+").Replace("?>", ":").Replace("?$", "'");
                            if (replacedString.Length > Length)
                            {
                                AddCorrectError(new CONTRLError() { ErrorCode = "39", ErrorMessage = "Maximale Länge ist " + Length, Edi = edi, EdiElement = testElement, Segmentposition = iGroupIndex + 2, Gruppenposition = iGroupPartIndex + 1 });
                                iGroupPartIndex++;
                                continue;
                            }


                            switch (dataType)
                            {

                                case "nn":
                                    {
                                        if (!numericRegex.IsMatch(replacedString))
                                        {
                                            AddCorrectError(new CONTRLError() { ErrorCode = "37", ErrorMessage = "Kein numerischer String " + replacedString, Edi = edi, EdiElement = testElement, Segmentposition = iGroupIndex + 2, Gruppenposition = iGroupPartIndex + 1 });
                                            iGroupPartIndex++;
                                            continue;
                                        }
                                    }
                                    break;
                                case "an":
                                    {
                                        if (!alphanumericRegex.IsMatch(replacedString))
                                        {
                                            AddCorrectError(new CONTRLError() { ErrorCode = "37", ErrorMessage = "Kein alphanumerischer String " + replacedString, Edi = edi, EdiElement = testElement, Segmentposition = iGroupIndex + 2, Gruppenposition = iGroupPartIndex + 1 });
                                            iGroupPartIndex++;
                                            continue;
                                        }
                                    }
                                    break;
                                case "aa":
                                    {
                                        if (!alphaRegex.IsMatch(replacedString))
                                        {
                                            AddCorrectError(new CONTRLError() { ErrorCode = "37", ErrorMessage = "Kein alphanumerischer String " + replacedString, Edi = edi, EdiElement = testElement, Segmentposition = iGroupIndex + 2, Gruppenposition = iGroupPartIndex + 1 });
                                            iGroupPartIndex++;
                                            continue;
                                        }
                                    }
                                    break;
                            }
                            iGroupPartIndex++;
                        }

                        iGroupIndex++;
                    }
                }

            }
            return true;
        }


        protected override bool CheckTreeElement(TreeElement testElement, bool bParentMandatory = false)
        {
            if (testElement.Name.StartsWith("SG") == false)
            {
                CheckSegment(testElement);
            }
            foreach (TreeElement child in testElement.Children.Values)
            {
                if (((testElement.Dirty || (testElement.Name.StartsWith("SG") && testElement.Occurence == 0)) || testElement.Name == "/") && CheckTreeElement(child, bParentMandatory && (testElement.APERAK_Status == "M" || testElement.APERAK_Status == "R")) == false)
                {
                    if (LastError == "")
                    {
                        LastError = child.Name;
                    }
                    TreeElement par = child.Parent;
                    while (par != null && par.Edi.Count == 0)
                    {
                        par = par.Parent;
                    }
                    AddCorrectError(new CONTRLError() { ErrorCode = "13", ErrorMessage = testElement.Parent.Name + ":" + testElement.Name, Edi = child.Edi.First(), EdiElement = child });

                }
            }

            if ((testElement.Name.StartsWith("SG") == false && testElement.Name != "UNH") && testElement.Name != "/" && (testElement.APERAK_Status == "M" || testElement.APERAK_Status == "R") && bParentMandatory && testElement.Edi.Count == 0)
            {
                LastError = testElement.Name;
                TreeElement par = testElement.Parent;
                while (par != null && par.Edi.Count == 0)
                {
                    par = par.Parent;
                }
                AddCorrectError(new CONTRLError() { ErrorCode = "13", ErrorMessage = testElement.Parent.Name + ":" + testElement.Name, Edi = lastEDI, EdiElement = testElement });

            }

            return true;
        }
        protected string GetValueFromEdi(string pos, string edi)
        {




            if (pos == null || pos == "")
                return null;

            string[] SubPos = pos.Split(new char[] { ':' });
            if (!edi.StartsWith(SubPos[0]))
            {

                return null;
            }
            edi = edi.Replace("?+", "?<").Replace("?:", "?>");
            string[] Groups = edi.Split(new char[] { '+' });
            int GroupPos = Int32.Parse(SubPos[1]);
            if (Groups.Length <= GroupPos)
                return null;
            string[] SubGroups = Groups[GroupPos].Split(new char[] { ':' });
            if (SubPos[2].Contains("("))
            {
                string[] range = SubPos[2].Split(new char[] { ',' });
                int start = Int32.Parse(range[0].Substring(1));
                int end = Int32.Parse(range[1].Substring(0, range[1].Length - 1));
                List<string> parts = new List<string>();
                for (int i = start; i <= end; i++)
                {
                    if (SubGroups.Length <= i)
                        break;
                    parts.Add(SubGroups[i].Replace("?<", "+").Replace("?>", ":").Replace("?$", "'"));
                }
                return String.Join("", parts).Trim();
            }
            else
            {
                int DetailPos = Int32.Parse(SubPos[2]);
                if (SubGroups.Length <= DetailPos)
                    return null;
                String result = SubGroups[DetailPos].Replace("?<", "+").Replace("?>", ":").Replace("?$", "'");
                return result.Trim();
            }
        }
        protected string GetValue(string pos, TreeElement elem, string sedi = null)
        {
            string edi = "";

            if (sedi == null)
                edi = elem.Edi.First();
            else
                edi = sedi;



            if (pos == null || pos == "")
                return null;

            string[] SubPos = pos.Split(new char[] { ':' });
            if (!edi.StartsWith(SubPos[0]))
            {
                List<TreeElement> resultList = new List<TreeElement>();
                elem.Parent.FindElements(SubPos[0], true, ref resultList); //hier mal ein recurse = true ausprobieren
                var resultEdi = (from TreeElement res in resultList where res.Edi != null && res.Edi.Count() > 0 select res);
                if (resultEdi.Count() == 1)
                {
                    edi = resultEdi.First().Edi[0];
                }
                else
                    return null;
            }
            edi = edi.Replace("?+", "?<").Replace("?:", "?>");
            string[] Groups = edi.Split(new char[] { '+' });
            int GroupPos = Int32.Parse(SubPos[1]);
            if (Groups.Length <= GroupPos)
                return null;
            string[] SubGroups = Groups[GroupPos].Split(new char[] { ':' });
            if (SubPos[2].Contains("("))
            {
                string[] range = SubPos[2].Split(new char[] { ',' });
                int start = Int32.Parse(range[0].Substring(1));
                int end = Int32.Parse(range[1].Substring(0, range[1].Length - 1));
                List<string> parts = new List<string>();
                for (int i = start; i <= end; i++)
                {
                    if (SubGroups.Length <= i)
                        break;
                    parts.Add(SubGroups[i].Replace("?<", "+").Replace("?>", ":").Replace("?$", "'"));
                }
                return String.Join("", parts).Trim();
            }
            else
            {
                int DetailPos = Int32.Parse(SubPos[2]);
                if (SubGroups.Length <= DetailPos)
                    return null;
                String result = SubGroups[DetailPos].Replace("?<", "+").Replace("?>", ":").Replace("?$", "'");
                return result.Trim();
            }
        }
        protected int FindEDI(string searchstring, int iStartPos, out string UNHref, out string UNBref)
        {
            UNHref = null;
            UNBref = null;
            if (searchstring == null)
                return 0;
            int iLineCounter = -1;
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].StartsWith("UNB"))
                {
                    UNBref = GetValueFromEdi("UNB:5:0", segments[i]);
                }
                else if (segments[i].StartsWith("UNH"))
                {
                    UNHref = GetValueFromEdi("UNH:1:0", segments[i]);
                    iLineCounter = i;
                }
                if (i >= iStartPos && segments[i] == searchstring)
                {
                    return (i - iLineCounter) + 1;
                }
            }
            return 0;

        }
    }
}
