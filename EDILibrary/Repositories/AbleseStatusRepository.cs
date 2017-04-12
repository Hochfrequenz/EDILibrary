using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class AbleseStatusRepository : GenericRepository<BaseKeyValue>
    {
        public override List<BaseKeyValue> GetValueList(bool allowNullValue = true)
        {
            List<BaseKeyValue> liste = new List<BaseKeyValue>();
            if (allowNullValue)
                liste.Add(new BaseKeyValue() { Key = null, Value = "Kein Status" });
            liste.AddRange(new List<BaseKeyValue>(){
                    new BaseKeyValue(){ Key = "220",Value="abgelesener, wahrer Wert"},
                    new BaseKeyValue(){ Key = "67",Value="Ersatzwert"},
                    new BaseKeyValue(){ Key = "201",Value="Vorschlagswert"},
                    new BaseKeyValue(){ Key = "20",Value="nicht verwendbarer Wert"},                  
                });
            return liste;
        }
    }
}
