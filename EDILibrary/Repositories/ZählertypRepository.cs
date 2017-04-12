using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EDILibrary.Repositories
{
    public class ZählertypRepository : GenericRepository<BaseKeyValue>
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
                    new BaseKeyValue(){ Key = "AHZ",Value="analoger Haushaltszähler (Drehstrom)", Sparte = "S"},
                    new BaseKeyValue(){ Key = "WSZ",Value="analoger Wechselstromzähler", Sparte = "S"},
                    new BaseKeyValue(){ Key = "LAZ",Value="Lastgangzähler", Sparte = "S"},
                    new BaseKeyValue(){ Key = "MAX",Value="Maximumzähler", Sparte = "S"},
                    new BaseKeyValue(){ Key = "DKZ",Value="Drehkolbenzähler", Sparte = "G"},
                    new BaseKeyValue(){ Key = "BGZ",Value="Balgengaszähler", Sparte = "G"},
                    new BaseKeyValue(){ Key = "TRZ",Value="Turbinengasradzähler", Sparte = "G"},
                    new BaseKeyValue(){ Key = "UGZ",Value="Ultraschallgaszähler", Sparte = "G"},
                    new BaseKeyValue(){ Key = "WGZ",Value="Wirbelgaszähler", Sparte = "G"},
                    new BaseKeyValue(){ Key = "MRG",Value="Messdatenregistrierung", Sparte = "G"},
                    new BaseKeyValue(){ Key = "EHZ",Value="elektronischer Haushaltszähler"},
                    new BaseKeyValue(){ Key = "IVA",Value="Individuelle Abstimmung"},
                    new BaseKeyValue(){ Key = "MME",Value="moderne Messeinrichtung nach MsbG"}
                });
            return liste;
        }
    }

}
