using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class EmpfaengerVerguetungRepository : GenericRepository<BaseKeyValue>
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
                new BaseKeyValue(){ Key = "E10",Value="Lieferant"},
                new BaseKeyValue(){ Key = "E09",Value="Kunde"},
                });
            return liste;
        }
    }
}
