using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
    public class Wechselkategorie : BaseKeyValue
    {

    }
    public class WechselkategorieRepository 
    {
        public static Wechselkategorie GetWechselkategorieByKey(string mask, string key)
        {
            return (from Wechselkategorie reason in GetWechselKategorien(mask) where reason.Key == key select reason).FirstOrDefault();
        }
        public static List<Wechselkategorie> GetWechselKategorien(string mask)
        {

            return new List<Wechselkategorie>()
                {
                    new Wechselkategorie(){ Key = "Z09",Value="MSB-Wechsel"},
                    new Wechselkategorie(){ Key = "Z10",Value="MSB inkl. MDL-Wechsel"},
                    new Wechselkategorie(){ Key = "Z11",Value="MDL-Wechsel"},
                    new Wechselkategorie(){ Key = "Z12",Value="Ab-/Auslesung"}
                };


        }
    }
    
}
