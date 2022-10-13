using BuchDatenbank;
using Microsoft.AspNetCore.Mvc;

namespace BuchVerwaltungMVC.Models
{
    public class BuchListeModel
    {
        // Erstellen von 2 Listen, um die Datensätze aus den Datenbanktabellen zu speichern
        public List<BuchDTO> AktuelleBuecherListe { get; set; } = new();
        public List<BuchDTO> ArchivierteBuecherListe { get; set; } = new();

        public BuchListeModel(IEnumerable<BuchDTO> aktuelleBuecherListe, IEnumerable<BuchDTO> archivierteBuecherListe)
        {
            // Befüllen der Liste aktuelleBuecherListe
            foreach (var buchDTO in aktuelleBuecherListe)
            {
                AktuelleBuecherListe.Add(buchDTO);
            }
            // Befüllen der Liste archivierteBuecherListe
            foreach (var buchDTO in archivierteBuecherListe)
            {
                ArchivierteBuecherListe.Add(buchDTO);
            }
        }
    }
}
