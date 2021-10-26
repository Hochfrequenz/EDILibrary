// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EDILibrary
{
    public class ScriptHelper
    {
        public bool useLocalTime = true;
        public static string Escape(string input)
        {
            return input.Replace("+", "?+").Replace(":", "?:").Replace("'", "?'");
        }
        public string FormatDate(string dateString, string format)
        {
            DateTime date;
            var foundDate = false;
            var deDE = new CultureInfo("de-DE");
            foundDate = DateTime.TryParseExact(dateString, new[] { "yyyyMMdd", "MMdd", "yyyyMMddHHmm", "yyyyMMddHHmmss" }, deDE, DateTimeStyles.None, out date);
            if (!foundDate && !DateTime.TryParse(dateString, out date))
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
                            var offset = utcOffset.ToOffset(TimeZoneInfo.Local.GetUtcOffset(utcOffset)).Offset.Hours;
                            return date.ToLocalTime().ToString("yyyyMMddHHmm") + "+0" + offset;

                        }

                        // Zeitzone ist stets UTC
                        return date.ToString("yyyyMMddHHmm") + "+00";
                    }
                case "406":
                    {
                        if (useLocalTime)
                        {
                            var utcOffset = new DateTimeOffset(date, TimeSpan.Zero);
                            var offset = utcOffset.ToOffset(TimeZoneInfo.Local.GetUtcOffset(utcOffset)).Offset.Hours;
                            return "+0" + offset + "00";
                        }

                        // wir speichern immer UTC, daher die Zeitzone auf 0 setzen
                        return "+0000";
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
        static readonly Regex numericRegex = new Regex("^[0-9]+$", RegexOptions.Compiled);
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
            return escapeMap.Aggregate(value, (current, pair) => current.Replace(pair.Key, pair.Value));
        }
        static readonly Regex questionMarkRegex = new Regex("\\?'", RegexOptions.Compiled);

        string RecurseTemplate(string template, EdiObject parent)
        {
            var currentIndex = 0;
            var beginIndex = 0;
            var endIndex = 0;
            var resultBuilder = new StringBuilder();
            do
            {
                beginIndex = template.IndexOf("<", currentIndex);// :warn: is culture specific
                if (beginIndex == -1)
                    continue;
                endIndex = template.IndexOf(">", beginIndex); // :warn: is culture specific
                var codeTemplate = template.Substring(beginIndex, endIndex - beginIndex + 1);
                var code = codeTemplate.Substring(1, codeTemplate.Length - 2);
                resultBuilder.Clear();
                if (codeTemplate.StartsWith("<foreach"))
                {
                    var nodeparts = code.Split(new[] { ' ' });
                    var node = string.Join(" ", nodeparts.Skip(1));
                    beginIndex = template.IndexOf("</foreach " + node + ">", endIndex);// :warn: is culture specific
                    var innercode = template.Substring(endIndex + 1, beginIndex - endIndex - 1);
                    var nodes = from ele in parent.SelfOrChildren
                                where ele.Name == node
                                select ele;
                    if (!nodes.Any()) // wenn keine Treffer könnte es sich noch um eine field-Liste handeln
                    {
                        var stringNodes = from ele in parent.Fields
                                          where ele.Key == node
                                          select ele.Value;
                        foreach (var valueNode in stringNodes)
                        {
                            foreach (var val in valueNode)
                            {
                                var tempObject = new EdiObject(node, null, val);
                                tempObject.Fields.Add(node, new List<string> { val });
                                resultBuilder.Append(RecurseTemplate(innercode, tempObject));
                            }
                        }
                    }
                    const int i = 1;
                    var max = nodes.Count();
                    foreach (var subnode in nodes)
                    {
                        resultBuilder.Append(RecurseTemplate(innercode, subnode) + (i != max ? Environment.NewLine : ""));
                    }
                    beginIndex = template.IndexOf("<foreach", 0);
                    var end = "</foreach " + node + ">";
                    endIndex = template.IndexOf(end, beginIndex);

                    template = template.Substring(0, beginIndex) + template.Substring(beginIndex, endIndex - beginIndex + end.Length).Replace(template.Substring(beginIndex, endIndex - beginIndex + end.Length), resultBuilder.ToString()) + template.Substring(endIndex + end.Length);
                    template = template.TrimEnd('\r', '\n', '\t');
                    beginIndex = 0;
                }
                else if (codeTemplate.StartsWith("<if"))
                {
                    var nodeparts = code.Split(new[] { ' ' });
                    var node = string.Join(" ", nodeparts.Skip(1));
                    beginIndex = template.IndexOf("</if>", endIndex);
                    var innercode = template.Substring(endIndex + 1, beginIndex - endIndex - 1);

                    string value = null;

                    var selection = from ele in parent.Fields
                                    where ele.Key == node
                                    select ele.Value[0];
                    if (selection.Any())
                        value = selection.Single();
                    else
                        value = "";

                    if (value != null)
                        value = EscapeValue(value.Trim());

                    if (!string.IsNullOrEmpty(value))
                    {
                        resultBuilder.Append(RecurseTemplate(innercode, parent));
                    }
                    beginIndex = template.IndexOf("<if", 0);
                    endIndex = template.IndexOf("</if>", beginIndex);
                    template = template.Substring(0, beginIndex) + template.Substring(beginIndex, endIndex - beginIndex + 5).Replace(template.Substring(beginIndex, endIndex - beginIndex + 5), resultBuilder.ToString()) + template.Substring(endIndex + 5);
                    beginIndex = 0;
                }
                else if (codeTemplate.StartsWith("<date"))
                {
                    var nodeparts = code.Split(new[] { ' ' });
                    var node = string.Join(" ", nodeparts.Skip(1));
                    var innerNodeParts = node.Split(new[] { ';' });
                    string value = null;

                    var selection = from ele in parent.Fields
                                    where ele.Key == innerNodeParts[0]
                                    select ele.Value[0];
                    var enumerable = selection as string[] ?? selection.ToArray();
                    if (enumerable.Any())
                        value = enumerable.Single();
                    else
                        value = "";

                    value = value?.Trim();

                    var format = innerNodeParts[1];

                    if (!numericRegex.IsMatch(innerNodeParts[1]))
                    {
                        selection = from ele in parent.Fields
                                    where ele.Key == innerNodeParts[1]
                                    select ele.Value[0];

                        if (selection.Any())
                            format = selection.Single();
                        else
                            format = "";

                        if (format != null)
                            format = EscapeValue(format.Trim());
                    }
                    if (!string.IsNullOrEmpty(value))
                    {
                        resultBuilder.Append(ScriptHelper.Escape(helper.FormatDate(value, format)));
                    }
                    beginIndex = template.IndexOf("<date", 0);
                    endIndex = template.IndexOf(">", beginIndex);
                    template = template.Substring(0, beginIndex) + template.Substring(beginIndex, endIndex - beginIndex + 1).Replace(template.Substring(beginIndex, endIndex - beginIndex + 1), resultBuilder.ToString()) + template.Substring(endIndex + 1);
                    beginIndex = 0;
                }
                else if (codeTemplate.StartsWith("<!") || codeTemplate.StartsWith("<$"))
                {
                    //determine segment counter
                    // do it the "dirty" way, count the segment ends from last unh
                    if (codeTemplate.Contains("SegmentCounter"))
                    {
                        var segCount = template.Substring(template.Substring(0, beginIndex).LastIndexOf("UNH+")).Count(c => c == "'".ToCharArray()[0]); // warn: culture specific
                                                                                                                                                        //escapte ' muss ich abziehen

                        var deduct = questionMarkRegex.Matches(template.Substring(template.Substring(0, beginIndex).LastIndexOf("UNH+"))).Count;
                        segCount -= deduct;
                        resultBuilder.Append(segCount);
                    }
                    else if (codeTemplate.Contains("MessageNumber"))
                    {
                        var messageCount = template.Split(new[] { "UNH+" }, StringSplitOptions.RemoveEmptyEntries).Length - 1;
                        resultBuilder.Append(messageCount);
                    }
                    code = code.TrimStart('!', '$');
                    // evaluate code
                    try
                    {
                        _ = code.Split(new[] { ';' });
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
                    catch (Exception) // todo: now pokoemon catching
                    {
                        //     MessageBox.Show(e.ToString());
                    }
                    template = template.Replace(codeTemplate, resultBuilder.ToString());
                }
                else if (codeTemplate.StartsWith("<§"))
                {
                    var items = code.Split(new[] { ' ' });
                    _ = items.Skip(1).Take(1).First();
                    _ = string.Join(" ", items.Skip(2));
                    /*
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
                    */
                    //scope.SetVariable(variableName, value);
                    template = template.Replace(codeTemplate, "");
                }
                else
                {
                    string value;// = null;
                    string length = null;
                    string maxCount = null;
                    string[] fieldLengths = null;
                    if (code.Contains("["))
                    {
                        var codeParts = code.Split("[".ToCharArray());
                        code = codeParts[0];

                        // es gibt zwei verschiedene Ansätze, entweder gleich verteilte Felder [35,2] oder Angabe aller Feldlängen [70;9;6]
                        if (codeParts[1].Contains(','))
                        {
                            var lengthParts = codeParts[1].Split(",".ToCharArray());
                            length = lengthParts[0];
                            maxCount = lengthParts[1].Substring(0, lengthParts[1].Length - 1);
                        }
                        else if (codeParts[1].Contains(';'))
                        {
                            fieldLengths = codeParts[1].Split(",".ToCharArray());
                        }
                    }
                    if (code == parent.Name + ":Key")
                    {
                        if (parent.Field("Key") != null)
                            value = parent.Field("Key");
                        else
                            value = parent.Key;
                    }
                    else
                    {
                        var selection = from ele in parent.Fields
                                        where ele.Key == code
                                        select ele.Value[0];
                        var enumerable = selection as string[] ?? selection.ToArray();
                        if (enumerable.Any())
                        {
                            value = enumerable.Single();
                        }
                        else
                            value = "";

                        if (value != null)
                            value = EscapeValue(value.Trim());
                    }
                    if (length != null)
                    {
                        var parts = new List<string>();
                        if (value == null)
                            value = "";
                        var temp_value = value;
                        var laenge = int.Parse(length);
                        if (maxCount != null) // gleiche Längen
                        {
                            while (temp_value.Length > laenge)
                            {

                                var temp = temp_value.Substring(0, laenge);
                                parts.Add(temp);
                                temp_value = temp_value.Substring(laenge);
                            }
                            parts.Add(temp_value);
                            // Auffüllen auf Maximalanzahl, da sonst Konstellationen wie Nachname::Vorname nicht möglich sind
                            while (parts.Count < int.Parse(maxCount))
                            {
                                parts.Add("");
                            }
                        }
                        else if (fieldLengths != null) // feste Längen
                        {
                            var count = fieldLengths.Length;
                            while (fieldLengths.Length > 0)
                            {
                                laenge = int.Parse(fieldLengths.First());
                                fieldLengths = fieldLengths.Skip(1).ToArray();
                                if (temp_value.Length > laenge)
                                {
                                    var temp = temp_value.Substring(0, laenge);
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
                        value = string.Join(":", parts.Take(int.Parse(maxCount)));

                    }

                    resultBuilder.Append(value);
                    //template = template.Replace(codeTemplate, evalResult);
                    template = template.Substring(0, beginIndex) + template.Substring(beginIndex, endIndex - beginIndex + 1).Replace(template.Substring(beginIndex, endIndex - beginIndex + 1), resultBuilder.ToString()) + template.Substring(endIndex + 1);
                }

                currentIndex = 0;
            } while (beginIndex != -1);

            return template.Trim();

        }
        public static void Preinitialize()
        {

        }

        private static readonly List<Tuple<string, string>> CompileReplacements = new List<Tuple<string, string>>
        {
            new Tuple<string, string>(":+", "+"),
            new Tuple<string, string>(":'","'"),
            new Tuple<string, string>("+'", "'"),
            new Tuple<string, string>(":'", "'"), // why are there duplicates? and why is the order important?
            new Tuple<string, string>("+'", "'"),
            new Tuple<string, string>(":'", "'"),
            // can we maybe use a compiled regex and simply replace all at once?
        };

        public string CompileTemplate(string template, EdiObject sourceRoot)
        {
            var result = RecurseTemplate(template, sourceRoot);
            // a good unittest could start here because everything that follows is just replacements.
            var una = result.Substring(0, 8);
            var content = result.Substring(8);
            // todo: use stringbuilder and stringbuilder.replace instead of overwriting content https://youtu.be/qfZVu0alU0I?t=34
            content = content.Replace("??", "<<").Replace("?+", "?<").Replace("?:", "?>");
            foreach (var (oldVar, replacement) in CompileReplacements)
            {
                while (content.Contains(oldVar))
                {
                    content = content.Replace(oldVar, replacement);
                }
            }
            // todo: write a unittest for the edge cases this code is supposed to solve and then find a better way
            while (content.Contains("\n\n") || content.Contains("\r\r") || content.Contains("\n\r") || content.Contains("\r\n\r\n") || content.Contains(Environment.NewLine) || content.Contains("\r\n\r\n") || content.Contains(Environment.NewLine))
            {
                while (content.Contains("\r\n\r\n"))
                {
                    content = content.Replace("\r\n\r\n", Environment.NewLine);
                }
                while (content.Contains(Environment.NewLine))
                {
                    content = content.Replace(Environment.NewLine, "");
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
                    content = content.Replace("\n\r", Environment.NewLine);
                }
                while (content.Contains("\r\n\r\n"))
                {
                    content = content.Replace("\r\n\r\n", Environment.NewLine);
                }
                while (content.Contains(Environment.NewLine))
                {
                    content = content.Replace(Environment.NewLine, "");
                }
            }
            while (content.Contains("\n"))
            {
                content = content.Replace("\n", "");
            }
            content = content.Replace("?<", "?+").Replace("?>", "?:");
            return una + content;
        }
    }
}
