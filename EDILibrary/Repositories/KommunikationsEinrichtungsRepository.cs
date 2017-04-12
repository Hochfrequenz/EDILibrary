using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class KommunikationsEinrichtungsRepository : GenericRepository<BaseKeyValue>
    {
        public override List<BaseKeyValue> GetValueList(bool allowNullValue = true)
        {
            if (_initialized)
            {
                if (allowNullValue)
                {
                    List<BaseKeyValue> returnList = new List<BaseKeyValue>();
                    returnList.Add(new BaseKeyValue() { Key = null, Value = "Keine/unbekannte Komm.Einrichtung" });
                    returnList.AddRange(_internalList);
                    return returnList;
                }

                return _internalList;
            }

            List<BaseKeyValue> liste = new List<BaseKeyValue>();
            if (allowNullValue)
                liste.Add(new BaseKeyValue() { Key = null, Value = "Keine/unbekannte Komm.Einrichtung" });
            liste.AddRange(new List<BaseKeyValue>(){
                    new BaseKeyValue(){ Key = "GSM",Value="GSM/GPRS/UMTS/LTE-Kom.-Einr."},
                    new BaseKeyValue(){ Key = "ETH",Value="Ethernet-Kom.-Einricht. LAN/WLAN"},
                    new BaseKeyValue(){ Key = "PLC",Value="PLC-Kom.-Einrichtung"},
                    new BaseKeyValue(){ Key = "PST",Value="Festnetz-Kom.-Einricht. TAE"}
                    
                });
            return liste;
        }
    }
}
