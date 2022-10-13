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
        // In folgendem Multiline Comment ist der Code zu sehen, der für Dependency Injection benötigt wird
        // Da dieser allerdings einen Fehler erzeugt, nur als Kommentar hier angeführt
        // Der Code würde die Funktion GetConnectionString() ersetzen

        /*private readonly KonfigurationsLeser _konfigurationsLeser;

        public BuchController(KonfigurationsLeser konfigurationsleser)
        {
            this._konfigurationsLeser = konfigurationsleser;
        }

        public string GetConnectionString()
        {
            return _konfigurationsLeser.LiesDatenbankVerbindungZurMariaDB();
        }*/

        // Auslesen des Connection Strings zur Datenbank
        public string GetConnectionString()
        {
            return "Server=localhost;User ID=root;Password=password;Database=BuchDB;"; 
        }

        public IActionResult Index()
        {
            string connectionString = this.GetConnectionString();
            // Einlesen der Daten aus der Datenbank in das Model
            BuchListeModel model = LeseDatenAusDatenbankInModelEin(connectionString);   
            // Laden der View
            return View(model);

        }

        public BuchListeModel LeseDatenAusDatenbankInModelEin(string connectionString)
        { 
            // Erzeugen einer Instanz der Klasse BuchRepository, um Zugriff auf die notwendigen Funktionen zum Auslesen der Datenbanktabellen zu bekommen
            var repository = new BuchRepository(connectionString);

            // Erstellen von 2 Listen, welche die Datensätze der Tabellen aktuelle_buecher und archivierte_buecher speichern sollen
            List<BuchDTO>? aktuelleBuecherListe = new();
            List<BuchDTO>? archivierteBuecherListe = new();

            // Auslesen der Datensätze aus den 2 Datenbank-Tabellen und speichern der Datensätze in den zwei oben erstellten Listen
            // Zur Parallelisierung erfolgt dies in 2 Threads
            Thread leseAlleAktuellenBuecherEin = new Thread(() =>
            {
                aktuelleBuecherListe = repository.HoleAlleAktuelleBuecher();
            });

            Thread leseAlleArchiviertenBuecherEin = new Thread(() =>
            {
                archivierteBuecherListe = repository.HoleAlleArchivierteBuecher();
            });

            // Ausführen der 2 oben erstellten Threads
            leseAlleAktuellenBuecherEin.Start();
            leseAlleArchiviertenBuecherEin.Start();

            leseAlleAktuellenBuecherEin.Join();
            leseAlleArchiviertenBuecherEin.Join();

            return new BuchListeModel(aktuelleBuecherListe, archivierteBuecherListe);
        }

        public void VerschiebenInAndereTabelle(BuchDTO buch, string Ursprungstabelle, string Zieltabelle)
        {
            string connectionString = GetConnectionString();
            // Erzeugen einer Instanz der Klasse BuchRepository, um Zugriff auf die notwendigen Funktionen zum Verschieben der Datensätze zwischen den Tabellen zu bekommen
            var repository = new BuchRepository(connectionString);
            // Verschieben eines Datensatzes in eine andere Tabelle
            repository.VerschiebeBuchInAndereTabelle(buch, Ursprungstabelle, Zieltabelle); 
        }

        // Verschieben eines Datensatzes von der Tabelle archivierte_buecher in die Tabelle aktuelle_buecher
        public IActionResult VerschiebeVonArchiviertNachAktuell(BuchDTO buch)
        {
            VerschiebenInAndereTabelle(buch, "archivierte_buecher", "aktuelle_buecher");
            // Erneutes Einlesen der Daten aus beiden Tabellen
            BuchListeModel model = LeseDatenAusDatenbankInModelEin(GetConnectionString()); 
            // Erstellen der View
            return View("Views/Buch/Index.cshtml", model); 
        }

        // Verschieben eines Datensatzes von der Tabelle aktuelle_buecher in die Tabelle archivierte_buecher
        public IActionResult VerschiebeVonAktuellNachArchiviert(BuchDTO buch)
        {
            VerschiebenInAndereTabelle(buch, "aktuelle_buecher", "archivierte_buecher"); 
            // Erneutes Einlesen der Daten aus beiden Tabellen
            BuchListeModel model = LeseDatenAusDatenbankInModelEin(GetConnectionString()); 
            // Erstellen der View
            return View("Views/Buch/Index.cshtml", model); 
        }



    }
}
