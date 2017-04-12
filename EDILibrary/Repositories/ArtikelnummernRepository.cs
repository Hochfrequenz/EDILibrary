using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class ArtikelnummernRepository : GenericRepository<BaseKeyValue>
    {
        public override List<BaseKeyValue> GetValueList(bool allowNullValue = true)
        {
            if (_initialized)
            {
                if (allowNullValue)
                {
                    List<BaseKeyValue> returnList = new List<BaseKeyValue>();
                    returnList.Add(new BaseKeyValue() { Key = null, Value = "unbekannt" });
                    returnList.AddRange(_internalList);
                    return returnList;
                }

                return _internalList;
            }
            List<BaseKeyValue> liste = new List<BaseKeyValue>();
            if (allowNullValue)
                liste.Add(new BaseKeyValue() { Key = null, Value = "unbekannt" });
            liste.AddRange(new List<BaseKeyValue>(){
                new BaseKeyValue(){ Key = "9990001000053",Value="Leistung"},
                    new BaseKeyValue(){ Key = "9990001000079",Value="Leistung pauschal"},
                    new BaseKeyValue(){ Key = "9990001000087",Value="Grundpreis"},
                    new BaseKeyValue(){ Key = "9990001000160",Value="Reservenetzkapazität"},
                    new BaseKeyValue(){ Key = "9990001000178",Value="Reserveleistung"},
                    new BaseKeyValue(){ Key = "9990001000186",Value="zusätzliche Ablesung"},
                    new BaseKeyValue(){ Key = "9990001000219",Value="Prüfgebühren"},
                    new BaseKeyValue(){ Key = "9990001000269",Value="Wirkarbeit"},
                    new BaseKeyValue(){ Key = "9990001000285",Value="singul. Betriebsmittel"},
                    new BaseKeyValue(){ Key = "9990001000334",Value="Abgabe KWKG"},
                    new BaseKeyValue(){ Key = "9990001000376",Value="Abschlag"},
                    new BaseKeyValue(){ Key = "9990001000417",Value="Konzessionsabgabe"},
                    new BaseKeyValue(){ Key = "9990001000433",Value="Entgelt für Fernauslese"},
                    new BaseKeyValue(){ Key = "9990001000475",Value="Untermessung"},
                    new BaseKeyValue(){ Key = "9990001000508",Value="Blindmehrarbeit"},
                    new BaseKeyValue(){ Key = "9990001000532",Value="Entgelt für Abrechnung"},
                    new BaseKeyValue(){ Key = "9990001000615",Value="Entgelt für Messung und Ablesung"},
                    new BaseKeyValue(){ Key = "9990001000623",Value="Entgelt für Einb., Betr. Wart."},
                    new BaseKeyValue(){ Key = "9990001000681",Value="§19 Strom NEV"},
                    new BaseKeyValue(){ Key = "9990001000706",Value="Offshore-Haftungsumlage"},
                    new BaseKeyValue(){ Key = "9990001000730",Value="Umlage abschaltb. Lasten"},
                    new BaseKeyValue(){ Key = "9990001000699",Value="Befestigungseinrichtung z.B. Zählertafel"},
                    new BaseKeyValue(){ Key = "9990001000615",Value="Entgelt für Messung und Ablesung"}
                    
                    
                });
            return liste;
        }
    }
}
