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
    }
}
