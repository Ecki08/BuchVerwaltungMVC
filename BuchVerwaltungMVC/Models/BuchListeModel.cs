using BuchDatenbank;
using Buecher;
using Microsoft.AspNetCore.Mvc;

namespace BuchVerwaltungMVC.Models
{
    public class BuchListeModel
    {        
        public List<BuchDTO> AktuelleBuecherListe { get; set; } = new();
        public List<BuchDTO> ArchivierteBuecherListe { get; set; } = new();

        public BuchListeModel(IEnumerable<BuchDTO> aktuelleBuecherListe, IEnumerable<BuchDTO> archivierteBuecherListe)
        {
            foreach (var buchDTO in aktuelleBuecherListe)
            {
                AktuelleBuecherListe.Add(buchDTO);
            }
            foreach (var buchDTO in archivierteBuecherListe)
            {
                ArchivierteBuecherListe.Add(buchDTO);
            }
        }
    }
}
