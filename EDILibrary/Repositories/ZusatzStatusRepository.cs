using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class ZusatzStatusRepository : GenericRepository<BaseKeyValue>
    {
        public override List<BaseKeyValue> GetValueList(bool allowNullValue = true)
        {
            List<BaseKeyValue> liste = new List<BaseKeyValue>();
            if (allowNullValue)
                liste.Add(new BaseKeyValue() { Key = null, Value = "Kein Zusatz-Status" });
            liste.AddRange(new List<BaseKeyValue>(){
                    new BaseKeyValue(){ Key = "ZA2",Value="zu stornierender Wert"},
                    new BaseKeyValue(){ Key = "Z83",Value="Kundenselbstablesung"},
                });
            return liste;
        }
    }
}
