using MySqlConnector;

namespace BuchDatenbank
{
    public class BuchRepository
    {
        public string _connectionString;

        public BuchRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }


        public List<BuchDTO> HoleAlleAktuelleBuecher()
        {
            return HoleAlleBuecherVonEinerTabelle("aktuelle_Buecher"); //Liest alle aktuellen Bücher
        }

        public List<BuchDTO> HoleAlleArchivierteBuecher()
        {
            return HoleAlleBuecherVonEinerTabelle("archivierte_Buecher"); //Liest alle archivierten Bücher
        }


        public List<BuchDTO> HoleAlleBuecherVonEinerTabelle(string TabellenName)
        {
            List<BuchDTO> Buecher = new();

            //Hier wird die Datenbankabfrage gestartet und die Abfrage zum Auslesen einer der beiden tabellen wird ausgeführt
            using var db_Verbindung = new MySqlConnection(_connectionString);
            db_Verbindung.Open();
            string queryBuecher = "SELECT titel, autor FROM " + TabellenName;
            using var commando = new MySqlCommand(queryBuecher, db_Verbindung);
            using var reader = commando.ExecuteReader();

            while (reader.Read())
            {
                BuchDTO buch = new BuchDTO
                {
                    //Id = (int)reader["id"],
                    Autor = (string?)reader["autor"],
                    Titel = (string?)reader["titel"]
                };

                Buecher.Add(buch);
            }

            db_Verbindung.Close();

            return Buecher;
        }

        public void VerschiebeBuchInAndereTabelle(BuchDTO buch, string UrsprungsTabellenName, string ZielTabellenName)
        {
            using var db_Verbindung = new MySqlConnection(_connectionString);

            LoescheBuchAusTabelle(buch, UrsprungsTabellenName); //Lösche das Buch aus der Quelltabelle

            FuegeBuchEin(buch, ZielTabellenName); //Füge das buch der Zieltabelle hinzu
        }

        public void LoescheBuchAusTabelle(BuchDTO buch, string Tabellenname)
        {
            //Datenbankverbindung aufbauen und Befehl zum Löschen aus der Quelladresse wird hier ausgeführt
            using var db_Verbindung = new MySqlConnection(_connectionString);
            db_Verbindung.Open();

            string query = "DELETE FROM " + Tabellenname + " WHERE titel = @titel AND autor = @autor";
            using var command = new MySqlCommand(query, db_Verbindung);
            command.Parameters.AddWithValue("titel", buch.Titel);
            command.Parameters.AddWithValue("autor", buch.Autor);

            command.ExecuteNonQuery();

            db_Verbindung.Close();
        }

        public void FuegeBuchEin(BuchDTO buch, string Tabellenname)
        {
            //Datenbankverbindung aufbauen und Befehl zum Einfuegen in die Zieladresse wird hier ausgeführt
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

    public class BuchDTO
    {
        public int Id { get; set; }
        public string? Titel { get; set; }
        public string? Autor { get; set; }
    }
}