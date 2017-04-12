using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
    public class SpannungsebenenRepository : GenericRepository<BaseKeyValue>
    {
        
        public override List<BaseKeyValue> GetValueList(bool allowNullValue = true)
        {
            if (_initialized)
            {
                if (allowNullValue)
                {
                    List<BaseKeyValue> returnList = new List<BaseKeyValue>();
                    returnList.Add( new BaseKeyValue() { Key = null, Value = "Keine Angabe" });
                    returnList.AddRange(_internalList);
                    return returnList;
                }

                return _internalList;
            }

            List<BaseKeyValue> liste = new List<BaseKeyValue>();
            if (allowNullValue)
                liste.Add(new BaseKeyValue() { Key = null, Value = "Keine Angabe" });
            liste.AddRange(new List<BaseKeyValue>(){
                    new BaseKeyValue(){ Key = "E06",Value="Niederspannung"},
                    new BaseKeyValue(){ Key = "E05",Value="Mittelspannung"},
                    new BaseKeyValue(){ Key = "E04",Value="Hochspannung"},
                    new BaseKeyValue(){ Key = "E03",Value="Höchstspannung"},
                    new BaseKeyValue(){ Key = "E07",Value="HöS/HS Umspannung"},
                    new BaseKeyValue(){ Key = "E08",Value="HS/MS Umspannung"},
                    new BaseKeyValue(){ Key = "E08",Value="MS/NS Umspannung"}
                    
                });
            return liste;
        }
    }
    
}
