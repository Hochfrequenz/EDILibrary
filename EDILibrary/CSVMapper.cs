using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EDILibrary
{
    public class CSVMapper
    {
        protected static void ParseStep(JObject step, StringBuilder builder)
        {
            var stepName = step.Property("name").Value.Value<string>();
            foreach (JObject group in step.Property("groups").Value as JArray)
            {
                var groupName = group.Property("key").Value.Value<string>();
                if (group.Property("fields") != null)
                {
                    foreach (JObject field in @group.Property("fields").Value as JArray)
                    {
                        if (field.Property("steps") != null)
                        {
                            foreach (
                                var subStep in (
                                    field.Property("steps").Value as JObject
                                ).Properties()
                            )
                            {
                                ParseStep(subStep.Value.Value<JObject>(), builder);
                            }
                        }
                        else
                        {
                            var fieldName = field.Property("key").Value.Value<string>();
                            builder.Append(string.Format("{0}/{1};", groupName, fieldName));
                        }
                    }
                }
            }
            if (step.Property("fields") != null)
            {
                foreach (JObject field in step.Property("fields").Value as JArray)
                {
                    if (field.Property("steps") != null)
                    {
                        foreach (
                            var subStep in (field.Property("steps").Value as JObject).Properties()
                        )
                        {
                            ParseStep(subStep.Value.Value<JObject>(), builder);
                        }
                    }
                    else
                    {
                        var fieldName = field.Property("key").Value.Value<string>();
                        builder.Append(string.Format("{0};", fieldName));
                    }
                }
            }
        }

        protected static void ParseProperty(
            JProperty prop,
            string prefix,
            StringBuilder builder,
            StringBuilder valueBuilder
        )
        {
            if (prop.Value.GetType() == typeof(JArray))
            {
                //if the array is non empty, process properties, otherwise continue
                if (!(prop.Value as JArray).Any())
                {
                    return;
                }

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
                {
                    valueBuilder.Append(prop.Value.Value<string>() + ";");
                }
            }
        }

        public static string CreateCSVTemplateFromJSON(string json)
        {
            var rootObject = JsonConvert.DeserializeObject<JObject>(json);
            var builder = new StringBuilder();
            var valueBuilder = new StringBuilder();
            if (rootObject.Property("steps") != null)
            {
                foreach (var step in (rootObject.Property("steps").Value as JObject).Properties())
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
                catch (Exception) // todo: no pokemon catching
                {
                    return null;
                }
                foreach (var prop in rootObject.Properties())
                {
                    ParseProperty(prop, "", builder, valueBuilder);
                }
            }
            return builder.ToString().TrimEnd(';')
                + Environment.NewLine
                + valueBuilder.ToString().TrimEnd(';');
        }

        protected static void BuildObjectFromSegment(
            string segment,
            string value,
            JObject localRoot
        )
        {
            while (true)
            {
                if (segment.Contains("|"))
                {
                    var segmentParts = segment.Split('|');
                    var newSegment = string.Join("|", segmentParts.Skip(1));
                    if (localRoot.Property(segmentParts[0]) == null)
                    {
                        var newChilds = new JArray(new JObject());
                        localRoot.Add(segmentParts[0], newChilds);
                    }

                    segment = newSegment;
                    localRoot = (localRoot.Property(segmentParts[0]).Value as JArray)[0] as JObject;
                    continue;
                }

                localRoot.Add(segment, value);

                break;
            }
        }

        protected static string RemoveStepFromSegment(string segment)
        {
            return !segment.Contains("|") ? segment : string.Join("|", segment.Split('|').Skip(1));
        }

        public static List<string> CreateJSONFromCSV(string csv)
        {
            //first split header line from content lines
            var lines = csv.LowMemSplit(Environment.NewLine);
            var segments = lines[0].Split(new[] { ";" }, StringSplitOptions.None);
            var returnList = new List<string>();
            foreach (var line in lines.Skip(1))
            {
                var lineSegments = line.Split(new[] { ";" }, StringSplitOptions.None);
                var index = 0;
                var lineObject = new JObject();
                var nachrichtObject = new JObject { { "Nachricht", new JArray(lineObject) } };
                var dokumentObject = new JObject { { "Dokument", new JArray(nachrichtObject) } };
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
