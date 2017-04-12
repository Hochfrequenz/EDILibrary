using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDILibrary.Repositories;

namespace EDILibrary
{
    public class CONTRLGenerator
    {
        public List<string> errors = new List<string>();
        protected string LastError = "";

        public string GenerateControl(EDIFileInfo edi_info, TreeElement tree, IEdiObject edi_root, string edi, out bool result, out string edi_ref, string contrl_version = "1.3d")
        {
            EDIFileInfo contrl_info = new EDIFileInfo()
            {
                Empfänger = edi_info.Sender,
                Sender = edi_info.Empfänger,
                Format = "CONTRL",
                Version = contrl_version
            };
            string createtemplateCONTRL = "";//TODO: Template laden EDIExtensions.LoadResourceTemplate(contrl_info, "create.template");
            EDILibrary.GenericEDIWriter writer = new GenericEDIWriter();
            var contrl = GenerateControlObject(edi_info, tree, edi_root,edi, out result, out edi_ref, contrl_version);
            string contrl_edi = writer.CompileTemplate(createtemplateCONTRL, contrl);
            edi_ref = contrl.Key;
            return contrl_edi;
        }
        public virtual IEdiObject GenerateControlObject(EDIFileInfo edi_info, TreeElement tree, IEdiObject edi_root, string edi, out bool result, out string edi_ref, string contrl_version = "1.3d")
        {
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

            if(edi_root!=null)
                nachricht.AddField(EDIEnums.Referenz, edi_root.Key);
            else
                nachricht.AddField(EDIEnums.Referenz, "kein UNB-Segment");

            nachricht.AddField(EDIEnums.Status, result ? "7" : "4");
            
            newDoc.AddChild(nachricht);
            edi_ref = newDoc.Key;
            return newDoc;
        }
        protected virtual bool CheckSegment(TreeElement testElement)
        {
            if (String.IsNullOrEmpty(testElement.CONTRL_Check_String))
                return true;
            foreach (string sedi in testElement.Edi)
            {
                string edi = sedi;
                edi = edi.Replace("?+", "?<").Replace("?:", "?>");
                string[] Groups = edi.Split(new char[] { '+' }, StringSplitOptions.None);
                string[] CheckGroups = testElement.CONTRL_Check_String.Split(new char[] { '+' });
                int iGroupIndex = 0;
                foreach (string group in CheckGroups)
                {
                    string groupString = null;
                    if (Groups.Length > iGroupIndex + 1)
                        groupString = Groups[iGroupIndex + 1];
                    if (group[0] == 'M')
                    {
                        if (String.IsNullOrWhiteSpace(groupString))
                        {
                            errors.Add("Verpflichtendes Segment in " + Groups[0] + " nicht gefunden");
                            return false;
                        }
                    }
                    else if (String.IsNullOrWhiteSpace(groupString))
                        return true; // optionales Segment ohne Inhalt
                    string[] GroupParts = groupString.Split(new char[] { ':' }, StringSplitOptions.None);
                    string[] CheckGroupParts = group.Split(new char[] { '*' }, StringSplitOptions.None);
                    int iGroupPartIndex = 0;
                    if (GroupParts.Count() > (CheckGroupParts.Count() - 1))
                    {
                        //zu viele Elemente in einer Segmentgruppe
                        return false;
                    }
                    foreach (string groupPart in CheckGroupParts.Skip(1)) // das erste Element ist das C/R der Gruppe
                    {
                        string groupPartString = null;
                        if (GroupParts.Length > iGroupPartIndex)
                            groupPartString = GroupParts[iGroupPartIndex];
                        if (groupPart[0] == 'M')
                        {
                            if (String.IsNullOrWhiteSpace(groupPartString))
                            {
                                errors.Add("Verpflichtendes Element in " + GroupParts[0] + " nicht gefunden");
                                return false;
                            }
                        }
                        else if (groupPartString == null)
                            return true; // optionales Datenelement ohne Inhalt

                        //check for Length and data type
                        string dataType = groupPart.Substring(1, 2); //data type
                        int Length = Int32.Parse(groupPart.Substring(3));

                        string replacedString = groupPartString.Replace("?<", "+").Replace("?>", ":").Replace("?$", "'");

                        if (replacedString.Length > Length)
                        {
                            errors.Add("String " + replacedString + " länger als erlaubt ("+Length+")");
                            return false;
                        }

                        //use regular expression to check data type
                        // nn = numeric
                        // an = alphanumeric
                        // aa = alpha

                        iGroupPartIndex++;
                    }

                    iGroupIndex++;
                }

            }
            return true;
        }


        protected virtual bool CheckTreeElement(TreeElement testElement, bool bParentMandatory = false)
        {
            // if (testElement.CONTRL_Status == "M" || testElement.CONTRL_Status==null)
            {
                foreach (TreeElement child in testElement.Children.Values)
                {
                    if ((testElement.Dirty || testElement.Name == "/") && CheckTreeElement(child, bParentMandatory && testElement.CONTRL_Status == "M") == false)
                    {
                        if (LastError == "")
                        {
                            LastError = child.Name;
                        }
                        errors.Add("Segment " + LastError+ " fehlt");
                        return false;
                    }
                }

                if ((testElement.Name.StartsWith("SG") == false && testElement.Name != "UNH") && testElement.Name != "/" && testElement.CONTRL_Status == "M" && bParentMandatory && testElement.Edi.Count == 0)
                {
                    LastError = testElement.Name;
                    //errors.Add("Segment " + LastError + " fehlt");
                    return false;
                }
            }
            if (testElement.Name.StartsWith("SG") == false)
            {
                return CheckSegment(testElement);
            }
            return true;
        }
    }
}
