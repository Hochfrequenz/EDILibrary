using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
   
    public class StatusPraemieRepository : GenericRepository<BaseKeyValue>
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
                new BaseKeyValue(){ Key = "Z26",Value="Anlage ist technisch nicht fernsteuerbar"},    
                new BaseKeyValue(){ Key = "Z27",Value="Anlage ist technisch fernsteuerbar"},
                new BaseKeyValue(){ Key = "Z28",Value="Anlage ist durch den Lieferanten fernsteuerbar"},



                });
            return liste;
        }
    }
    
}
