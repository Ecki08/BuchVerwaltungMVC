using BuchDatenbank;
using BuchVerwaltungMVC.Models;
using Microsoft.AspNetCore.Mvc;
using BuchVerwaltungMVC.Konfigurationsleser;
using System.Xml.Serialization;
using System;

namespace BuchVerwaltungMVC.Controllers
{
    public class BuchController : Controller
    {
        /*private readonly KonfigurationsLeser _konfigurationsLeser;

        public BuchController(KonfigurationsLeser konfigurationsleser)
        {
            this._konfigurationsLeser = konfigurationsleser;
        }

        public string GetConnectionString()
        {
            return _konfigurationsLeser.LiesDatenbankVerbindungZurMariaDB();
        }*/

        public string GetConnectionString()
        {
            return "Server=localhost;User ID=root;Password=password;Database=BuchDb;";
        }

        public IActionResult Index()
        {
            string connectionString = this.GetConnectionString();
            BuchListeModel model = LeseDatenAusDatenbankInModelEin(connectionString);

            return View(model);

        }

        public BuchListeModel LeseDatenAusDatenbankInModelEin(string connectionString)
        { 
            var repository = new BuchRepository(connectionString);

            List<BuchDTO>? aktuelleBuecherListe = new();
            List<BuchDTO>? archivierteBuecherListe = new();

            //Auslesen der beiden Tabellen und befüllen der Listen mit den Daten in zwei Unterschiedlichen Threads (zur Parallelisierung)
            Thread leseAlleAktuellenBuecherEin = new Thread(() =>
            {
                aktuelleBuecherListe = repository.HoleAlleAktuelleBuecher();
            });

            Thread leseAlleArchiviertenBuecherEin = new Thread(() =>
            {
                archivierteBuecherListe = repository.HoleAlleArchivierteBuecher();
            });

            //Ausführen der oben erstellten Threads
            leseAlleAktuellenBuecherEin.Start();
            leseAlleArchiviertenBuecherEin.Start();

            leseAlleAktuellenBuecherEin.Join();
            leseAlleArchiviertenBuecherEin.Join();

            return new BuchListeModel(aktuelleBuecherListe, archivierteBuecherListe);
        }

        public void VerschiebenInAndereTabelle(BuchDTO buch, string Ursprungstabelle, string Zieltabelle)
        {
            string connectionString = GetConnectionString();
            //Repository zum verschieben wird Initialisiert
            var repository = new BuchRepository(connectionString);
            repository.VerschiebeBuchInAndereTabelle(buch, Ursprungstabelle, Zieltabelle); //Verschieben des Buches (Löschen aus der 1. Tabelle und Einfügen in die andere Tabelle) wird ausgeführt
        }

        public IActionResult VerschiebeVonArchiviertNachAktuell(BuchDTO buch)
        {
            VerschiebenInAndereTabelle(buch, "archivierte_buecher", "aktuelle_buecher"); //Start des verschiebens eines archivierten Buches, zu einem aktuellen Buch

            BuchListeModel model = LeseDatenAusDatenbankInModelEin(GetConnectionString()); //Neues Laden der beiden Tabellen, nach obiger Verschiebung

            return View("Views/Buch/Index.cshtml", model); //erstellen der View
        }

        public IActionResult VerschiebeVonAktuellNachArchiviert(BuchDTO buch)
        {
            VerschiebenInAndereTabelle(buch, "aktuelle_buecher", "archivierte_buecher"); //Start des verschiebens eines aktuellen Buches, zu einem archivierten Buch

            BuchListeModel model = LeseDatenAusDatenbankInModelEin(GetConnectionString()); //Neues Laden der beiden Tabellen, nach obiger Verschiebung

            return View("Views/Buch/Index.cshtml", model); //erstellen der View
        }



    }
}
