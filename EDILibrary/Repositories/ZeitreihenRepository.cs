using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class ZeitreihenRepository : GenericRepository<BaseKeyValue>
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
                new BaseKeyValue(){ Key = "DZR",Value="Deltazeitreihe gem. Ziff. 1.2.5, Anlage 1"},
                    new BaseKeyValue(){ Key = "EGS",Value="Einspeisegangsumme"},
                    new BaseKeyValue(){ Key = "FPE",Value="Fahrplanentnahmesumme"},
                    new BaseKeyValue(){ Key = "FPI",Value="Fahrplaneinspeisesumme"},
                    new BaseKeyValue(){ Key = "LGS",Value="Lastgangsumme"},
                    new BaseKeyValue(){ Key = "NZR",Value="Netzzeitreihe"},
                    new BaseKeyValue(){ Key = "SES",Value="Standardeinspeiseprofilsumme"},
                    new BaseKeyValue(){ Key = "SLS",Value="Standardlastprofilsumme"},
                    new BaseKeyValue(){ Key = "TES",Value="Tagesparameterabhängige Einspeiseprofilsumme"},
                    new BaseKeyValue(){ Key = "TLS",Value="Tagesparameterabhängige Lastprofilsumme"},
                    new BaseKeyValue(){ Key = "VZR",Value="Verlustzeitreihe"},
                    
                });
            return liste;
        }
    }
}
