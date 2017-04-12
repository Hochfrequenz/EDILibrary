using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
   
    public class MarktgebieteRepository : GenericRepository<BaseKeyValue>
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
                new BaseKeyValue(){ Key = "37Y701125MH0000I",Value="NCG"},    
                new BaseKeyValue(){ Key = "37Y701133MH0000P",Value="GASPOOL"},
              //  new BaseKeyValue(){ Key = "37Y701130ML0000P",Value="L-Gas 1"},                  
                    
                    
                });
            return liste;
        }
    }
    
}
