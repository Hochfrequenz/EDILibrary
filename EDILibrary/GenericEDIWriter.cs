// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace EDILibrary 
{
    public class ScriptHelper
    {
        public readonly bool useLocalTime = true;
        public String Escape(string input)
        {
            return input.Replace("+", "?+").Replace(":", "?:").Replace("'","?'");
        }
        public String FormatDate(String dateString, String format)
        {

            if (!DateTime.TryParse(dateString, out DateTime date))
                return dateString;

            switch (format)
            {
                case "102":
                    {
                        return date.ToLocalTime().ToString("yyyyMMdd");
                    }
                case "106":
                    {
                        if (useLocalTime)
                            return date.ToLocalTime().ToString("MMdd");
                        return date.ToString("MMdd");
                    }
                case "203":
                    {
                        if (useLocalTime)
                            return date.ToLocalTime().ToString("yyyyMMddHHmm");
                        return date.ToString("yyyyMMddHHmm");
                    }
                case "204":
                    {
                        if (useLocalTime)
                            return date.ToLocalTime().ToString("yyyyMMddHHmmss");
                        return date.ToString("yyyyMMddHHmmss");
                    }
                case "303":
                    {
                        if (useLocalTime)
                        {

                            var utcOffset = new DateTimeOffset(date, TimeSpan.Zero);
                            int offset = utcOffset.ToOffset(TimeZoneInfo.Local.GetUtcOffset(utcOffset)).Offset.Hours;
                            return date.ToLocalTime().ToString("yyyyMMddHHmm") + "+0" + offset.ToString(); ;

                        }
                        else
                        {
                            // Zeitzone ist stets UTC
                            return date.ToString("yyyyMMddHHmm") + "+00";
                        }
                    }
                case "406":
                    {
                        if (useLocalTime)
                        {
                            var utcOffset = new DateTimeOffset(date, TimeSpan.Zero);
                            int offset = utcOffset.ToOffset(TimeZoneInfo.Local.GetUtcOffset(utcOffset)).Offset.Hours;
                            return "+0" + offset.ToString()+"00";
                        }
                        else
                        {
                            // wir speichern immer UTC, daher die Zeitzone auf 0 setzen
                            return "+0000";
                        }
                    }
                case "Z01":
                    {
                        return dateString;
                    }
                case "602":
                    {
                        return date.ToString("yyyy");
                    }
                case "610":
                    {
                        return date.ToString("yyyyMM");
                    }
                default:
                    {
                        return dateString;
                    }


            }
        }
    }
    public class GenericEDIWriter
    {
        public static ScriptHelper helper = new ScriptHelper();
        Regex numericRegex = new Regex("^[0-9]+$");
        public GenericEDIWriter()
        {
            escapeMap.Add(":", "?:");
            escapeMap.Add("+", "?+");
            escapeMap.Add("'", "?'");
            escapeMap.Add("’", "?'");
            escapeMap.Add("‘", "?'");
            escapeMap.Add("‛", "?'");
            escapeMap.Add("′", "?'");

        }
        protected Dictionary<string, string> escapeMap = new Dictionary<string, string>();
        protected string EscapeValue(string value)
        {
            
            foreach(KeyValuePair<string,string> pair in escapeMap)
            {
                value = value.Replace(pair.Key, pair.Value);
            }
            return value;
        }
        String RecurseTemplate(string template, IEdiObject parent)
        {
            int currentIndex = 0;
            int beginIndex = 0;
            int endIndex = 0;
            StringBuilder resultBuilder = new StringBuilder();
            do
            {
                beginIndex = template.IndexOf("<", currentIndex);
                if (beginIndex == -1)
                    continue;
                endIndex = template.IndexOf(">", beginIndex);
                string codeTemplate = template.Substring(beginIndex, (endIndex - beginIndex) + 1);
                string code = codeTemplate.Substring(1, codeTemplate.Length - 2);
                resultBuilder.Clear();
                if (codeTemplate.StartsWith("<foreach"))
                {
                    string[] nodeparts = code.Split(new char[] { ' ' });
                    string node = String.Join(" ", nodeparts.Skip(1));
                    string innercode;
                    beginIndex = template.IndexOf("</foreach " + node + ">", endIndex);
                    innercode = template.Substring(endIndex + 1, (beginIndex - endIndex) - 1);
                    var nodes = from ele in parent.SelfOrChildren
                                where ele.Name == node
                                select ele;
                    if (nodes.Count() == 0) // wenn keine Treffer könnte es sich noch um eine field-Liste handeln
                    {
                        var string_nodes = from ele in parent.Fields
                                           where ele.Key == node
                                           select ele.Value;
                        foreach (List<string> value_node in string_nodes)
                        {
                            foreach (string val in value_node)
                            {
                                IEdiObject tempObject = new IEdiObject(node, null, val);
                                tempObject.Fields.Add(node, new List<string>() { val });
                                resultBuilder.Append(RecurseTemplate(innercode, tempObject));

                            }
                        }

                    }
                    int i = 1;
                    int max = nodes.Count();
                    foreach (var subnode in nodes)
                    {
                        resultBuilder.Append(RecurseTemplate(innercode, subnode) + ((i != max) ? "\r\n" : ""));
                    }
                    beginIndex = template.IndexOf("<foreach", 0);
                    string end = "</foreach " + node + ">";
                    endIndex = template.IndexOf(end, beginIndex);

                    template = template.Substring(0, beginIndex) + template.Substring(beginIndex, endIndex - beginIndex + end.Length).Replace(template.Substring(beginIndex, (endIndex - beginIndex) + end.Length), resultBuilder.ToString()) + template.Substring(endIndex + end.Length);
                    template = template.TrimEnd(new char[] { '\r', '\n', '\t' });
                    beginIndex = 0;
                }
                else if (codeTemplate.StartsWith("<if"))
                {
                    string[] nodeparts = code.Split(new char[] { ' ' });
                    string node = String.Join(" ", nodeparts.Skip(1));
                    string innercode;
                    beginIndex = template.IndexOf("</if>", endIndex);
                    innercode = template.Substring(endIndex + 1, (beginIndex - endIndex) - 1);

                    string value = null;

                    var selection = (from ele in parent.Fields
                                     where ele.Key == node
                                     select ele.Value[0]);
                    if (selection.Count() > 0)
                        value = selection.Single();
                    else
                        value = "";

                    if (value != null)
                        value = EscapeValue(value.Trim());

                    if (value != null && value.Length > 0)
                    {
                        resultBuilder.Append(RecurseTemplate(innercode, parent));
                    }
                    beginIndex = template.IndexOf("<if", 0);
                    endIndex = template.IndexOf("</if>", beginIndex);
                    template = template.Substring(0, beginIndex) + template.Substring(beginIndex, endIndex - beginIndex + 5).Replace(template.Substring(beginIndex, (endIndex - beginIndex) + 5), resultBuilder.ToString()) + template.Substring(endIndex + 5);
                    beginIndex = 0;

                }
                else if (codeTemplate.StartsWith("<date"))
                {
                    string[] nodeparts = code.Split(new char[] { ' ' });
                    string node = String.Join(" ", nodeparts.Skip(1));
                    var innerNodeParts = node.Split(new char[] { ';' });
                    string value = null;

                    var selection = (from ele in parent.Fields
                                     where ele.Key == innerNodeParts[0]
                                     select ele.Value[0]);
                    if (selection.Count() > 0)
                        value = selection.Single();
                    else
                        value = "";

                    if (value != null)
                        value = value.Trim();

                    string format = innerNodeParts[1];
                    
                    if (!numericRegex.IsMatch(innerNodeParts[1]))
                    {
                        selection = (from ele in parent.Fields
                                     where ele.Key == innerNodeParts[1]
                                     select ele.Value[0]);

                        if (selection.Count() > 0)
                            format = selection.Single();
                        else
                            format = "";

                        if (format != null)
                            format = EscapeValue(format.Trim());
                    }
                    if (value != null && value.Length > 0)
                    {
                        resultBuilder.Append(helper.Escape(helper.FormatDate(value, format)));
                    }
                    beginIndex = template.IndexOf("<date", 0);
                    endIndex = template.IndexOf(">", beginIndex);
                    template = template.Substring(0, beginIndex) + template.Substring(beginIndex, endIndex - beginIndex + 1).Replace(template.Substring(beginIndex, (endIndex - beginIndex) + 1), resultBuilder.ToString()) + template.Substring(endIndex + 1);
                    beginIndex = 0;

                }
                else if (codeTemplate.StartsWith("<!") || (codeTemplate.StartsWith("<$")))
                {
                    //determine segment counter
                    // do it the "dirty" way, count the segment ends from last unh
                    if (codeTemplate.Contains("SegmentCounter"))
                    {
                        int segCount = template.Substring(template.Substring(0, beginIndex).LastIndexOf("UNH+")).Count(c => c == "'".ToCharArray()[0]);
                        //escapte ' muss ich abziehen
                        Regex r = new Regex("\\?'");
                        int deduct=r.Matches(template.Substring(template.Substring(0, beginIndex).LastIndexOf("UNH+"))).Count;
                        segCount -= deduct;
                        // deduct UNH and UNT
                        /* Aufgrund von Fehlermeldung "weiterer Fehler? Segmentzähler" vom 22.08.2011 werden UNH und UNT nun mitgezählt*/
                        //segCount-=2;
                        resultBuilder.Append(segCount);
                        //TODO: Direktes schreiben in die Ausgabe
                        //scope.SetVariable("SegmentCounter", segCount.ToString());
                        
                    }
                    else if (codeTemplate.Contains("MessageNumber"))
                    {
                        int messageCount = template.Split(new string[]{"UNH+"},StringSplitOptions.RemoveEmptyEntries).Length - 1;
                        resultBuilder.Append(messageCount);
                        //TODO: Direktes schreiben in die Ausgabe
                        //scope.SetVariable("MessageNumber", messageCount.ToString());
                    }
                    code = code.TrimStart(new char[] { '!', '$' });
                    // evaluate code
                    try
                    {
                        String[] lines = code.Split(new char[] { ';' });
                        /*int counter = 0;
                        
                        foreach (string line in lines)
                        {
                            
                            if (counter == lines.Count() - 1 && codeTemplate.StartsWith("<!"))
                            {

                                if (!compilerCache.ContainsKey(line))
                                {
                                    compilerCache.Add(line, engine.CreateScriptSourceFromString(line, SourceCodeKind.Expression).Compile());
                                }
                                String result = (String)compilerCache[line].Execute(scope);
                                resultBuilder.Append(result);
                            }
                            else
                            {
                                if (!compilerCache.ContainsKey(line))
                                {
                                    compilerCache.Add(line, engine.CreateScriptSourceFromString(line, SourceCodeKind.SingleStatement).Compile());
                                }
                                compilerCache[line].Execute(scope);

                            }
                            counter++;
                        }*/
                    }
                    catch (Exception)
                    {
                   //     MessageBox.Show(e.ToString());
                    }
                    template = template.Replace(codeTemplate, resultBuilder.ToString());
                }
                else if (codeTemplate.StartsWith("<§"))
                {
                    String[] items = code.Split(new char[] { ' ' });
                    string variableName = items.Skip(1).Take(1).First();
                    string item = String.Join(" ", items.Skip(2));
                    string value = null;
                    try
                    {
                        value = (from ele in parent.Fields
                                 where ele.Key == item
                                 select ele.Value[0]).SingleOrDefault();
                    }
                    catch (Exception)
                    {
                        value = "";
                    }
                    //scope.SetVariable(variableName, value);
                    template = template.Replace(codeTemplate, "");
                }
                else
                {
                    string value = null;
                    string length = null;
                    string max_count = null;
                    string[] fieldLengths = null;
                    if(code.Contains("["))
                    {
                        string[] code_parts = code.Split("[".ToCharArray());
                        code = code_parts[0];

                        // es gibt zwei verschiedene Ansätze, entweder gleich verteilte Felder [35,2] oder Angabe aller Feldlängen [70;9;6]
                        if (code_parts[1].Contains(','))
                        {
                            string[] length_parts = code_parts[1].Split(",".ToCharArray());
                            length = length_parts[0];
                            max_count = length_parts[1].Substring(0, length_parts[1].Length - 1);
                        }
                        else if (code_parts[1].Contains(';'))
                        {
                            fieldLengths = code_parts[1].Split(",".ToCharArray());
                           
                        }
                    }
                    if (code == parent.Name+":Key")
                    {
                        if (parent.Field("Key") != null)
                            value = parent.Field("Key");
                        else
                            value = parent.Key;
                    }
                    else
                    {


                        
                            var selection = (from ele in parent.Fields
                                     where ele.Key == code
                                     select ele.Value[0]);
                            if (selection.Count() > 0)
                            {
                                value = selection.Single();
                            }
                            else
                                value = "";

                            if (value != null)
                                value = EscapeValue(value.Trim());
                        
                        
                    }
                    if (length != null)
                    {
                        List<string> parts = new List<string>();
                        if (value == null)
                            value = "";
                        string temp_value =value;
                        int laenge = Int32.Parse(length);
                        if (max_count != null) // gleiche Längen
                        {
                            while (temp_value.Length > laenge)
                            {

                                string temp = temp_value.Substring(0, laenge);
                                parts.Add(temp);
                                temp_value = temp_value.Substring(laenge);
                            }
                            parts.Add(temp_value);
                            // Auffüllen auf Maximalanzahl, da sonst Konstellationen wie Nachname::Vorname nicht möglich sind
                            while (parts.Count < Int32.Parse(max_count))
                            {
                                parts.Add("");
                            }
                        }
                        else if (fieldLengths != null) // feste Längen
                        {
                            int count = fieldLengths.Length;
                            while (fieldLengths.Length > 0)
                            {
                                laenge = Int32.Parse(fieldLengths.First());
                                fieldLengths = fieldLengths.Skip(1).ToArray();
                                if(temp_value.Length> laenge)
                                {
                                    string temp = temp_value.Substring(0, laenge);
                                    parts.Add(temp);
                                    temp_value = temp_value.Substring(laenge);
                                }
                                else
                                {
                                    parts.Add(temp_value);
                                }
                                
                            }
                            while (parts.Count < count)
                            {
                                parts.Add("");
                            }
                        }
                        // Bei leerem Vornamen muss trotzdem ein Doppelpunkt drin sein.
                        //if((from string s in parts where s!="" select s).Count()>0)
                            value = string.Join(":", parts.Take(Int32.Parse(max_count)));
                       
                    }
                    
                    resultBuilder.Append(value);
                    
                    //template = template.Replace(codeTemplate, evalResult);
                    template = template.Substring(0, beginIndex) + template.Substring(beginIndex, endIndex - beginIndex + 1).Replace(template.Substring(beginIndex, (endIndex - beginIndex) + 1), resultBuilder.ToString()) + template.Substring(endIndex + 1);
                }

                currentIndex = 0;
            } while (beginIndex != -1);

            return template.Trim();

        }
        public static void Preinitialize()
        {
            
        }
        public String CompileTemplate(String template, IEdiObject sourceRoot)
        {
            
            string result = RecurseTemplate(template, sourceRoot);
            string UNA = result.Substring(0, 8);
            string content = result.Substring(8);
            content = content.Replace("??", "<<").Replace("?+", "?<").Replace("?:", "?>");
            while (content.Contains(":+"))
            {
                content = content.Replace(":+", "+");
            }
            while (content.Contains(":'"))
            {
                content = content.Replace(":'", "'");
            }
            while(content.Contains("+'"))
            {
               content= content.Replace("+'","'");
            }
            while (content.Contains(":'"))
            {
                content = content.Replace(":'", "'");
            }
            while (content.Contains("+'"))
            {
                content = content.Replace("+'", "'");
            }
            while (content.Contains(":'"))
            {
                content = content.Replace(":'", "'");
            }
            while ((content.Contains("\n\n")) || (content.Contains("\r\r")) || (content.Contains("\n\r")) || (content.Contains("\r\n\r\n")) || (content.Contains("\r\n")) || content.Contains("\r\n\r\n") || (content.Contains("\r\n")) )
             {
				while(content.Contains("\r\n\r\n"))		 
                {
					content = content.Replace("\r\n\r\n","\r\n");
				}
				while (content.Contains("\r\n"))
				{
					content = content.Replace("\r\n", "");
				}
                while (content.Contains("\n\n"))
                {
                    content = content.Replace("\n\n", "\n");
                }
                while (content.Contains("\r\r"))
                {
                    content = content.Replace("\r\r", "\r");
                }
                while (content.Contains("\n\r"))
                {
                    content = content.Replace("\n\r", "\r\n");
                }
                while (content.Contains("\r\n\r\n"))
                {
                    content = content.Replace("\r\n\r\n", "\r\n");
                }
                while (content.Contains("\r\n"))
                {
                    content = content.Replace("\r\n", "");
                }
             }
            while (content.Contains("\n"))
            {
                content = content.Replace("\n", "");
            }
            content = content.Replace("?<", "?+").Replace("?>", "?:");
            return UNA+content;
        }
    }
}
