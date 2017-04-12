
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class ZählverfahrenRepository : GenericRepository<BaseKeyValue>
    {
        public override List<BaseKeyValue> GetValueList(bool allowNullValue = true)
        {
            if (_initialized)
            {
                if (allowNullValue)
                {
                    List<BaseKeyValue> returnList = new List<BaseKeyValue>();
                    returnList.Add(new BaseKeyValue() { Key = null, Value = "Keine Angabe" });
                    returnList.AddRange(_internalList);
                    return returnList;
                }

                return _internalList;
            }
            List<BaseKeyValue> liste = new List<BaseKeyValue>();
            if (allowNullValue)
                liste.Add(new BaseKeyValue() { Key = null, Value = "Keine Angabe" });
            liste.AddRange(new List<BaseKeyValue>(){
                new BaseKeyValue(){ Key = "E01",Value="Registrierende Leistungsmessung (RLM)"},
                    new BaseKeyValue(){ Key = "E02",Value="Nicht registrierende Leistungsmessung"},
                    new BaseKeyValue(){ Key = "E14",Value="TLP mit separater Messung"},
                    new BaseKeyValue(){ Key = "E24",Value="TLP mit gemeinsamer Messung"},
                    new BaseKeyValue(){ Key = "Z29",Value="Pauschalanlage"},
                    
                });
            return liste;
        }
    }
}
