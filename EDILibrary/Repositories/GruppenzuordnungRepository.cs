using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class GruppenzuordnungRepository : GenericRepository<BaseKeyValue>
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
                new BaseKeyValue(){ Key = "Z15",Value="Haushaltskunde gem. EnWG"},
                new BaseKeyValue(){ Key = "Z18",Value="kein Haushaltskunde"},
                    new BaseKeyValue(){ Key = "GABi-RLMNEV",Value="Normierungsersatzverfahren - Exit"},
                    new BaseKeyValue(){ Key = "GABi-RLMmT",Value="RLM-Kunde in Tagesregime"},
                    new BaseKeyValue(){ Key = "GABi-RLMoT",Value="RLM-Kunde im Stundenregime"},
                    new BaseKeyValue(){ Key = "ENTRY_H2",Value="Entry Wasserstoff physisch"}
                    
                });
            return liste;
        }
    }
}
