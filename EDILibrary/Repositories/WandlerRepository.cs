using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class WandlerRepository : GenericRepository<BaseKeyValue>
    {
        
        public override List<BaseKeyValue> GetValueList(bool allowNullValue = true)
        {
            if (_initialized)
            {
                if (allowNullValue)
                {
                    List<BaseKeyValue> returnList = new List<BaseKeyValue>();
                    returnList.Add(new BaseKeyValue() { Key = null, Value = "Kein/unbekannter Wandler" });
                    returnList.AddRange(_internalList);
                    return returnList;
                }

                return _internalList;
            }
            List<BaseKeyValue> liste = new List<BaseKeyValue>();
            if (allowNullValue)
                liste.Add(new BaseKeyValue() { Key = null, Value = "Kein/unbekannter Wandler" });
            liste.AddRange(new List<BaseKeyValue>(){
                    new BaseKeyValue(){ Key = "MIW",Value="Messwandlersatz Strom", Sparte="S"},
                    new BaseKeyValue(){ Key = "MPW",Value="Kombimesswandlersatz (Strom und Spannung)", Sparte="S"},
                    new BaseKeyValue(){ Key = "MBW",Value="Blockstromwandler", Sparte="S"},
                    new BaseKeyValue(){ Key = "MUW",Value="Messwandlersatz Spannung", Sparte="S"},
                    new BaseKeyValue(){ Key = "DMU",Value="Dichtemengenumwerter", Sparte="G"},
                    // MSW entfällt zum 01.04.13
                    new BaseKeyValue(){ Key = "MSW",Value="Messwandler", Sparte="S",Bis="04/13"},
                    new BaseKeyValue(){ Key = "TMU",Value="Temperaturmengenumwerter", Sparte="G"},
                    new BaseKeyValue(){ Key = "ZMU",Value="Zustandsmengenumwerter", Sparte="G"}
                    
                });
            return liste;
        }
    }
}
