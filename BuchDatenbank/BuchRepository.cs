using MySqlConnector;

namespace BuchDatenbank
{
    public class BuchRepository
    {
        public string _connectionString;

        // Setzen des Connection Strings zur Datenbank
        public BuchRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        // Auslesen aller Datensätze aus der Tabelle aktuelle_buecher 
        public List<BuchDTO> HoleAlleAktuelleBuecher()
        {
            return HoleAlleDatensaetzeEinerTabelle("aktuelle_buecher");
        }

        // Auslesen aller Datensätze aus der Tabelle archivierte_buecher 
        public List<BuchDTO> HoleAlleArchivierteBuecher()
        {
            return HoleAlleDatensaetzeEinerTabelle("archivierte_buecher");
        }

        // Aulesen aller Datensätze aus einer Tabelle
        public List<BuchDTO> HoleAlleDatensaetzeEinerTabelle(string tabellenname)
        {
            List<BuchDTO> Buecher = new();

            // Start und Ausführung der Datenbankabfrage zum Auslesen aller Datensätze einer Tabelle 
            using var db_Verbindung = new MySqlConnection(_connectionString);
            db_Verbindung.Open();
            string query = "SELECT titel, autor FROM " + tabellenname;
            using var commando = new MySqlCommand(query, db_Verbindung);
            using var reader = commando.ExecuteReader();

            while (reader.Read())
            {
                BuchDTO buch = new BuchDTO
                {
                    Autor = (string?)reader["autor"],
                    Titel = (string?)reader["titel"]
                };

                Buecher.Add(buch);
            }

            db_Verbindung.Close();

            return Buecher;
        }

        // Verschieben eines Datensatzes von einer Tabelle in eine andere Tabelle
        public void VerschiebeBuchInAndereTabelle(BuchDTO buch, string UrsprungsTabellenName, string ZielTabellenName)
        {
            using var db_Verbindung = new MySqlConnection(_connectionString);
            // Löschen des Datensatzes aus der Ursprungstabelle
            LoescheBuchAusTabelle(buch, UrsprungsTabellenName); 
            // Einfügen des Datensatzes in die Zieltabelle
            FuegeBuchEin(buch, ZielTabellenName);
        }

        // Löschen eines Datensatzes aus einer Tabelle
        public void LoescheBuchAusTabelle(BuchDTO buch, string Tabellenname)
        {
            // Start und Ausführung der Datenbankabfrage zum Löschen eines Datensatzes aus einer Tabelle 
            using var db_Verbindung = new MySqlConnection(_connectionString);
            db_Verbindung.Open();

            string query = "DELETE FROM " + Tabellenname + " WHERE titel = @titel AND autor = @autor";
            using var command = new MySqlCommand(query, db_Verbindung);
            command.Parameters.AddWithValue("titel", buch.Titel);
            command.Parameters.AddWithValue("autor", buch.Autor);
            command.ExecuteNonQuery();

            db_Verbindung.Close();
        }

        // Einfügen eines Datensatzes in einer Tabelle
        public void FuegeBuchEin(BuchDTO buch, string Tabellenname)
        {
            // Start und Ausführung der Datenbankabfrage zum Einfügen eines Datensatzes in eine Tabelle
            using var db_Verbindung = new MySqlConnection(_connectionString);
            db_Verbindung.Open();

            string query = "INSERT INTO " + Tabellenname + " (titel, autor) VALUES ('" + buch.Titel + "', '" + buch.Autor + "')";
            using var command = new MySqlCommand(query, db_Verbindung);
            command.Parameters.AddWithValue("titel", buch.Titel);
            command.Parameters.AddWithValue("autor", buch.Autor);
            command.ExecuteNonQuery();

            db_Verbindung.Close();
        }
    }
}