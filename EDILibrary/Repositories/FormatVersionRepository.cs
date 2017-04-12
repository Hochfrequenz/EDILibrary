using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public static class stringExtensions
    {
        public static bool DotDelimitedIsSameOrAfter(this string a, string b)
        {
            if (a == null || b == null)
                return false;
            return a.IsSameOrAfter(b, new FormatVersionComparer());
        }
        public static bool DashDelimitedIsSameOrAfter(this string a, string b)
        {
            if (a == null || b == null)
                return false;
            return a.IsSameOrAfter(b, new PackageVersionComparer());
        }
        public static bool DashDelimitedBetween(this string a, string b, string c)
        {
            if (a == null || b == null || c == null)
                return false;
            return a.IsBetween(b, c, new PackageVersionComparer());
        }
        public static bool IsBetween(this string a, string b, string c, StringComparer comp)
        {
            return comp.Compare(a, b) >= 0 && comp.Compare(a, c) <= 0;
        }
        public static bool IsSameOrAfter(this string a, string b, StringComparer comp)
        {
            return comp.Compare(a, b) <= 0;
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

    }
    public class PackageVersionComparer : StringComparer
    {
        public override int Compare(string x, string y)
        {
            var partsX = x.Split('/');
            var partsY = y.Split('/');
            for (int i = partsY.Length - 1; i >= 0; i--)
            {
                if (partsX.Length <= i)
                    return 1;
                var partComp = partsY[i].CompareTo(partsX[i]);
                if (partComp != 0)
                    return partComp;
            }
            return 0;
        }

        public override bool Equals(string x, string y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }

    }
    public class FormatVersionComparer : StringComparer
    {
        public override int Compare(string x, string y)
        {
            var partsX = x.Split('.');
            var partsY = y.Split('.');
            for (int i = 0; i < partsY.Length; i++)
            {
                if (partsX.Length <= i)
                    return 1;
                var partComp = partsY[i].CompareTo(partsX[i]);
                if (partComp != 0)
                    return partComp;
            }
            return 0;
        }

        public override bool Equals(string x, string y)
        {
            return x.Equals(y);
        }

        public override int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }

    }
    public class FormatVersionRepository
    {
        public static List<string> GetFormatPackages()
        {
            return new List<string>() { "keine Angabe", "10/17", "10/16", "04/16", "10/15", "04/15", "10/14", "04/14", "10/13", "04/13", "10/12", "04/12" };
        }
        public static string getFormatPackage(string format, string version)
        {
            switch (format)
            {
                case "UTILMD":
                    {
                        switch (version)
                        {
                            case "5.1f":
                                return "10/16";
                            case "5.1e":
                                return "04/16";
                            case "5.1d":
                                return "10/15";
                            case "5.1c":
                                return "04/15";
                            case "5.1b":
                                return "10/14";
                            case "5.1a":
                                return "04/14";
                            case "5.1":
                                return "10/13";
                            case "5.0":
                                return "04/13";
                            case "4.5":
                                return "10/12";
                            case "4.4a":
                                return "04/12";
                            default:
                                return null;
                        }
                    }
                case "INVOIC":
                    {
                        switch (version)
                        {
                            case "2.6d":
                                return "10/16";
                            case "2.6c":
                                return "04/16";
                            case "2.6b":
                                return "10/15";
                            case "2.6a":
                                return "04/15";
                            case "2.6":
                                return "10/14";
                            case "2.5a":
                                return "04/14";
                            default:
                                return null;
                        }
                    }
                case "APERAK":
                    {
                        switch (version)
                        {
                            case "2.1a":
                                return "10/15";
                            case "2.1":
                                return "04/14";
                            case "2.0g":
                                return "10/12";
                            case "2.0f":
                                return "04/12";
                            default:
                                return null;
                        }
                    }
                case "CONTRL":
                    {
                        switch (version)
                        {
                            case "2.0":
                                return "10/15";
                            default:
                                return null;
                        }
                    }
                case "IFTSTA":
                    {
                        switch (version)
                        {
                            case "1.2a":
                                return "10/15";
                            case "1.2":
                                return "10/14";
                            case "1.1b":
                                return "04/14";
                            case "1.1a":
                                return "04/13";
                            default:
                                return null;
                        }
                    }
                case "INSRPT":
                    {
                        switch (version)
                        {
                            case "1.0b":
                                return "10/15";
                            case "1.0a":
                                return "10/14";
                            case "1.0":
                                return "04/14";
                            default:
                                return null;
                        }
                    }
                case "REMADV":
                    {
                        switch (version)
                        {
                            case "2.7b":
                                return "04/16";
                            case "2.7a":
                                return "10/15";
                            case "2.7":
                                return "04/15";
                            case "2.6":
                                return "10/14";
                            case "2.5a":
                                return "04/14";
                            case "2.5":
                                return "10/13";
                            default:
                                return null;
                        }
                    }
                case "MSCONS":
                    {
                        switch (version)
                        {
                            case "2.2g":
                                return "10/16";
                            case "2.2f":
                                return "04/16";
                            case "2.2e":
                                return "10/15";
                            case "2.2d":
                                return "04/15";
                            case "2.2c":
                                return "10/14";
                            case "2.2b":
                                return "04/14";
                            case "2.2a":
                                return "10/13";
                            case "2.2":
                                return "04/13";
                            default:
                                return null;
                        }

                    }
                case "QUOTES":
                    {
                        switch (version)
                        {
                            case "1.0f":
                                return "10/16";
                            case "1.0e":
                                return "04/16";
                            case "1.0d":
                                return "10/15";
                            case "1.0c":
                                return "10/14";
                            case "1.0b":
                                return "04/14";
                            case "1.0a":
                                return "10/13";
                            default:
                                return null;
                        }
                    }
                case "ORDRSP":
                    {
                        switch (version)
                        {
                            case "1.1f":
                                return "10/16";
                            case "1.1e":
                                return "04/16";
                            case "1.1d":
                                return "10/15";
                            case "1.1c":
                                return "04/15";
                            case "1.1b":
                                return "10/14";
                            case "1.1a":
                                return "04/14";
                            case "1.1":
                                return "04/13";
                            default:
                                return null;
                        }
                    }
                case "ORDERS":
                    {
                        switch (version)
                        {
                            case "1.1h":
                                return "10/16";
                            case "1.1g":
                                return "04/16";
                            case "1.1f":
                                return "10/15";
                            case "1.1e":
                                return "04/15";
                            case "1.1d":
                                return "10/14";
                            case "1.1c":
                                return "04/14";
                            case "1.1b":
                                return "10/13";
                            case "1.1a":
                                return "04/13";
                            default:
                                return null;
                        }

                    }
                case "REQOTE":
                    {
                        switch (version)
                        {
                            case "1.1a":
                                return "10/15";
                            case "1.1":
                                return "10/14";
                            case "1.0":
                                return "04/14";
                            default:
                                return null;
                        }
                    }

                case "PRICAT":
                    {
                        switch (version)
                        {
                            case "1.0a":
                                return "04/16";
                            default:
                                return null;
                        }
                    }
                default:
                    {
                        return null;
                    }
            }
        }
        public static string GetFormatVersion(string formatPackage, string format)
        {
            switch (formatPackage)
            {
                case "10/16":
                    {
                        switch (format)
                        {
                            case "UTILMD":
                                {
                                    return "5.1f";
                                }
                            case "INVOIC":
                                {
                                    return "2.6d";
                                }
                            case "APERAK":
                                {
                                    return "2.1a";
                                }
                            case "CONTRL":
                                {
                                    return "2.0";
                                }
                            case "IFTSTA":
                                {
                                    return "1.2a";
                                }
                            case "INSRPT":
                                {
                                    return "1.0b";
                                }
                            case "REMADV":
                                {
                                    return "2.7b";
                                }
                            case "MSCONS":
                                {
                                    return "2.2g";
                                }
                            case "QUOTES":
                                {
                                    return "1.0f";
                                }
                            case "ORDRSP":
                                {
                                    return "1.1f";
                                }
                            case "ORDERS":
                                {
                                    return "1.1h";
                                }
                            case "REQOTE":
                                {
                                    return "1.1a";
                                }
                            case "PRICAT":
                                {
                                    return "1.0a";
                                }
                        }
                    }
                    break;
                case "04/16":
                    {
                        switch (format)
                        {
                            case "UTILMD":
                                {
                                    return "5.1e";
                                }
                            case "INVOIC":
                                {
                                    return "2.6c";
                                }
                            case "APERAK":
                                {
                                    return "2.1a";
                                }
                            case "CONTRL":
                                {
                                    return "2.0";
                                }
                            case "IFTSTA":
                                {
                                    return "1.2a";
                                }
                            case "INSRPT":
                                {
                                    return "1.0b";
                                }
                            case "REMADV":
                                {
                                    return "2.7b";
                                }
                            case "MSCONS":
                                {
                                    return "2.2f";
                                }
                            case "QUOTES":
                                {
                                    return "1.0e";
                                }
                            case "ORDRSP":
                                {
                                    return "1.1e";
                                }
                            case "ORDERS":
                                {
                                    return "1.1g";
                                }
                            case "REQOTE":
                                {
                                    return "1.1a";
                                }
                            case "PRICAT":
                                {
                                    return "1.0a";
                                }
                        }
                    }
                    break;
                case "10/15":
                    {
                        switch (format)
                        {
                            case "UTILMD":
                                {
                                    return "5.1d";
                                }
                            case "INVOIC":
                                {
                                    return "2.6b";
                                }
                            case "APERAK":
                                {
                                    return "2.1a";
                                }
                            case "CONTRL":
                                {
                                    return "2.0";
                                }
                            case "IFTSTA":
                                {
                                    return "1.2a";
                                }
                            case "INSRPT":
                                {
                                    return "1.0b";
                                }
                            case "REMADV":
                                {
                                    return "2.7a";
                                }
                            case "MSCONS":
                                {
                                    return "2.2e";
                                }
                            case "QUOTES":
                                {
                                    return "1.0d";
                                }
                            case "ORDRSP":
                                {
                                    return "1.1d";
                                }
                            case "ORDERS":
                                {
                                    return "1.1f";
                                }
                            case "REQOTE":
                                {
                                    return "1.1a";
                                }
                        }
                    }
                    break;
                case "04/15":
                    {
                        switch (format)
                        {
                            case "UTILMD":
                                {
                                    return "5.1c";
                                }
                            case "INVOIC":
                                {
                                    return "2.6a";
                                }
                            case "APERAK":
                                {
                                    return "2.1a";
                                }
                            case "CONTRL":
                                {
                                    return "2.0";
                                }
                            case "IFTSTA":
                                {
                                    return "1.2a";
                                }
                            case "INSRPT":
                                {
                                    return "1.0b";
                                }
                            case "REMADV":
                                {
                                    return "2.7";
                                }
                            case "MSCONS":
                                {
                                    return "2.2d";
                                }
                            case "QUOTES":
                                {
                                    return "1.0d";
                                }
                            case "ORDRSP":
                                {
                                    return "1.1c";
                                }
                            case "ORDERS":
                                {
                                    return "1.1e";
                                }
                            case "REQOTE":
                                {
                                    return "1.1a";
                                }
                        }
                    }
                    break;
                case "10/14":
                    {
                        switch (format)
                        {
                            case "UTILMD":
                                {
                                    return "5.1b";
                                }
                            case "INVOIC":
                                {
                                    return "2.6";
                                }
                            case "APERAK":
                                {
                                    return "2.1a";
                                }
                            case "CONTRL":
                                {
                                    return "2.0";
                                }
                            case "IFTSTA":
                                {
                                    return "1.2";
                                }
                            case "INSRPT":
                                {
                                    return "1.0a";
                                }
                            case "REMADV":
                                {
                                    return "2.6";
                                }
                            case "MSCONS":
                                {
                                    return "2.2c";
                                }
                            case "QUOTES":
                                {
                                    return "1.0c";
                                }
                            case "ORDRSP":
                                {
                                    return "1.1b";
                                }
                            case "ORDERS":
                                {
                                    return "1.1d";
                                }
                            case "REQOTE":
                                {
                                    return "1.1";
                                }
                        }
                    }
                    break;
                case "04/14":
                    {
                        switch (format)
                        {
                            case "UTILMD":
                                {
                                    return "5.1a";
                                }
                            case "INVOIC":
                                {
                                    return "2.5a";
                                }
                            case "APERAK":
                                {
                                    return "2.1";
                                }
                            case "IFTSTA":
                                {
                                    return "1.1b";
                                }
                            case "INSRPT":
                                {
                                    return "1.0";
                                }
                            case "REMADV":
                                {
                                    return "2.5a";
                                }
                            case "MSCONS":
                                {
                                    return "2.2b";
                                }
                            case "QUOTES":
                                {
                                    return "1.0b";
                                }
                            case "ORDRSP":
                                {
                                    return "1.1a";
                                }
                            case "ORDERS":
                                {
                                    return "1.1c";
                                }
                            case "REQOTE":
                                {
                                    return "1.0";
                                }
                        }
                    }
                    break;
                case "10/13":
                    {
                        switch (format)
                        {
                            case "UTILMD":
                                {
                                    return "5.1";
                                }
                            case "INVOIC":
                                {
                                    return "2.5a";
                                }
                            case "APERAK":
                                {
                                    return "2.1";
                                }
                            case "IFTSTA":
                                {
                                    return "1.1b";
                                }
                            case "INSRPT":
                                {
                                    return "1.0";
                                }
                            case "REMADV":
                                {
                                    return "2.5";
                                }
                            case "MSCONS":
                                {
                                    return "2.2a";
                                }
                            case "QUOTES":
                                {
                                    return "1.0a";
                                }
                            case "ORDRSP":
                                {
                                    return "1.1a";
                                }
                            case "ORDERS":
                                {
                                    return "1.1b";
                                }
                            case "REQOTE":
                                {
                                    return "1.0";
                                }
                        }
                    }
                    break;
                case "04/13":
                    {
                        switch (format)
                        {
                            case "UTILMD":
                                {
                                    return "5.0";
                                }
                            case "APERAK":
                                {
                                    return "2.1";
                                }
                            case "IFTSTA":
                                {
                                    return "1.1a";
                                }
                            case "REMADV":
                                {
                                    return "2.5";
                                }
                            case "MSCONS":
                                {
                                    return "2.2";
                                }
                            case "QUOTES":
                                {
                                    return "1.0a";
                                }
                            case "ORDRSP":
                                {
                                    return "1.1";
                                }
                            case "ORDERS":
                                {
                                    return "1.1a";
                                }
                        }
                    }
                    break;

                case "10/12":
                    {
                        switch (format)
                        {
                            case "UTILMD":
                                {
                                    return "4.5";
                                }
                            case "APERAK":
                                {
                                    return "2.0g";
                                }
                            case "IFTSTA":
                                {
                                    return "1.1a";
                                }
                        }
                    }
                    break;
                case ("04/12"):
                    {
                        switch (format)
                        {
                            case "UTILMD":
                                {
                                    return "4.4a";
                                }
                            case "APERAK":
                                {
                                    return "2.0f";
                                }
                        }
                    }
                    break;
                default:
                    {
                        return null;
                    }

            }
            return null;
        }
    }
}
