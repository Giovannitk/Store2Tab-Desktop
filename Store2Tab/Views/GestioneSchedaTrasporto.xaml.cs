using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per i dati della scheda trasporto
    /// </summary>
    public class SchedaTrasportoData
    {
        // Dati Vettore
        public string DatiVettore { get; set; } = string.Empty;
        public string PartitaIvaVettore { get; set; } = string.Empty;
        public string AlboAutotrasporto { get; set; } = string.Empty;

        // Dati Committente
        public string DatiCommittente { get; set; } = string.Empty;
        public string PartitaIvaCommittente { get; set; } = string.Empty;

        // Dati Caricatore
        public string DatiCaricatore { get; set; } = string.Empty;
        public string PartitaIvaCaricatore { get; set; } = string.Empty;

        // Dati Proprietario Merce
        public string DatiProprietarioMerce { get; set; } = string.Empty;
        public string PartitaIvaProprietario { get; set; } = string.Empty;
        public string EventualiDichiarazioni { get; set; } = string.Empty;

        // Dati Merce Trasportata
        public string TipologiaMerce { get; set; } = string.Empty;
        public string QuantitaPeso { get; set; } = string.Empty;
        public string LuogoCarico { get; set; } = string.Empty;
        public string LuogoScarico { get; set; } = string.Empty;

        // Dati Compilazione
        public string LuogoCompilazione { get; set; } = string.Empty;
        public string Compilatore { get; set; } = string.Empty;
    }

    public partial class GestioneSchedaTrasportoView : UserControl
    {
        private SchedaTrasportoData _currentData;

        public GestioneSchedaTrasportoView()
        {
            InitializeComponent();
            DataContext = new SchedaTrasportoViewModel();
            _currentData = new SchedaTrasportoData();

            // Inizializza con i placeholder
            InitializePlaceholders();
        }

        private void InitializePlaceholders()
        {
            // Imposta i placeholder text per aiutare l'utente
            EventualiDichiarazioniTextBox.Text = "Eventuali dichiarazioni (in assenza di proprietario)";
            EventualiDichiarazioniTextBox.GotFocus += (s, e) =>
            {
                if (EventualiDichiarazioniTextBox.Text == "Eventuali dichiarazioni (in assenza di proprietario)")
                {
                    EventualiDichiarazioniTextBox.Text = "";
                }
            };
            EventualiDichiarazioniTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(EventualiDichiarazioniTextBox.Text))
                {
                    EventualiDichiarazioniTextBox.Text = "Eventuali dichiarazioni (in assenza di proprietario)";
                }
            };
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            // Raccogli i dati dai controlli
            CollectDataFromControls();

            if (DataContext is SchedaTrasportoViewModel viewModel)
            {
                // Valida i dati prima di salvare
                if (ValidateData())
                {
                    viewModel.SalvaSchedaTrasporto();
                }
                else
                {
                    MessageBox.Show("Alcuni campi obbligatori sono vuoti. Controllare i dati inseriti.",
                                  "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SchedaTrasportoViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare la scheda trasporto corrente?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaSchedaTrasporto();
                    ClearAllFields();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SchedaTrasportoViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler annullare le modifiche?",
                    "Conferma Annulla", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    viewModel.AnnullaModifiche();
                    ClearAllFields();
                }
            }
        }

        private void CollectDataFromControls()
        {
            _currentData.DatiVettore = DatiVettoreTextBox.Text;
            _currentData.PartitaIvaVettore = PartitaIvaVettoreTextBox.Text;
            _currentData.AlboAutotrasporto = AlboAutotrasportoTextBox.Text;

            _currentData.DatiCommittente = DatiCommittenteTextBox.Text;
            _currentData.PartitaIvaCommittente = PartitaIvaCommittenteTextBox.Text;

            _currentData.DatiCaricatore = DatiCaricatoreTextBox.Text;
            _currentData.PartitaIvaCaricatore = PartitaIvaCaricatoreTextBox.Text;

            _currentData.DatiProprietarioMerce = DatiProprietarioMerceTextBox.Text;
            _currentData.PartitaIvaProprietario = PartitaIvaProprietarioTextBox.Text;
            _currentData.EventualiDichiarazioni = EventualiDichiarazioniTextBox.Text;

            _currentData.TipologiaMerce = TipologiaMerceTextBox.Text;
            _currentData.QuantitaPeso = QuantitaPesoTextBox.Text;
            _currentData.LuogoCarico = LuogoCaricoTextBox.Text;
            _currentData.LuogoScarico = LuogoScaricoTextBox.Text;

            _currentData.LuogoCompilazione = LuogoCompilazioneTextBox.Text;
            _currentData.Compilatore = CompilatoreTextBox.Text;
        }

        private bool ValidateData()
        {
            // Verifica che almeno i campi principali siano compilati
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(DatiVettoreTextBox.Text))
                isValid = false;

            if (string.IsNullOrWhiteSpace(DatiCommittenteTextBox.Text))
                isValid = false;

            if (string.IsNullOrWhiteSpace(TipologiaMerceTextBox.Text))
                isValid = false;

            if (string.IsNullOrWhiteSpace(LuogoCaricoTextBox.Text))
                isValid = false;

            if (string.IsNullOrWhiteSpace(LuogoScaricoTextBox.Text))
                isValid = false;

            return isValid;
        }

        private void ClearAllFields()
        {
            // Pulisce tutti i campi
            DatiVettoreTextBox.Text = "";
            PartitaIvaVettoreTextBox.Text = "";
            AlboAutotrasportoTextBox.Text = "";

            DatiCommittenteTextBox.Text = "";
            PartitaIvaCommittenteTextBox.Text = "";

            DatiCaricatoreTextBox.Text = "";
            PartitaIvaCaricatoreTextBox.Text = "";

            DatiProprietarioMerceTextBox.Text = "";
            PartitaIvaProprietarioTextBox.Text = "";
            EventualiDichiarazioniTextBox.Text = "Eventuali dichiarazioni (in assenza di proprietario)";

            TipologiaMerceTextBox.Text = "";
            QuantitaPesoTextBox.Text = "";
            LuogoCaricoTextBox.Text = "";
            LuogoScaricoTextBox.Text = "";

            LuogoCompilazioneTextBox.Text = "";
            CompilatoreTextBox.Text = "";

            _currentData = new SchedaTrasportoData();
        }

        public void LoadSchedaTrasporto(SchedaTrasportoData data)
        {
            if (data == null) return;

            DatiVettoreTextBox.Text = data.DatiVettore;
            PartitaIvaVettoreTextBox.Text = data.PartitaIvaVettore;
            AlboAutotrasportoTextBox.Text = data.AlboAutotrasporto;

            DatiCommittenteTextBox.Text = data.DatiCommittente;
            PartitaIvaCommittenteTextBox.Text = data.PartitaIvaCommittente;

            DatiCaricatoreTextBox.Text = data.DatiCaricatore;
            PartitaIvaCaricatoreTextBox.Text = data.PartitaIvaCaricatore;

            DatiProprietarioMerceTextBox.Text = data.DatiProprietarioMerce;
            PartitaIvaProprietarioTextBox.Text = data.PartitaIvaProprietario;
            EventualiDichiarazioniTextBox.Text = data.EventualiDichiarazioni;

            TipologiaMerceTextBox.Text = data.TipologiaMerce;
            QuantitaPesoTextBox.Text = data.QuantitaPeso;
            LuogoCaricoTextBox.Text = data.LuogoCarico;
            LuogoScaricoTextBox.Text = data.LuogoScarico;

            LuogoCompilazioneTextBox.Text = data.LuogoCompilazione;
            CompilatoreTextBox.Text = data.Compilatore;

            _currentData = data;
        }
    }
}