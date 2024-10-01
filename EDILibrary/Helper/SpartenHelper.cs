using System.Collections.Generic;
using System.Threading.Tasks;
using EDILibrary.Interfaces;

namespace EDILibrary.Helper
{
    /// <summary>
    /// Lists of MP-Ids per Sparte
    /// </summary>
    public class Sparten
    {
        /// <summary>
        /// All known GS1 providers of Strom
        /// </summary>
        public ISet<string> STROM { get; } =
            new HashSet<string>
            {
                // Strom provider IDs
                "4033872000041",
                "4038777000004",
                "4041408000007",
                "4041409000006",
                "4042322000005",
                "4042322000005",
                "4042805002007",
                "4043581000027",
                "4045399000060",
                "4048454000005",
                "4260016040025",
                "4260016040032",
                "4260154980047",
                "4260154980115",
                "4260400590006",
                "4399901903883",
                "4399902034364",
                "4399902114356",
                "4399902196468",
                "4399902232050",
            };

        /// <summary>
        /// All known GS1 providers of Gas
        /// </summary>
        public ISet<string> GAS { get; } =
            new HashSet<string>
            {
                // Gas provider IDs
                "4041408700013",
                "4042322100002",
                "4042322100002",
                "4042805000607",
                "4043581000041",
                "4048454000012",
                "4260016042005",
                "4043581000034",
            };

        /// <summary>
        /// All known marktpartnerIds of Water
        /// </summary>
        public ISet<string> WASSER { get; set; } = new HashSet<string>();
    }

    public enum Sparte
    {
        STROM,
        GAS,
        WASSER,
        ABWASSER,
    }

    /// <summary>
    /// an extension class to retrieve the division from a market partner 13digit id (basically guessing)
    /// </summary>
    public class SpartenHelper : IDivisionResolver
    {
        private static readonly Sparten sparten = new();

        /// <summary>
        /// Tries to determine the division from the sender and receiver's code numbers.
        /// </summary>
        public Sparte GetSparte(string? absenderCode, string? empfaengerCode)
        {
            if (absenderCode is null)
            {
                return Sparte.STROM;
            }

            if (absenderCode.StartsWith("98"))
            {
                return Sparte.GAS;
            }

            if (absenderCode[0] != '9' && absenderCode[0] != '4')
            {
                return Sparte.WASSER;
            }

            if (absenderCode.StartsWith("99"))
            {
                //one is STROM, so the other can't be anything else
            }
            else if (empfaengerCode is not null)
            {
                if (empfaengerCode.StartsWith("98"))
                {
                    return Sparte.GAS;
                }

                if (empfaengerCode.StartsWith("99"))
                {
                    //one is STROM, so the other can't be anything else
                }
                else
                {
                    //both are GS1
                    if (sparten.GAS.Contains(absenderCode))
                    {
                        return Sparte.GAS;
                    }

                    if (sparten.GAS.Contains(empfaengerCode))
                    {
                        return Sparte.GAS;
                    }
                }
            }

            return Sparte.STROM;
        }
    }
}
