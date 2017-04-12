using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
   
    public class ErzeugungsanlagenRepository : GenericRepository<BaseKeyValue>
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

            List<BaseKeyValue> liste =  new List<BaseKeyValue>();
            if(allowNullValue)
                liste.Add(new BaseKeyValue(){Key=null,Value="Keine Angabe"});
            liste.AddRange(new List<BaseKeyValue>(){
                new BaseKeyValue(){ Key = "Z33",Value="EEG-Anlage ohne DV-Pflicht"},    
                new BaseKeyValue(){ Key = "Z34",Value="KWKG-Anlage"},
                new BaseKeyValue(){ Key = "Z35",Value="sonstige Anlage"},
                new BaseKeyValue(){ Key = "Z37",Value="EEG-Anlage mit DV-Pflicht"},



                });
            return liste;
        }
    }
    
}
