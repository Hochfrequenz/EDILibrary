using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace EDILibrary
{

    [Obsolete("Use " + nameof(EdiObject) + " instead.", true)]
    // ReSharper disable once InconsistentNaming
    public class IEdiObject
    {
    }

    [DataContract]
    public class EdiObject
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string MigName { get; set; }
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Edi { get; set; }
        public EdiObject Parent { get; private set; }
        XElement template;
        [DataMember]
        public List<EdiObject> Children { get; set; }
        public List<EdiObject> SelfOrChildren
        {
            get { var list = Children.Take(Children.Count).ToList(); list.Add(this); return list; }
        }
        [DataMember]
        public Dictionary<string, List<string>> Fields { get; set; }
        [DataMember]
        public Dictionary<string, string> MigFields { get; set; }
        [DataMember]
        public Dictionary<string, string> EdiFields { get; set; }
        public EdiObject(EDIEnums enumValue, XElement temp, string keyValue) : this(EDIEnumHelper.GetDescription(enumValue), temp, keyValue)
        {

        }
        public EdiObject(string name, XElement temp, string keyValue)
        {
            Name = name;
            Children = new List<EdiObject>();
            Fields = new Dictionary<string, List<string>>();
            MigFields = new Dictionary<string, string>();
            EdiFields = new Dictionary<string, string>();
            template = temp;
            Key = keyValue;

        }
        protected static string GenerateKey(string objectName)
        {
            switch (objectName)
            {
                case "Dokument":
                case "Nachricht":
                case "Vorgang":
                    {
                        return Guid.NewGuid().ToString("N").Substring(0, 13).ToUpper();
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        protected void ParseJSON(dynamic json)
        {

            foreach (var child in json.Children<JProperty>())
            {
                if (child.Value.GetType() == typeof(JArray))
                {
                    foreach (var childValue in child.Value)
                    {
                        var childNode = new EdiObject(child.Name, null, GenerateKey(child.Name));
                        childNode.ParseJSON(childValue);
                        if (childNode.Fields.Any() || childNode.Children.Any())
                            AddChild(childNode);
                    }
                }
                else
                {
                    if (child.Value != null && child.Value is JObject)
                    {
                        //go through the child properties and dot "." them together
                        foreach (var childprop in (child.Value as JObject).Properties())
                        {
                            //only allow one nest level, so only strings allowed here
                            if (childprop.Value != null && (childprop.Value.Type == JTokenType.String || childprop.Value.Type == JTokenType.Integer))
                            {
                                Fields.Add(child.Name + "." + childprop.Name, new List<string> { childprop.Value.Value<string>() });
                            }
                        }
                    }
                    else if (!(child.Value == null || child.Type == JTokenType.Null))
                        Fields.Add(child.Name, new List<string> { child.Value.ToString() });
                }
            }

        }
        public static EdiObject CreateFromJSON(string JSON)
        {
            dynamic json = JsonConvert.DeserializeObject(JSON);
            var doc = json["Dokument"][0];
            var root = new EdiObject(EDIEnums.Dokument, null, GenerateKey("Dokument"));
            root.ParseJSON(doc);
            return root;
        }

        public override string ToString()
        {
            return Name + "  " + Key;
        }
        protected static void Recurse(XElement elem, EdiObject child)
        {
            foreach (var (key, value) in child.Fields)
            {
                var f = new XElement("Field");
                f.SetAttributeValue("name", key);
                f.Value = string.Join("|", value);
                elem.Add(f);
            }
            foreach (var cl in child.Children)
            {
                var ch = new XElement("Class");
                ch.SetAttributeValue("name", cl.Name);
                ch.SetAttributeValue("key", cl.Key);
                Recurse(ch, cl);
                elem.Add(ch);
            }
        }
        protected static string escapeSpecialChars(string value)
        {
            return value.Replace("\"", "\\\"");
        }
        protected StringBuilder _builder;
        protected void RecurseJSON(EdiObject cur)
        {
            var hasKey = false;

            var i = cur.Fields.Count;
            var hasClass = cur.Children.Any();
            if (cur.Key != null)
            {
                hasKey = true;
                _builder.AppendLine("\"" + "Key" + "\" : \"" + cur.Key + "\"" + (i != 0 || hasClass ? "," : ""));
            }
            if (cur.Fields.Count(f => f.Value.Count > 1) == cur.Fields.Count && cur.Fields.Any(f => f.Value.Count > 1)) // check for multiple values
            {
                var index = 0;
                var oldI = i;
                while (index < cur.Fields.First(f => f.Value.Count > 1).Value.Count)
                {
                    foreach (var (key, value) in cur.Fields)
                    {
                        i--;
                        _builder.AppendLine("\"" + key + "\" : \"" + escapeSpecialChars(value.ElementAt(index)) + "\"" + (i != 0 || hasClass ? "," : ""));
                    }
                    i = oldI;
                    if (index + 1 < cur.Fields.First(f => f.Value.Count > 1).Value.Count)
                        _builder.AppendLine("},{");
                    index++;
                }
            }
            else
            {
                foreach (var (key, value) in cur.Fields)
                {
                    i--;
                    _builder.AppendLine("\"" + key + "\" : \"" + escapeSpecialChars(value.FirstOrDefault()) + "\"" + (i != 0 || hasClass ? "," : ""));

                }
            }
            var j = cur.Children.Count;
            if (j == 0 && cur.Fields.Count == 0) // bei keinen fields und classes einen Key einfügen (momentan nur für Kontakt notwendig)
            {
                if (!hasKey)
                {
                    string key;// = "";
                    if (cur.Key != null)
                    {
                        key = "\"" + "Key" + "\" : \"" + cur.Key + "\"" + (i != 0 || hasClass ? "," : "");
                    }
                    else
                    {
                        key = "\"" + "Key" + "\" : \"" + "" + "\"" + (i != 0 || hasClass ? "," : "");
                    }
                    _builder.AppendLine(key);
                }

            }
            var lastName = "";
            var openElement = false;
            foreach (var elem in cur.Children)
            {
                if (elem.Name != lastName && lastName != "")
                {
                    _builder.AppendLine("]" + (j != 0 ? "," : ""));
                    openElement = false;
                }
                else if (lastName != "")
                    _builder.AppendLine(",{");
                j--;
                if (elem.Name != lastName)
                {
                    _builder.AppendLine("\"" + elem.Name + "\" :  [{");
                    openElement = true;
                }
                RecurseJSON(elem);
                _builder.AppendLine("}");
                lastName = elem.Name;
            }
            if (openElement)
                _builder.AppendLine("]" + (j != 0 ? "," : ""));
        }
        protected void RecurseJSON(XElement cur)
        {
            var hasKey = false;

            var i = cur.Descendants("Field").Count(d => d.Parent == cur);
            var hasClass = cur.Descendants("Class").Any(d => d.Parent == cur);
            if (cur.Attribute("key") != null)
            {
                hasKey = true;
                _builder.AppendLine("\"" + "Key" + "\" : \"" + cur.Attribute("key").Value + "\"" + (i != 0 || hasClass ? "," : ""));
            }
            foreach (var elem in cur.Descendants("Field").Where(d => d.Parent == cur))
            {
                i--;
                _builder.AppendLine("\"" + elem.Attribute("name").Value + "\" : \"" + elem.Value + "\"" + (i != 0 || hasClass ? "," : ""));

            }
            var j = cur.Descendants("Class").Count(d => d.Parent == cur);
            if (j == 0 && cur.Descendants("Field").All(d => d.Parent != cur)) // bei keinen fields und classes einen Key einfügen (momentan nur für Kontakt notwendig)
            {
                if (!hasKey)
                {
                    string key;// = "";
                    if (cur.Attribute("key") != null)
                    {
                        key = "\"" + "Key" + "\" : \"" + cur.Attribute("key").Value + "\"" + (i != 0 || hasClass ? "," : "");
                    }
                    else
                    {
                        key = "\"" + "Key" + "\" : \"" + "" + "\"" + (i != 0 || hasClass ? "," : "");
                    }
                    _builder.AppendLine(key);
                }

            }
            foreach (var elem in cur.Descendants("Class").Where(d => d.Parent == cur))
            {
                j--;
                _builder.AppendLine("\"" + elem.Attribute("name").Value + "\" :  [{");
                RecurseJSON(elem);
                _builder.AppendLine("}]" + (j != 0 ? "," : ""));
            }
        }
        public string Serialize()
        {
            var root = new XElement("EDIFACT");
            Recurse(root, this);
            return root.ToString();
        }

        public string SerializeToJSON()
        {
            _builder = new StringBuilder();
            _builder.AppendLine("{");
            _builder.AppendLine("\"Dokument\":[{");
            RecurseJSON(this);
            _builder.AppendLine("}]}");
            return _builder.ToString();
        }
        public bool ContainsField(string name)
        {
            return Fields.ContainsKey(name);
        }
        public bool ContainsField(EDIEnums enumValue)
        {
            return Fields.ContainsKey(EDIEnumHelper.GetDescription(enumValue));
        }
        public string[] FieldList(EDIEnums enumValue)
        {
            if (Fields.ContainsKey(EDIEnumHelper.GetDescription(enumValue)))
            {
                return Fields[EDIEnumHelper.GetDescription(enumValue)].ToArray<string>();
            }

            return null;
        }
        public string[] FieldList(string name)
        {
            if (Fields.ContainsKey(name))
            {
                return Fields[name].ToArray<string>();
            }

            return null;
        }
        public string Field(EDIEnums enumValue)
        {
            if (Fields.ContainsKey(EDIEnumHelper.GetDescription(enumValue)))
            {
                return Fields[EDIEnumHelper.GetDescription(enumValue)].First();
            }

            return null;
        }
        public string Field(string name)
        {
            if (Fields.ContainsKey(name))
            {
                return Fields[name].First();
            }

            return null;
        }

        public string Field(EDIEnums enumValue, int index)
        {
            if (Fields.ContainsKey(EDIEnumHelper.GetDescription(enumValue)))
            {
                return Fields[EDIEnumHelper.GetDescription(enumValue)][index];
            }

            return null;
        }
        public void AddField(EDIEnums enumValue, string value)
        {
            if (Fields.ContainsKey(EDIEnumHelper.GetDescription(enumValue)))
            {
                Fields[EDIEnumHelper.GetDescription(enumValue)] = new List<string> { value };
            }
            else
            {
                Fields.Add(EDIEnumHelper.GetDescription(enumValue), new List<string> { value });
            }
        }
        public EdiObject Child(EDIEnums name, string key)
        {
            return Child(EDIEnumHelper.GetDescription(name), key);
        }
        public EdiObject Child(EDIEnums name)
        {
            return Child(EDIEnumHelper.GetDescription(name));
        }
        public EdiObject Child(string name, string key)
        {
            return (from child in Children where child.Name == name && child.Key == key select child).FirstOrDefault();
        }
        public EdiObject Child(string name)
        {
            return (from child in Children where child.Name == name select child).FirstOrDefault();
        }
        public void AddChild(EdiObject child)
        {
            if (child == null)
                return;
            child.Parent = this;
            Children.Add(child);
        }
        public bool ContainsChild(EDIEnums name, string key)
        {
            return ContainsChild(EDIEnumHelper.GetDescription(name), key);
        }
        public bool ContainsChild(EDIEnums name)
        {
            return ContainsChild(EDIEnumHelper.GetDescription(name));
        }
        public bool ContainsChild(string name, string key)
        {
            return (from child in Children where child.Name == name && child.Key == key select child).Any();
        }
        public bool ContainsChild(string name)
        {
            return (from child in Children where child.Name == name select child).Any();
        }
        public void RemoveField(string name)
        {
            Fields.Remove(name);
        }
        public void RemoveChilds(EDIEnums name)
        {
            RemoveChilds(EDIEnumHelper.GetDescription(name));
        }
        public void RemoveChilds(string name)
        {
            var children = (from child in Children where child.Name == name select child).ToList();
            foreach (var child in children)
            {
                Children.Remove(child);
            }
        }
        public List<EdiObject> Childs(EDIEnums name)
        {
            return Childs(EDIEnumHelper.GetDescription(name));
        }
        public List<EdiObject> Childs(string name)
        {
            return (from child in Children where child.Name == name select child).ToList();
        }
        public EdiObject Clone()
        {
            var clone = new EdiObject(Name, null, Key);
            foreach (var child in Children)
            {
                clone.AddChild(child.Clone());
            }
            foreach (var (key, value) in Fields)
            {
                clone.Fields.Add(key, new List<string>(value));
            }
            return clone;
        }
    }
}
