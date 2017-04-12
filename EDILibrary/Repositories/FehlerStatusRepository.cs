using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class FehlerStatusRepository : GenericRepository<BaseKeyValue>
    {
        public override List<BaseKeyValue> GetValueList(bool allowNullValue = true)
        {
            List<BaseKeyValue> liste = new List<BaseKeyValue>();
            if (allowNullValue)
                liste.Add(new BaseKeyValue() { Key = null, Value = "Kein Fehler" });
            liste.AddRange(new List<BaseKeyValue>(){
                    new BaseKeyValue(){ Key = "Z01",Value="Qualifier nicht aus erlaubtem Wertebereich"},
                    new BaseKeyValue(){ Key = "Z02",Value="Format nicht eingehalten"},
                    new BaseKeyValue(){ Key = "Z03",Value="Erforderliche Angabe (in einem Datenelement) fehlt"},
                    new BaseKeyValue(){ Key = "Z05",Value="Empfänger-MP-ID und Empfänger stimmen nicht überein"},
                    new BaseKeyValue(){ Key = "Z06",Value="MP-ID bei Empfänger nicht bekannt"},
                    new BaseKeyValue(){ Key = "Z07",Value="Datenaustauschreferenz des Absenders bei Empfänger bereits bekannt"},
                    new BaseKeyValue(){ Key = "Z08",Value="Segment fehlt"},
                    new BaseKeyValue(){ Key = "Z09",Value="Zählpunktbezeichnung fehlt"}, 
                    new BaseKeyValue(){ Key = "Z10",Value="Zahlpunktbezeichnung unbekannt"},
                    new BaseKeyValue(){ Key = "Z11",Value="Mindestumfang im Initialprozess zur Identifizierung nicht eingehalten"},
                    new BaseKeyValue(){ Key = "Z12",Value="Mindestumfang im Initialprozess zur Identifizierung eingehalten, im IT-System nicht gefunden"},
                    new BaseKeyValue(){ Key = "Z13",Value="Mindestumfang im Initialprozess zur Identifizierung eingehalten, im IT-System nicht eindeutig"},
                    new BaseKeyValue(){ Key = "Z14",Value="Lieferstelle im IT-System nicht gefunden"},
                    new BaseKeyValue(){ Key = "Z15",Value="Lieferstelle im IT-System nicht eindeutig"},
                    new BaseKeyValue(){ Key = "Z16",Value="Lieferstelle nicht mehr im Netzgebiet"},
                    new BaseKeyValue(){ Key = "Z17",Value="Absender ist zum angegebenen Zeitintervall dem Zählpunkt nicht zugeordnet"},
                    new BaseKeyValue(){ Key = "Z18",Value="Empfänger ist zum angegebenen Zeitintervall dem Zählpunkt nicht zugeordnet"},
                    new BaseKeyValue(){ Key = "Z19",Value="Gerätenummer am Zählpunkt nicht bekannt"},
                    new BaseKeyValue(){ Key = "Z20",Value="OBIS-Code am Zählpunkt nicht bekannt"},
                    new BaseKeyValue(){ Key = "Z21",Value="Vorgangsinterne Referenzierung fehlerhaft"},
                    new BaseKeyValue(){ Key = "Z23",Value="Version der Summenzeitreihe aus MSCONS unbekannt"}


                });
            return liste;
        }
    }
}
