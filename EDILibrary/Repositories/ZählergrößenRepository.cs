using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
    public class ZählergrößenRepository : GenericRepository<BaseKeyValue>
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
                    new BaseKeyValue(){ Key = "G10",Value="Gaszähler G10"},
                    new BaseKeyValue(){ Key = "G100",Value="Gaszähler G100"},
                    new BaseKeyValue(){ Key = "G1000",Value="Gaszähler G1000"},
                    new BaseKeyValue(){ Key = "G16",Value="Gaszähler G16"},
                    new BaseKeyValue(){ Key = "G160",Value="Gaszähler G160"},
                    new BaseKeyValue(){ Key = "G1600",Value="Gaszähler G1600"},
                    new BaseKeyValue(){ Key = "G2.5",Value="Gaszähler G2.5"},
                    new BaseKeyValue(){ Key = "G25",Value="Gaszähler G25"},
                    new BaseKeyValue(){ Key = "G250",Value="Gaszähler G250"},
                    new BaseKeyValue(){ Key = "G2500",Value="Gaszähler G2500"},
                    new BaseKeyValue(){ Key = "G4",Value="Gaszähler G4"},
                    new BaseKeyValue(){ Key = "G40",Value="Gaszähler G40"},
                    new BaseKeyValue(){ Key = "G400",Value="Gaszähler G400"},
                    new BaseKeyValue(){ Key = "G4000",Value="Gaszähler G4000"},
                    new BaseKeyValue(){ Key = "G6",Value="Gaszähler G6"},
                    new BaseKeyValue(){ Key = "G65",Value="Gaszähler G65"},
                    new BaseKeyValue(){ Key = "G650",Value="Gaszähler G650"},
                    new BaseKeyValue(){ Key = "G6500",Value="Gaszähler G6500"},
                });
            return liste;
        }
    }

}
