using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
   
    public class RegelzonenRepository : GenericRepository<BaseKeyValue>
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
                new BaseKeyValue(){ Key = "10YDE-ENBW-----N",Value="Transnet BW"},    
                new BaseKeyValue(){ Key = "10YDE-EON------1",Value="Tennet TSO GmbH"},
                new BaseKeyValue(){ Key = "10YDE-RWENET---I",Value="Amprion"},
                new BaseKeyValue(){ Key = "10YDE-VE-------2",Value="50Hertz Transmission"},
                    
                    
                    
                });
            return liste;
        }
    }
    
}
