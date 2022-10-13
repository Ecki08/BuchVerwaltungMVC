using Microsoft.Extensions.Configuration;

namespace BuchVerwaltungMVC.Konfigurationsleser
{
    // Die Klasse Konfigurationsleser liest die Konfiguration aus und damit auch den ConnectionString zur Datenbank
    // Damit würde das Design-Prinzip Dependency Injection realisiert werden
    public class KonfigurationsLeser : IKonfigurationsLeser
    {
        private readonly IConfiguration _configuration;

        public KonfigurationsLeser(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

         public string LiesDatenbankVerbindungZurMariaDB()
        {
            return _configuration.GetConnectionString("MariaDB");
        }        
    }

    public interface IKonfigurationsLeser
    {
        string LiesDatenbankVerbindungZurMariaDB();
    }
    
}