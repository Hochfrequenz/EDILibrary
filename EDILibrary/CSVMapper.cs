using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                            builder.Append(String.Format("{0}/{1};", groupName, fieldName));
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
                        builder.Append(String.Format("{0};", fieldName));
                    }
                }
            }
        }
        public string CreateCSVTemplateFromJSON(string json)
        {
            JObject rootObject = JsonConvert.DeserializeObject<JObject>(json);
            StringBuilder builder = new StringBuilder();
            foreach (var step in ((rootObject.Property("steps").Value) as JObject).Properties())
            {
                ParseStep(step.Value.Value<JObject>(), builder);
            }
            return builder.ToString();
        }
        protected void BuildObjectFromSegment(string segment, string value, JObject localRoot)
        {
            if (segment.Contains("/"))
            {
                var segmentParts = segment.Split('/');
                string newSegment = String.Join("/", segmentParts.Skip(1));
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
            if (!segment.Contains("/"))
            {
                return segment;
            }
            return String.Join("/",segment.Split('/').Skip(1));
        }
        public List<string> CreateJSONFromCSV(string csv)
        {
            //first split header line from content lines
            var lines = csv.LowMemSplit("\r\n");
            var segments = lines[0].Split(';');
            List<string> returnList = new List<string>();
            foreach (var line in lines.Skip(1))
            {
                var lineSegments = line.Split(';');
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
                    BuildObjectFromSegment(RemoveStepFromSegment(segment), lineSegments[index], lineObject);
                    index++;
                }
                returnList.Add(JsonConvert.SerializeObject(dokumentObject));
            }
            return returnList;
        }
    }
}
