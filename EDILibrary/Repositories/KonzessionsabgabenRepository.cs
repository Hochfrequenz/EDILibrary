using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class KonzessionsabgabenRepository : GenericRepository<BaseKeyValue>
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
                new BaseKeyValue(){ Key = "TA",Value="Tarifkunden, für Strom § 2. (2) 1b HT bzw. ET (hohe KA) und für Gas § 2 (2) 2b"},
                new BaseKeyValue(){ Key = "KAS",Value="für alle konzessionsvertraglichen Sonderregelungen, die nicht in die Systematik der KAV eingegliedert sind"},
                new BaseKeyValue(){ Key = "SA",Value="Sondervertragskunden < 1 kV nach § 2 (7) und > 1 kV, Preis nach § 2 (3)"},
                    new BaseKeyValue(){ Key = "SAS",Value="Kennzeichnung, dass ein abweichender Preis für Sondervertragskunden vorliegt"},
                    
                    new BaseKeyValue(){ Key = "TAS",Value="Kennzeichnung, dass ein abweichender Preis für Tarifkunden vorliegt"},
                    new BaseKeyValue(){ Key = "TK",Value="für Gas nach KAV § 2 (2) 2a bei ausschließlicher Nutzung zum Kochen und Warmwassererzeugung"},
                    new BaseKeyValue(){ Key = "TKS",Value="Kennzeichnung, wenn nach KAV § 2 (2) 2a ein anderen Preis zu verwenden ist"},
                    new BaseKeyValue(){ Key = "TS",Value="für Strom mit Schwachlast § 2. (2) 1a NT (niedrige KA, 0,61 ct/kWh)"},
                    new BaseKeyValue(){ Key = "TSS",Value="Kennzeichnung, dass ein abweichender Preis für Schwachlast angewendet wird"},
                    
                });
            return liste;
        }
    }
}
