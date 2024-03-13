using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EDILibrary.Helper;
using EDILibrary.Interfaces;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EDILibraryTests
{
    [TestClass]
    public class TestSpartenFinder
    {
        [TestMethod]
        [DataRow("9900000000", "9900000000", Sparte.STROM)]
        [DataRow(null, "9900000000", Sparte.STROM)]
        [DataRow(null, null, Sparte.STROM)]
        [DataRow("9800000000", "9800000000", Sparte.GAS)]
        [DataRow("9900000000", "9800000000", Sparte.STROM)]
        [DataRow("4033872000041", "9900000000", Sparte.STROM)]
        [DataRow("4033872000041", "4038777000004", Sparte.STROM)]
        [DataRow("4041408700013", "4038777000004", Sparte.GAS)]
        [DataRow("4260016042005", "4041408700013", Sparte.GAS)]
        [DataRow("5260016042005", "4041408700013", Sparte.WASSER)]
        public async Task TestSparte(
            string sender,
            string receiver,
            Sparte expectedSparte
        )
        {
            Sparten s = new();
            s.STROM.Add("4033872000041");
            s.GAS.Add("4041408700013");
            s.GAS.Add("4260016042005");
            IDivisionResolver spartenHelper = new SpartenHelper();
            var sparte = spartenHelper.GetSparte(sender, receiver);
            sparte.Should().Be(expectedSparte);
        }
    }
}
