using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
    public class BefestigungsartRepository : GenericRepository<BaseKeyValue>
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
                new BaseKeyValue(){ Key = "DPA",Value="3-Dreipunktaufhängung", Sparte = "S"},
                new BaseKeyValue(){ Key = "BKE",Value="Stecktechnik", Sparte = "S"},
                    new BaseKeyValue(){ Key = "HUT",Value="Hutschiene", Sparte = "S"},
                    new BaseKeyValue(){ Key = "Z31",Value="Einstutzen-Zähler", Sparte = "G"},
                    new BaseKeyValue(){ Key = "Z32",Value="Zweistutzen-Zähler", Sparte = "G"},
                });
            return liste;
        }
    }

}
