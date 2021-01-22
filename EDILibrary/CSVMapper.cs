using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EDILibrary
{
    public class CSVMapper
    {
        protected void ParseStep(JObject step, StringBuilder builder)
        {
            string stepName = step.Property("name").Value.Value<string>();
            foreach (JObject group in ((step.Property("groups").Value) as JArray))
            {
                string groupName = group.Property("key").Value.Value<string>();
                if (group.Property("fields") != null)
                {
                    foreach (JObject field in ((group.Property("fields").Value) as JArray))
                    {
                        if (field.Property("steps") != null)
                        {
                            foreach (var subStep in ((field.Property("steps").Value) as JObject).Properties())
                            {
                                ParseStep(subStep.Value.Value<JObject>(), builder);
                            }
                        }
                        else
                        {
                            string fieldName = field.Property("key").Value.Value<string>();
                            builder.Append(string.Format("{0}/{1};", groupName, fieldName));
                        }
                    }
                }
            }
            if (step.Property("fields") != null)
            {
                foreach (JObject field in ((step.Property("fields").Value) as JArray))
                {
                    if (field.Property("steps") != null)
                    {
                        foreach (var subStep in ((field.Property("steps").Value) as JObject).Properties())
                        {
                            ParseStep(subStep.Value.Value<JObject>(), builder);
                        }
                    }
                    else
                    {
                        string fieldName = field.Property("key").Value.Value<string>();
                        builder.Append(string.Format("{0};", fieldName));
                    }
                }
            }
        }
        protected void ParseProperty(JProperty prop, string prefix, StringBuilder builder, StringBuilder valueBuilder)
        {
            if (prop.Value.GetType() == typeof(JArray))
            {
                //if the array is non empty, process properties, otherwise continue
                if ((prop.Value as JArray).Count == 0)
                    return;

                foreach (var subProp in ((prop.Value as JArray)[0] as JObject).Properties())
                {
                    ParseProperty(subProp, prefix + prop.Name + "|", builder, valueBuilder);
                }
            }
            else if (prop.Value.GetType() == typeof(JObject))
            {
                foreach (var subProp in (prop.Value as JObject).Properties())
                {
                    builder.Append(prefix + prop.Name + "|" + subProp.Name + ";");
                }
            }
            else
            {
                builder.Append(prefix + prop.Name + ";");
                if (valueBuilder != null)
                    valueBuilder.Append(prop.Value.Value<string>() + ";");
            }
        }
        public string CreateCSVTemplateFromJSON(string json)
        {
            JObject rootObject = JsonConvert.DeserializeObject<JObject>(json);
            StringBuilder builder = new StringBuilder();
            StringBuilder valueBuilder = new StringBuilder();
            if (rootObject.Property("steps") != null)
            {
                foreach (var step in ((rootObject.Property("steps").Value) as JObject).Properties())
                {
                    ParseStep(step.Value.Value<JObject>(), builder);
                }
            }
            else
            {
                //navigate to first document and first message
                try
                {
                    rootObject = (rootObject.Property("Dokument").Value as JArray)[0] as JObject;
                    rootObject = (rootObject.Property("Nachricht").Value as JArray)[0] as JObject;
                }
                catch (Exception)
                {
                    return null;
                }
                foreach (var prop in rootObject.Properties())
                {
                    ParseProperty(prop, "", builder, valueBuilder);
                }
            }
            return builder.ToString().TrimEnd(';') + "\r\n" + valueBuilder.ToString().TrimEnd(';');
        }
        protected void BuildObjectFromSegment(string segment, string value, JObject localRoot)
        {
            if (segment.Contains("|"))
            {
                var segmentParts = segment.Split('|');
                string newSegment = string.Join("|", segmentParts.Skip(1));
                if (localRoot.Property(segmentParts[0]) == null)
                {
                    JArray newChilds = new JArray(new JObject());
                    localRoot.Add(segmentParts[0], newChilds);
                }
                BuildObjectFromSegment(newSegment, value, (localRoot.Property(segmentParts[0]).Value as JArray)[0] as JObject);
            }
            else
            {
                localRoot.Add(segment, value);
            }
        }
        protected string RemoveStepFromSegment(string segment)
        {
            if (!segment.Contains("|"))
            {
                return segment;
            }
            return string.Join("|", segment.Split('|').Skip(1));
        }
        public List<string> CreateJSONFromCSV(string csv)
        {
            //first split header line from content lines
            var lines = csv.LowMemSplit("\r\n"); // todo: make this more lenient to also accept  LF line endings.
            var segments = lines[0].Split(new string[] { ";" }, StringSplitOptions.None);
            List<string> returnList = new List<string>();
            foreach (var line in lines.Skip(1))
            {
                var lineSegments = line.Split(new string[] { ";" }, StringSplitOptions.None);
                int index = 0;
                JObject lineObject = new JObject();
                JObject nachrichtObject = new JObject
                {
                    { "Nachricht", new JArray(lineObject) }
                };
                JObject dokumentObject = new JObject
                {
                    { "Dokument", new JArray(nachrichtObject) }
                };
                foreach (var segment in segments)
                {
                    BuildObjectFromSegment(segment, lineSegments[index], lineObject);
                    index++;
                }
                returnList.Add(JsonConvert.SerializeObject(dokumentObject));
            }
            return returnList;
        }
    }
}
