using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class StatusNetznutzungRepository : GenericRepository<BaseKeyValue>
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
                new BaseKeyValue(){ Key = "E02",Value="Vertrag zwischen Lieferant und VNB"},
                new BaseKeyValue(){ Key = "E01",Value="Direkter Vertrag zwischen Kunde und VNB"},
                //Z03 zum 01.10.2013 gelöscht (Ä018)
                    
                });
            return liste;
        }
    }
}
