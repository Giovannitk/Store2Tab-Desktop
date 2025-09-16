// Store2Tab/ViewModels/ConfigurationViewModel.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class ConfigurationViewModel : INotifyPropertyChanged
    {
        private string _server = "SERVER2019\\PSERVICE";
        private string _database = "pss_b_marcello";
        private string _userId = "sa";
        private string _password = "barcatfilcat";
        private bool _useWindowsAuthentication = false;
        private bool _trustServerCertificate = true;
        private int _connectionTimeout = 30;
        private bool _isTestingConnection = false;

        public string Server
        {
            get => _server;
            set
            {
                _server = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public string Database
        {
            get => _database;
            set
            {
                _database = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public bool UseWindowsAuthentication
        {
            get => _useWindowsAuthentication;
            set
            {
                _useWindowsAuthentication = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public bool TrustServerCertificate
        {
            get => _trustServerCertificate;
            set
            {
                _trustServerCertificate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public int ConnectionTimeout
        {
            get => _connectionTimeout;
            set
            {
                _connectionTimeout = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ConnectionString));
            }
        }

        public bool IsTestingConnection
        {
            get => _isTestingConnection;
            set
            {
                _isTestingConnection = value;
                OnPropertyChanged();
            }
        }

        public string ConnectionString
        {
            get
            {
                var connectionStringBuilder = new System.Text.StringBuilder();
                connectionStringBuilder.Append($"Server={Server};Database={Database};");

                if (UseWindowsAuthentication)
                {
                    connectionStringBuilder.Append("Integrated Security=true;");
                }
                else
                {
                    connectionStringBuilder.Append($"User Id={UserId};Password={Password};");
                }

                if (TrustServerCertificate)
                {
                    connectionStringBuilder.Append("TrustServerCertificate=true;");
                }

                connectionStringBuilder.Append($"Connection Timeout={ConnectionTimeout};");

                return connectionStringBuilder.ToString();
            }
        }

        public async void TestConnection()
        {
            if (IsTestingConnection) return;

            try
            {
                IsTestingConnection = true;

                var success = await ServiceConfiguration.TestConnectionStringAsync(ConnectionString);

                if (success)
                {
                    MessageBox.Show("Connessione riuscita!", "Test Connessione",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Connessione fallita. Verificare i parametri.", "Test Connessione",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il test: {ex.Message}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsTestingConnection = false;
            }
        }

        public async void SaveConfiguration()
        {
            try
            {
                // Prima testa la connessione
                var success = await ServiceConfiguration.TestConnectionStringAsync(ConnectionString);

                if (!success)
                {
                    var result = MessageBox.Show(
                        "La connessione non funziona. Salvare comunque?",
                        "Avviso",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result != MessageBoxResult.Yes)
                        return;
                }

                ServiceConfiguration.SaveUserConnectionString(ConnectionString);

                MessageBox.Show(
                    "Configurazione salvata! Riavviare l'applicazione per applicare le modifiche.",
                    "Configurazione Salvata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il salvataggio: {ex.Message}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadCurrentConfiguration()
        {
            try
            {
                // Carica la configurazione attuale e la parsifica
                var currentConnectionString = ServiceConfiguration.GetEffectiveConnectionString()
                    ?? ServiceConfiguration.GetEffectiveDefaultConnectionString();

                ParseConnectionString(currentConnectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il caricamento: {ex.Message}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ParseConnectionString(string connectionString)
        {
            // Parser semplice per la connection string
            var parts = connectionString.Split(';');

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part)) continue;

                var keyValue = part.Split('=');
                if (keyValue.Length != 2) continue;

                var key = keyValue[0].Trim().ToLower();
                var value = keyValue[1].Trim();

                switch (key)
                {
                    case "server":
                        Server = value;
                        break;
                    case "database":
                        Database = value;
                        break;
                    case "user id":
                        UserId = value;
                        UseWindowsAuthentication = false;
                        break;
                    case "password":
                        Password = value;
                        break;
                    case "integrated security":
                        UseWindowsAuthentication = value.ToLower() == "true";
                        break;
                    case "trustservercertificate":
                        TrustServerCertificate = value.ToLower() == "true";
                        break;
                    case "connection timeout":
                        if (int.TryParse(value, out int timeout))
                            ConnectionTimeout = timeout;
                        break;
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}