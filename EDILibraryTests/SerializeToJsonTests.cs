using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using AwesomeAssertions;
using EDILibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace EDILibraryTests
{
    [TestClass]
    public class SerializeToJsonTests
    {
        [TestMethod]
        public void SerializeToJSON_WithNewlineInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Freitext"] = new List<string> { "Zeile 1\nZeile 2" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            root.TryGetProperty("Dokument", out _).Should().BeTrue();
        }

        [TestMethod]
        public void SerializeToJSON_WithTabInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Text"] = new List<string> { "Spalte1\tSpalte2" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            root.TryGetProperty("Dokument", out _).Should().BeTrue();
        }

        [TestMethod]
        public void SerializeToJSON_WithCarriageReturnInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Text"] = new List<string> { "Zeile 1\rZeile 2" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            root.TryGetProperty("Dokument", out _).Should().BeTrue();
        }

        [TestMethod]
        public void SerializeToJSON_WithBackslashInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Path"] = new List<string> { @"C:\Users\test" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            root.TryGetProperty("Dokument", out _).Should().BeTrue();
        }

        [TestMethod]
        public void SerializeToJSON_WithControlCharInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Data"] = new List<string> { "text\u000Bmore" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            root.TryGetProperty("Dokument", out _).Should().BeTrue();
        }

        [TestMethod]
        public void SerializeToJSON_WithQuoteInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Name"] = new List<string> { "Max \"Mustermann\"" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            root.TryGetProperty("Dokument", out _).Should().BeTrue();
        }

        [TestMethod]
        public void SerializeToJSON_WithMultipleControlCharsInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Freitext"] = new List<string>
            {
                "Zeile 1\nZeile 2\n\r\nTab:\tEnde\\Backslash",
            };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            root.TryGetProperty("Dokument", out _).Should().BeTrue();
        }

        [TestMethod]
        public void SerializeToJSON_SimpleFieldsAndKey_ProducesExpectedStructure()
        {
            var obj = new EdiObject("Test", new XElement("root"), "abc");
            obj.Fields["Name"] = new List<string> { "Wert" };
            obj.Fields["Zaehler"] = new List<string> { "42" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var elem = doc.RootElement.GetProperty("Dokument")[0];
            elem.GetProperty("Key").GetString().Should().Be("abc");
            elem.GetProperty("Name").GetString().Should().Be("Wert");
            elem.GetProperty("Zaehler").GetString().Should().Be("42");
        }

        [TestMethod]
        public void SerializeToJSON_ChildrenGroupedByName_ProducesExpectedStructure()
        {
            var obj = new EdiObject("Root", new XElement("root"), "r1");
            var child1 = new EdiObject("Segment", new XElement("seg"), "s1");
            child1.Fields["Code"] = new List<string> { "A" };
            var child2 = new EdiObject("Segment", new XElement("seg"), "s2");
            child2.Fields["Code"] = new List<string> { "B" };
            obj.AddChild(child1);
            obj.AddChild(child2);

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var segments = doc.RootElement.GetProperty("Dokument")[0].GetProperty("Segment");
            segments.GetArrayLength().Should().Be(2);
            segments[0].GetProperty("Code").GetString().Should().Be("A");
            segments[1].GetProperty("Code").GetString().Should().Be("B");
        }

        [TestMethod]
        public void SerializeToJSON_NoFieldsNoChildren_ProducesEmptyKey()
        {
            var obj = new EdiObject("Kontakt", new XElement("root"), null);

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var elem = doc.RootElement.GetProperty("Dokument")[0];
            elem.GetProperty("Key").GetString().Should().Be("");
        }

        [TestMethod]
        public void SerializeToJSON_MultiValueFields_ProducesMultipleObjects()
        {
            var obj = new EdiObject("Multi", new XElement("root"), "m1");
            obj.Fields["A"] = new List<string> { "a1", "a2" };
            obj.Fields["B"] = new List<string> { "b1", "b2" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var arr = doc.RootElement.GetProperty("Dokument");
            arr.GetArrayLength().Should().Be(2);
            arr[0].GetProperty("Key").GetString().Should().Be("m1");
            arr[0].GetProperty("A").GetString().Should().Be("a1");
            arr[0].GetProperty("B").GetString().Should().Be("b1");
            arr[1].GetProperty("A").GetString().Should().Be("a2");
            arr[1].GetProperty("B").GetString().Should().Be("b2");
        }

        [TestMethod]
        public void SerializeToJSON_MultiValueWithChildren_ChildrenAttachedToLastObject()
        {
            var obj = new EdiObject("Multi", new XElement("root"), "m1");
            obj.Fields["A"] = new List<string> { "a1", "a2" };
            obj.Fields["B"] = new List<string> { "b1", "b2" };
            var child = new EdiObject("Child", new XElement("c"), "c1");
            child.Fields["X"] = new List<string> { "x1" };
            obj.AddChild(child);

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var arr = doc.RootElement.GetProperty("Dokument");
            arr.GetArrayLength().Should().Be(2);
            arr[1].GetProperty("A").GetString().Should().Be("a2");
            arr[1].GetProperty("Child")[0].GetProperty("X").GetString().Should().Be("x1");
        }

        [TestMethod]
        [DataRow("SimpleFieldsAndKey")]
        [DataRow("ChildrenGroupedByName")]
        [DataRow("NoFieldsNoChildren")]
        [DataRow("MultiValueFields")]
        [DataRow("MultiValueWithChildren")]
        public void SerializeToJSON_MatchesSnapshot(string snapshotName)
        {
            var snapshot = LoadSnapshot(snapshotName);
            var newOutput = GetNewOutput(snapshotName);

            JToken
                .DeepEquals(snapshot, newOutput)
                .Should()
                .BeTrue(
                    $"New output for '{snapshotName}' differs from snapshot.\nSnapshot: {snapshot}\nNew:      {newOutput}"
                );
        }

        private static JToken LoadSnapshot(string name)
        {
            var path = Path.GetFullPath(Path.Combine("../../../Snapshots", name + ".json"));
            var json = File.ReadAllText(path);
            return JToken.Parse(json);
        }

        private static JToken GetNewOutput(string name)
        {
            EdiObject obj = name switch
            {
                "SimpleFieldsAndKey" => CreateSimpleFieldsAndKey(),
                "ChildrenGroupedByName" => CreateChildrenGroupedByName(),
                "NoFieldsNoChildren" => CreateNoFieldsNoChildren(),
                "MultiValueFields" => CreateMultiValueFields(),
                "MultiValueWithChildren" => CreateMultiValueWithChildren(),
                _ => throw new System.ArgumentException($"Unknown snapshot: {name}"),
            };
            return JToken.Parse(obj.SerializeToJSON());
        }

        private static EdiObject CreateSimpleFieldsAndKey()
        {
            var obj = new EdiObject("Test", new XElement("root"), "abc");
            obj.Fields["Name"] = new List<string> { "Wert" };
            obj.Fields["Zaehler"] = new List<string> { "42" };
            return obj;
        }

        private static EdiObject CreateChildrenGroupedByName()
        {
            var obj = new EdiObject("Root", new XElement("root"), "r1");
            var child1 = new EdiObject("Segment", new XElement("seg"), "s1");
            child1.Fields["Code"] = new List<string> { "A" };
            var child2 = new EdiObject("Segment", new XElement("seg"), "s2");
            child2.Fields["Code"] = new List<string> { "B" };
            obj.AddChild(child1);
            obj.AddChild(child2);
            return obj;
        }

        private static EdiObject CreateNoFieldsNoChildren()
        {
            return new EdiObject("Kontakt", new XElement("root"), null);
        }

        private static EdiObject CreateMultiValueFields()
        {
            var obj = new EdiObject("Multi", new XElement("root"), "m1");
            obj.Fields["A"] = new List<string> { "a1", "a2" };
            obj.Fields["B"] = new List<string> { "b1", "b2" };
            return obj;
        }

        private static EdiObject CreateMultiValueWithChildren()
        {
            var obj = new EdiObject("Multi", new XElement("root"), "m1");
            obj.Fields["A"] = new List<string> { "a1", "a2" };
            obj.Fields["B"] = new List<string> { "b1", "b2" };
            var child = new EdiObject("Child", new XElement("c"), "c1");
            child.Fields["X"] = new List<string> { "x1" };
            obj.AddChild(child);
            return obj;
        }
    }
}
