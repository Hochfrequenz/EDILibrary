using System.Collections.Generic;
using System.Text.Json;
using System.Xml.Linq;
using EDILibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.IsTrue(root.TryGetProperty("Dokument", out _));
        }

        [TestMethod]
        public void SerializeToJSON_WithTabInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Text"] = new List<string> { "Spalte1\tSpalte2" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.IsTrue(root.TryGetProperty("Dokument", out _));
        }

        [TestMethod]
        public void SerializeToJSON_WithCarriageReturnInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Text"] = new List<string> { "Zeile 1\rZeile 2" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.IsTrue(root.TryGetProperty("Dokument", out _));
        }

        [TestMethod]
        public void SerializeToJSON_WithBackslashInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Path"] = new List<string> { @"C:\Users\test" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.IsTrue(root.TryGetProperty("Dokument", out _));
        }

        [TestMethod]
        public void SerializeToJSON_WithControlCharInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Data"] = new List<string> { "text\u000Bmore" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.IsTrue(root.TryGetProperty("Dokument", out _));
        }

        [TestMethod]
        public void SerializeToJSON_WithQuoteInField_ProducesValidJson()
        {
            var obj = new EdiObject("Test", new XElement("root"), "test-key");
            obj.Fields["Name"] = new List<string> { "Max \"Mustermann\"" };

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            Assert.IsTrue(root.TryGetProperty("Dokument", out _));
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
            Assert.IsTrue(root.TryGetProperty("Dokument", out _));
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
            Assert.AreEqual("abc", elem.GetProperty("Key").GetString());
            Assert.AreEqual("Wert", elem.GetProperty("Name").GetString());
            Assert.AreEqual("42", elem.GetProperty("Zaehler").GetString());
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
            Assert.AreEqual(2, segments.GetArrayLength());
            Assert.AreEqual("A", segments[0].GetProperty("Code").GetString());
            Assert.AreEqual("B", segments[1].GetProperty("Code").GetString());
        }

        [TestMethod]
        public void SerializeToJSON_NoFieldsNoChildren_ProducesEmptyKey()
        {
            var obj = new EdiObject("Kontakt", new XElement("root"), null);

            var json = obj.SerializeToJSON();

            using var doc = JsonDocument.Parse(json);
            var elem = doc.RootElement.GetProperty("Dokument")[0];
            Assert.AreEqual("", elem.GetProperty("Key").GetString());
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
            Assert.AreEqual(2, arr.GetArrayLength());
            Assert.AreEqual("m1", arr[0].GetProperty("Key").GetString());
            Assert.AreEqual("a1", arr[0].GetProperty("A").GetString());
            Assert.AreEqual("b1", arr[0].GetProperty("B").GetString());
            Assert.AreEqual("a2", arr[1].GetProperty("A").GetString());
            Assert.AreEqual("b2", arr[1].GetProperty("B").GetString());
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
            Assert.AreEqual(2, arr.GetArrayLength());
            Assert.AreEqual("a2", arr[1].GetProperty("A").GetString());
            Assert.AreEqual("x1", arr[1].GetProperty("Child")[0].GetProperty("X").GetString());
        }
    }
}
