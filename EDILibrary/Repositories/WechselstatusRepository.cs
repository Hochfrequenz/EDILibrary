using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
    public class Wechselstatus : BaseKeyValue
    {

    }
    public class WechselstatusRepository 
    {
        public static Wechselstatus GetWechselstatusByKey(string mask, string key)
        {
            return (from Wechselstatus reason in GetWechselStatus(mask) where reason.Key == key select reason).FirstOrDefault();
        }
        public static List<Wechselstatus> GetWechselStatus(string mask)
        {

            return new List<Wechselstatus>()
                {
                    new Wechselstatus(){ Key = "Z13",Value="gescheitert"},
                    new Wechselstatus(){ Key = "Z14",Value="erfolgreich"},
                    new Wechselstatus(){ Key = "209",Value="Lieferung geplant"},
                    
                };


        }
    }
}
