// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace EDILibrary
{
    public class GenericEDILoader
    {
        protected virtual IEdiObject ProcessSpecificTemplate(XElement template, Dictionary<string, List<TreeElement>> objectMapping)
        {
            TreeElement treeRoot = objectMapping[TreeHelper.GetHash(template.ToString())][0];
            return ProcessSpecificTemplate(template, treeRoot, objectMapping);
        }
        protected virtual IEdiObject ProcessSpecificTemplate(XElement template, TreeElement treeRoot, Dictionary<string, List<TreeElement>> objectMapping)
        {
            string key = null;
            string ediSeg = null;
            if (template.Attribute("key") != null)
            {

                key = EvaluateTemplate(template.Attribute("key").Value, treeRoot, out ediSeg);

                if (key == null) // criteria not met
                    return null; // do not create child then

            }

            IEdiObject rootObject = new IEdiObject(template.Attribute("name").Value, template, key)
            {
                Edi = ediSeg
            };
            if (template.Attribute("migName") != null)
                rootObject.MigName = template.Attribute("migName").Value;
            var fields = from field in template.Elements("field") select field;
            foreach (XElement field in fields)
            {
                string selector = field.Attribute("ref").Value;
                if (field.Attribute("migName") != null)
                {
                    rootObject.MigFields[field.Attribute("name").Value] = field.Attribute("migName").Value;
                }
                string value = EvaluateTemplate(selector, treeRoot, out string ediSegment);
                rootObject.EdiFields[field.Attribute("name").Value] = ediSegment;
                if (value != null)
                {
                    if (rootObject.Fields.ContainsKey(field.Attribute("name").Value) == false)
                        rootObject.Fields[field.Attribute("name").Value] = new List<string>();
                    if (value.Contains("|"))
                    {
                        string[] subvalues = value.Split(new[] { '|' });
                        foreach (string val in subvalues)
                            rootObject.Fields[field.Attribute("name").Value].Add(val);
                    }
                    else
                    {
                        rootObject.Fields[field.Attribute("name").Value].Add(value);
                    }
                }
            }
            var children = from child in template.Elements("class") select child;

            foreach (XElement child in children)
            {
                int iChildCounter = 0;
                string Hash = "";
                if (child.Attribute("hash") == null)
                {
                    Hash = TreeHelper.GetHash(child.ToString());
                    child.SetAttributeValue("hash", Hash);
                }
                else
                {
                    Hash = child.Attribute("hash").Value;
                }

                string refName = child.Attribute("ref").Value.Split(new[] { '[' })[0];
                List<TreeElement> childTree = new List<TreeElement>();
                treeRoot.FindElements(refName, true, ref childTree, 1);
                childTree = (from childElem in childTree where childElem.Name == "/" || childElem.Dirty || childElem.Edi.Count > 0 select childElem).ToList();

                foreach (TreeElement childRoot in childTree)
                {
                    iChildCounter++;
                    IEdiObject childObject = ProcessSpecificTemplate(child, childRoot, objectMapping);

                    rootObject.AddChild(childObject);

                }
            }
            return rootObject;
        }
        protected string getValue(string pos, string edi)
        {
            if (_useCache && _valueCache.ContainsKey(edi))
            {
                if (_valueCache[edi].ContainsKey(pos))
                {
                    return _valueCache[edi][pos];
                }
            }
            edi = edi.Replace("??", "<<").Replace("?+", "?<").Replace("?:", "?>");
            if (pos == null || pos == "")
                return null;
            string[] Groups = edi.Split(new[] { "+" }, StringSplitOptions.None);
            string[] SubPos = pos.Split(new[] { ":" }, StringSplitOptions.None);
            if (!edi.StartsWith(SubPos[0]))
                return null;
            int GroupPos = int.Parse(SubPos[1]);
            if (Groups.Length <= GroupPos)
                return null;
            string[] SubGroups = Groups[GroupPos].Split(new[] { ":" }, StringSplitOptions.None);
            if (SubPos[2].Contains("("))
            {
                string[] range = SubPos[2].Split(new[] { "," }, StringSplitOptions.None);
                int start = int.Parse(range[0].Substring(1));
                int end = int.Parse(range[1].Substring(0, range[1].Length - 1));
                List<string> parts = new List<string>();
                for (int i = start; i <= end; i++)
                {
                    if (SubGroups.Length <= i)
                        break;
                    parts.Add(SubGroups[i].Replace("?<", "+").Replace("?>", ":").Replace("?$", "'").Replace("<<", "?"));
                }
                string endValue = string.Join("", parts).Trim();
                if (_useCache && !_valueCache.ContainsKey(edi))
                {
                    _valueCache.Add(edi, new Dictionary<string, string>());
                }
                if (_useCache)
                    _valueCache[edi][pos] = endValue;
                return endValue;
            }
            else
            {
                int DetailPos = int.Parse(SubPos[2]);
                if (SubGroups.Length <= DetailPos)
                    return null;
                string result = SubGroups[DetailPos].Replace("?<", "+").Replace("?>", ":").Replace("?$", "'").Replace("<<", "?");
                if (!_valueCache.ContainsKey(edi))
                {
                    _valueCache.Add(edi, new Dictionary<string, string>());
                }
                _valueCache[edi][pos] = result.Trim();
                return result.Trim();
            }
        }
        protected string EvaluateTemplate(string selector, TreeElement templateRoot, out string ediSegment)
        {
            //string[] sel_path= selector.Split(new char[]{'['});
            int klammerIndex = selector.IndexOf('[');
            if (klammerIndex == -1)
                klammerIndex = selector.Length;
            string selection = selector.Substring(0, klammerIndex);// sel_path[0];
            string path = null;
            //string[] sel_segments = selection.Split(new char[] { ':' });
            int sepIndex = selection.IndexOf(':');
            if (sepIndex == -1)
                sepIndex = selection.Length;
            string segment = selection.Substring(0, sepIndex);
            if (klammerIndex != selector.Length)
                path = selector.Substring(klammerIndex + 1, selector.Length - 1 - klammerIndex - 1);
            List<TreeElement> resultList = new List<TreeElement>();
            List<string> resultEDI = new List<string>();
            if (_useCache && _elementCache.ContainsKey(templateRoot))
            {
                if (_elementCache[templateRoot].ContainsKey(segment))
                {
                    resultEDI = _elementCache[templateRoot][segment];
                }
                else
                {
                    templateRoot.FindElements(segment, true, ref resultList, 2);
                    if ((from TreeElement res in resultList where res.Edi != null && res.Edi.Count > 0 select res).Count() > 0)
                    {

                        foreach (var result in resultList)
                        {
                            if (result.Edi.Count > 0)
                            {
                                foreach (var rEdi in result.Edi)
                                {
                                    //                                    if (!resultEDI.Contains(rEdi))
                                    {

                                        resultEDI.Add(rEdi);
                                    }
                                }
                            }
                        }
                        if (_useCache)
                            _elementCache[templateRoot].Add(segment, resultEDI);
                    }


                }
            }
            else
            {
                templateRoot.FindElements(segment, true, ref resultList, 2);
                if ((from TreeElement res in resultList where res.Edi != null && res.Edi.Count > 0 select res).Any())
                {
                    if (_useCache)
                        _elementCache.Add(templateRoot, new Dictionary<string, List<string>>());

                    foreach (var result in resultList)
                    {
                        if (result.Edi.Count > 0)
                        {
                            foreach (var rEdi in result.Edi)
                            {
                                //                                if (!resultEDI.Contains(rEdi))
                                {

                                    resultEDI.Add(rEdi);
                                }
                            }
                        }
                    }
                    if (_useCache)
                        _elementCache[templateRoot].Add(segment, resultEDI);
                }
            }
            ediSegment = null;
            if (resultEDI.Count == 0)
                return null;
            else if (resultEDI.Count >= 1)
            {
                List<string> ResultParts = new List<string>();
                foreach (string edi in resultEDI)
                {
                    if (path != null)
                    {
                        string[] paths = path.Split(new[] { '^', '|' }, StringSplitOptions.RemoveEmptyEntries);
                        List<string> searchEdi = new List<string>();
                        string Part = "";
                        bool condition_met = true;
                        if (path.Contains('|')) // Die OR-Verknüpfung erfordert eine andere Initialisierung
                            condition_met = false;
                        foreach (string temp_path in paths)
                        {
                            Part = "";
                            /* If we count more then two separators we already included the segment, so skip this*/
                            string seg_path = null;
                            if (temp_path.Split(new[] { ':' }).Length > 2)
                            {
                                seg_path = temp_path;
                                TreeElement searchElement = templateRoot.FindElement(seg_path.Split(new[] { ':' })[0], false);
                                if (searchElement != null)
                                    searchEdi.AddRange(searchElement.Edi);
                            }
                            else
                            {
                                seg_path = segment + ":" + temp_path;
                                searchEdi.Add(edi);
                            }
                            string sep_op = "=";
                            if (seg_path.Contains("!="))
                            {
                                sep_op = "!=";

                            }
                            int opIndex = seg_path.IndexOf(sep_op);
                            if (opIndex == -1)
                                opIndex = seg_path.Length;
                            string path_selector = seg_path.Substring(0, opIndex);
                            string path_value = null;
                            if (seg_path.Length != sepIndex)
                                path_value = seg_path.Substring(opIndex + sep_op.Length, seg_path.Length - opIndex - sep_op.Length);
                            else
                            {
                                path_value = "";
                            }
                            //.Where(s=>s==edi)
                            foreach (string edi_search in searchEdi)
                            {
                                if (edi_search.Substring(0, 3) == edi.Substring(0, 3) && edi_search != edi)
                                    continue;
                                if (sep_op == "=")
                                {
                                    if (getValue(path_selector, edi_search) == path_value)
                                    {
                                        //20120807: Geänder von edi_search auf edi um auch in CCI-CAV-Segmenten den Key richtig zu bestimmen
                                        Part = getValue(selection, edi);
                                        break;
                                    }
                                }
                                else
                                {
                                    if (getValue(path_selector, edi_search) != path_value)
                                    {
                                        //20120807: Geänder von edi_search auf edi um auch in CCI-CAV-Segmenten den Key richtig zu bestimmen
                                        Part = getValue(selection, edi);
                                        break;
                                    }
                                }
                            }
                            if (string.IsNullOrWhiteSpace(Part) == false)
                            {
                                if (path.Contains('|'))
                                {
                                    condition_met = true;
                                    break;
                                }

                            }
                            else
                            {
                                if (!path.Contains('|'))
                                {
                                    condition_met = false;
                                    break;
                                }
                            }
                        }
                        if (condition_met)
                        {
                            ediSegment = edi;
                            ResultParts.Add(Part);
                        }
                    }
                    else
                    {
                        ediSegment = edi;
                        ResultParts.Add(getValue(selection, edi));
                    }
                }
                if (ResultParts.Count > 0)
                    return string.Join("|", ResultParts);
                else
                    return null;
            }
            else
                return null;
        }
        public XElement LoadTemplate(string template)
        {
            
            StringReader reader = new StringReader(template);

            XElement root = XElement.Load(reader);
            return root;
        }
        protected bool _useCache = true;
        protected Dictionary<TreeElement, Dictionary<string, List<string>>> _elementCache = new Dictionary<TreeElement, Dictionary<string, List<string>>>();
        protected Dictionary<string, Dictionary<string, string>> _valueCache = new Dictionary<string, Dictionary<string, string>>();
        public IEdiObject LoadTemplateWithLoadedTree(XElement template, TreeElement tree)
        {
            Dictionary<string, List<TreeElement>> objectMapping = new Dictionary<string, List<TreeElement>>();
            var classes = from cls in template.DescendantsAndSelf("class")
                          select cls;
            _elementCache.Clear();
            _valueCache.Clear();
            XElement Dokument = (from temp in template.DescendantsAndSelf("class") where temp.Attribute("name").Value == "Dokument" select temp).Single();
            TreeElement docElement = null;
            foreach (XElement cls in classes)
            {
                if (TreeHelper.GetHash(cls.ToString()) == TreeHelper.GetHash(Dokument.ToString()))
                {
                    List<TreeElement> treeElements = new List<TreeElement>();
                    string refName = cls.Attribute("ref").Value.Split(new[] { '[' })[0];
                    tree.FindElements(refName, true, ref treeElements);
                    treeElements = (from childElem in treeElements where childElem.Name == "/" || childElem.Dirty || childElem.Edi.Count > 0 select childElem).ToList();
                    docElement = treeElements[0];
                }
                //List<TreeElement> treeElements = new List<TreeElement>();
                //string refName = cls.Attribute("ref").Value.Split(new char[] { '[' })[0];
                //tree.FindElements(refName, true, ref treeElements);
                //treeElements = (from childElem in treeElements where (childElem.Name == "/" || childElem.Dirty || childElem.Edi.Count > 0) select childElem).ToList<TreeElement>();
                //objectMapping[TreeHelper.GetHash(cls.ToString())] = treeElements;
            }
            return ProcessSpecificTemplate(Dokument, docElement, null);

        }
        public TreeElement LoadTree(string tree)
        {
            TreeElement treeRoot;
            string seperator = "\n";
            if (tree.IndexOf("\r\n") > -1)
            {
                seperator = "\r\n";
            }
            string[] lines = tree.Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
            treeRoot = new TreeElement("/[M;M]");

            TreeElement currentRoot = null;
            treeRoot.Parent = null;
            foreach (string line in lines)
            {
                string[] lineParts = line.Split(new[] { ':' });
                string[] elementParts = lineParts[1].Split(new[] { ',' });
                currentRoot = treeRoot.FindElement(lineParts[0]);
                bool first = true;
                foreach (string element in elementParts)
                {
                    TreeElement ele = new TreeElement(element);
                    if (ele.Name == "UNH")
                        ele.Dirty = true;
                    ele.Parent = currentRoot;
                    if (first)
                    {
                        ele.Key = true;
                        first = false;
                    }
                    currentRoot.Children.Add(ele.Name, ele);
                }
            }
            return treeRoot;
        }
        public TreeElement LoadEDI(string edi, TreeElement tree)
        {
            string elementDelimiter = ":";
            string groupDelimiter = "+";
            string segmentDelimiter = "'";
            int segDelimiterLength = 1;
            int UNAoffset = -1;
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
            message = message.Replace("?'", "?$");
            message = message.Replace("\"", "\\\"");
            var Segments = message.LowMemSplit(segmentDelimiter);
            if (tree != null)
            {
                TreeHelper.treeRoot = null;
                TreeElement currentTreeRoot = tree;
                foreach (string segment in Segments)
                {
                    string str_segment = segment.TrimStart('\r', '\n', '\t').TrimEnd('\r', '\n', '\t');
                    TreeElement child = TreeHelper.FindEdiElement(ref currentTreeRoot, str_segment);
                    if (child != null)
                        child.AddEdi(str_segment, child);
                    else
                        return tree;
                }
                return tree;
            }
            return null;

        }
    }
}
