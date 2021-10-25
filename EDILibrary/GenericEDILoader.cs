// Copyright (c) 2017 Hochfrequenz Unternehmensberatung GmbH
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace EDILibrary
{
    public class GenericEDILoader
    {
        protected virtual EdiObject ProcessSpecificTemplate(XElement template, Dictionary<string, List<TreeElement>> objectMapping)
        {
            var treeRoot = objectMapping[TreeHelper.GetHash(template.ToString())][0];
            return ProcessSpecificTemplate(template, treeRoot, objectMapping);
        }
        protected virtual EdiObject ProcessSpecificTemplate(XElement template, TreeElement treeRoot, Dictionary<string, List<TreeElement>> objectMapping)
        {
            string key = null;
            string ediSeg = null;
            if (template.Attribute("key") != null)
            {

                key = EvaluateTemplate(template.Attribute("key").Value, treeRoot, out ediSeg);

                if (key == null) // criteria not met
                    return null; // do not create child then

            }

            var rootObject = new EdiObject(template.Attribute("name").Value, template, key)
            {
                Edi = ediSeg
            };
            if (template.Attribute("migName") != null)
                rootObject.MigName = template.Attribute("migName").Value;
            var fields = from field in template.Elements("field") select field;
            foreach (var field in fields)
            {
                var selector = field.Attribute("ref").Value;
                if (field.Attribute("migName") != null)
                {
                    rootObject.MigFields[field.Attribute("name").Value] = field.Attribute("migName").Value;
                }
                var value = EvaluateTemplate(selector, treeRoot, out var ediSegment);
                rootObject.EdiFields[field.Attribute("name").Value] = ediSegment;
                if (value != null)
                {
                    if (rootObject.Fields.ContainsKey(field.Attribute("name").Value) == false)
                        rootObject.Fields[field.Attribute("name").Value] = new List<string>();
                    if (value.Contains("|"))
                    {
                        var subvalues = value.Split(new[] { '|' });
                        foreach (var val in subvalues)
                            rootObject.Fields[field.Attribute("name").Value].Add(val);
                    }
                    else
                    {
                        rootObject.Fields[field.Attribute("name").Value].Add(value);
                    }
                }
            }
            var children = from child in template.Elements("class") select child;

            foreach (var child in children)
            {
                //var iChildCounter = 0;
                var Hash = "";
                if (child.Attribute("hash") == null)
                {
                    Hash = TreeHelper.GetHash(child.ToString());
                    child.SetAttributeValue("hash", Hash);
                }

                var refName = child.Attribute("ref").Value.Split(new[] { '[' })[0];
                var childTree = new List<TreeElement>();
                treeRoot.FindElements(refName, true, ref childTree, 1);
                childTree = (from childElem in childTree where childElem.Name == "/" || childElem.Dirty || childElem.Edi.Count > 0 select childElem).ToList();

                foreach (var childObject in childTree.Select(childRoot => ProcessSpecificTemplate(child, childRoot, objectMapping)))
                {
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
            if (string.IsNullOrEmpty(pos))
                return null;
            var groups = edi.Split(new[] { "+" }, StringSplitOptions.None);
            var subPos = pos.Split(new[] { ":" }, StringSplitOptions.None);
            if (!edi.StartsWith(subPos[0]))
                return null;
            var groupPos = int.Parse(subPos[1]);
            if (groups.Length <= groupPos)
                return null;
            var subGroups = groups[groupPos].Split(new[] { ":" }, StringSplitOptions.None);
            if (subPos[2].Contains("("))
            {
                var range = subPos[2].Split(new[] { "," }, StringSplitOptions.None);
                var start = int.Parse(range[0].Substring(1));
                var end = int.Parse(range[1].Substring(0, range[1].Length - 1));
                var parts = new List<string>();
                for (var i = start; i <= end; i++)
                {
                    if (subGroups.Length <= i)
                        break;
                    parts.Add(subGroups[i].Replace("?<", "+").Replace("?>", ":").Replace("?$", "'").Replace("<<", "?"));
                }
                var endValue = string.Join("", parts).Trim();
                if (_useCache && !_valueCache.ContainsKey(edi))
                {
                    _valueCache.Add(edi, new Dictionary<string, string>());
                }
                if (_useCache)
                    _valueCache[edi][pos] = endValue;
                return endValue;
            }

            var DetailPos = int.Parse(subPos[2]);
            if (subGroups.Length <= DetailPos)
                return null;
            var result = subGroups[DetailPos].Replace("?<", "+").Replace("?>", ":").Replace("?$", "'").Replace("<<", "?");
            if (!_valueCache.ContainsKey(edi))
            {
                _valueCache.Add(edi, new Dictionary<string, string>());
            }
            _valueCache[edi][pos] = result.Trim();
            return result.Trim();
        }
        protected string EvaluateTemplate(string selector, TreeElement templateRoot, out string ediSegment)
        {
            //string[] sel_path= selector.Split(new char[]{'['});
            var klammerIndex = selector.IndexOf('[');
            if (klammerIndex == -1)
                klammerIndex = selector.Length;
            var selection = selector.Substring(0, klammerIndex);// sel_path[0];
            string path = null;
            //string[] sel_segments = selection.Split(new char[] { ':' });
            var sepIndex = selection.IndexOf(':');
            if (sepIndex == -1)
                sepIndex = selection.Length;
            var segment = selection.Substring(0, sepIndex);
            if (klammerIndex != selector.Length)
                path = selector.Substring(klammerIndex + 1, selector.Length - 1 - klammerIndex - 1);
            var resultList = new List<TreeElement>();
            var resultEDI = new List<string>();
            if (_useCache && _elementCache.ContainsKey(templateRoot))
            {
                if (_elementCache[templateRoot].ContainsKey(segment))
                {
                    resultEDI = _elementCache[templateRoot][segment];
                }
                else
                {
                    templateRoot.FindElements(segment, true, ref resultList, 2);
                    if ((from TreeElement res in resultList where res.Edi != null && res.Edi.Any() select res).Any())
                    {

                        foreach (var result in resultList)
                        {
                            if (result.Edi.Any())
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
                if ((from TreeElement res in resultList where res.Edi != null && res.Edi.Any() select res).Any())
                {
                    if (_useCache)
                        _elementCache.Add(templateRoot, new Dictionary<string, List<string>>());

                    resultEDI.AddRange(resultList.Where(result => result.Edi.Any()).SelectMany(result => result.Edi));
                    if (_useCache)
                        _elementCache[templateRoot].Add(segment, resultEDI);
                }
            }
            ediSegment = null;
            if (resultEDI.Count == 0)
            {
                return null;
            }

            if (resultEDI.Count >= 1)
            {
                var resultParts = new List<string>();
                foreach (var edi in resultEDI)
                {
                    if (path != null)
                    {
                        var paths = path.Split(new[] { '^', '|' }, StringSplitOptions.RemoveEmptyEntries);
                        var searchEdi = new List<string>();
                        var part = "";
                        var conditionMet = true;
                        if (path.Contains('|')) // Die OR-Verknüpfung erfordert eine andere Initialisierung
                            conditionMet = false;
                        foreach (var tempPath in paths)
                        {
                            part = "";
                            /* If we count more then two separators we already included the segment, so skip this*/
                            string segPath; //= null;
                            if (tempPath.Split(new[] { ':' }).Length > 2)
                            {
                                segPath = tempPath;
                                var searchElement = templateRoot.FindElement(segPath.Split(new[] { ':' })[0], false);
                                if (searchElement != null)
                                    searchEdi.AddRange(searchElement.Edi);
                            }
                            else
                            {
                                segPath = segment + ":" + tempPath;
                                searchEdi.Add(edi);
                            }

                            var sepOp = "=";
                            if (segPath.Contains("!="))
                            {
                                sepOp = "!=";
                            }

                            var opIndex = segPath.IndexOf(sepOp); // todo culture specific
                            if (opIndex == -1)
                                opIndex = segPath.Length;
                            var pathSelector = segPath.Substring(0, opIndex);
                            string pathValue = null;
                            if (segPath.Length != sepIndex)
                                pathValue = segPath.Substring(opIndex + sepOp.Length, segPath.Length - opIndex - sepOp.Length);
                            else
                            {
                                pathValue = "";
                            }

                            //.Where(s=>s==edi)
                            foreach (var ediSearch in searchEdi.Where(ediSearch => ediSearch.Substring(0, 3) != edi.Substring(0, 3) || ediSearch == edi))
                            {
                                if (sepOp == "=")
                                {
                                    if (getValue(pathSelector, ediSearch) == pathValue)
                                    {
                                        //20120807: Geänder von edi_search auf edi um auch in CCI-CAV-Segmenten den Key richtig zu bestimmen
                                        part = getValue(selection, edi);
                                        break;
                                    }
                                }
                                else
                                {
                                    if (getValue(pathSelector, ediSearch) != pathValue)
                                    {
                                        //20120807: Geänder von edi_search auf edi um auch in CCI-CAV-Segmenten den Key richtig zu bestimmen
                                        part = getValue(selection, edi);
                                        break;
                                    }
                                }
                            }

                            if (string.IsNullOrWhiteSpace(part) == false)
                            {
                                if (path.Contains('|'))
                                {
                                    conditionMet = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (!path.Contains('|'))
                                {
                                    conditionMet = false;
                                    break;
                                }
                            }
                        }

                        if (conditionMet)
                        {
                            ediSegment = edi;
                            resultParts.Add(part);
                        }
                    }
                    else
                    {
                        ediSegment = edi;
                        resultParts.Add(getValue(selection, edi));
                    }
                }

                if (resultParts.Any())
                    return string.Join("|", resultParts);
                return null;
            }

            return null;
        }
        public static XElement LoadTemplate(string template)
        {
            var reader = new StringReader(template);
            var root = XElement.Load(reader);
            return root;
        }

        protected bool _useCache = true;
        protected Dictionary<TreeElement, Dictionary<string, List<string>>> _elementCache = new Dictionary<TreeElement, Dictionary<string, List<string>>>();
        protected Dictionary<string, Dictionary<string, string>> _valueCache = new Dictionary<string, Dictionary<string, string>>();
        public EdiObject LoadTemplateWithLoadedTree(XElement template, TreeElement tree)
        {
            var objectMapping = new Dictionary<string, List<TreeElement>>();
            var classes = from cls in template.DescendantsAndSelf("class")
                          select cls;
            _elementCache.Clear();
            _valueCache.Clear();
            var dokument = (from temp in template.DescendantsAndSelf("class") where temp.Attribute("name").Value == "Dokument" select temp).Single();
            TreeElement docElement = null;
            foreach (var cls in classes)
            {
                if (TreeHelper.GetHash(cls.ToString()) == TreeHelper.GetHash(dokument.ToString()))
                {
                    var treeElements = new List<TreeElement>();
                    var refName = cls.Attribute("ref").Value.Split(new[] { '[' })[0];
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
            return ProcessSpecificTemplate(dokument, docElement, null);

        }
        public static TreeElement LoadTree(string tree)
        {
            TreeElement treeRoot;
            var seperator = "\n";
            if (tree.IndexOf(Environment.NewLine) > -1)
            {
                seperator = Environment.NewLine;
            }
            var lines = tree.Split(new[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
            treeRoot = new TreeElement("/[M;M]");

            TreeElement currentRoot = null;
            treeRoot.Parent = null;
            foreach (var line in lines)
            {
                var lineParts = line.Split(new[] { ':' });
                var elementParts = lineParts[1].Split(new[] { ',' });
                currentRoot = treeRoot.FindElement(lineParts[0]);
                var first = true;
                foreach (var element in elementParts)
                {
                    var ele = new TreeElement(element);
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
        public static TreeElement LoadEDI(string edi, TreeElement tree)
        {
            var elementDelimiter = ":";
            var groupDelimiter = "+";
            var segmentDelimiter = "'";
            var segDelimiterLength = 1;
            var unAoffset = -1;
            if (edi.StartsWith("UNA"))
            {
                var una = edi.Substring(0, 9);
                unAoffset = 8;
                elementDelimiter = una.Substring(3, 1);
                groupDelimiter = una.Substring(4, 1);
                segmentDelimiter = una.Substring(8, 1);
                segDelimiterLength = segmentDelimiter.Length;
                if (segmentDelimiter == "\r" && edi.IndexOf(Environment.NewLine) > -1)
                    segmentDelimiter = Environment.NewLine;
            }

            var message = edi.Substring(unAoffset + segDelimiterLength, edi.Length - (unAoffset + segDelimiterLength));
            message = message.Replace("?'", "?$");
            message = message.Replace("\"", "\\\"");
            if (tree != null)
            {
                TreeHelper.treeRoot = null;
                var currentTreeRoot = tree;
                foreach (var segment in message.SplitLines(segmentDelimiter.First()))
                {
                    var strSegment = segment.Line.ToString().TrimStart('\r', '\n', '\t').TrimEnd('\r', '\n', '\t');
                    var child = TreeHelper.FindEdiElement(ref currentTreeRoot, strSegment);
                    if (child != null)
                        child.AddEdi(strSegment, child);
                    else
                        return tree;
                }
                return tree;
            }
            return null;

        }
    }
}
