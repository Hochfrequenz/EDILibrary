using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
    public class Wechselanlass : BaseKeyValue
    {

    }
    public class WechselanlassRepository
    {
        public static Wechselanlass GetWechselanlassByKey(string mask, string key)
        {
            return (from Wechselanlass reason in GetWechselAnlass(mask) where reason.Key == key select reason).FirstOrDefault();
        }
        public static List<Wechselanlass> GetWechselAnlass(string mask)
        {

            return new List<Wechselanlass>()
                {
                    
                    new Wechselanlass(){ Key = "E17",Value="Fristüberschreitung"},
                    new Wechselanlass(){ Key = "Z66",Value="MSB-Scheitermeldung liegt vor"},
                    new Wechselanlass(){ Key = "Z74",Value="kein Zugang"},
                    new Wechselanlass(){ Key = "Z75",Value="Kommunikationsstörung"}
                };


        }
    }
}
