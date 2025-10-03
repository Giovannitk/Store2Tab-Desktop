using Store2Tab.Data.Models;
using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    public partial class GestioneSchedaTrasportoView : UserControl
    {
        private SchedaTrasportoViewModel ViewModel => (SchedaTrasportoViewModel)DataContext;
        private bool _isInitializing = false;

        public GestioneSchedaTrasportoView()
        {
            InitializeComponent();

            DataContext = new SchedaTrasportoViewModel();

            Loaded += GestioneSchedaTrasportoView_Loaded;
        }

        private async void GestioneSchedaTrasportoView_Loaded(object sender, RoutedEventArgs e)
        {

            if (ViewModel != null)
            {
                await ViewModel.CaricaDatiDefaultAsync();
                CaricaDatiNellaUI();
                AggiungiEventHandlers();
            }
            else
            {
                MessageBox.Show("ViewModel è NULL!");
            }
        }

        private void AggiungiEventHandlers()
        {
            // Aggiungi TextChanged a tutti i TextBox per segnare come modificato
            DatiVettoreTextBox.TextChanged += TextBox_TextChanged;
            PartitaIvaVettoreTextBox.TextChanged += TextBox_TextChanged;
            AlboAutotrasportoTextBox.TextChanged += TextBox_TextChanged;
            DatiCommittenteTextBox.TextChanged += TextBox_TextChanged;
            PartitaIvaCommittenteTextBox.TextChanged += TextBox_TextChanged;
            DatiCaricatoreTextBox.TextChanged += TextBox_TextChanged;
            PartitaIvaCaricatoreTextBox.TextChanged += TextBox_TextChanged;
            DatiProprietarioMerceTextBox.TextChanged += TextBox_TextChanged;
            PartitaIvaProprietarioTextBox.TextChanged += TextBox_TextChanged;
            EventualiDichiarazioniTextBox.TextChanged += TextBox_TextChanged;
            TipologiaMerceTextBox.TextChanged += TextBox_TextChanged;
            QuantitaPesoTextBox.TextChanged += TextBox_TextChanged;
            LuogoCaricoTextBox.TextChanged += TextBox_TextChanged;
            LuogoScaricoTextBox.TextChanged += TextBox_TextChanged;
            LuogoCompilazioneTextBox.TextChanged += TextBox_TextChanged;
            CompilatoreTextBox.TextChanged += TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitializing && ViewModel != null)
            {
                ViewModel.SegnaModificato();
            }
        }

        private async void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;

            // Raccogli i dati dai controlli
            var scheda = RaccoliDatiDaUI();

            // Valida i dati
            if (ViewModel.ValidaDatiScheda(scheda))
            {
                bool successo = await ViewModel.SalvaSchedaTrasportoAsync(scheda);

                if (successo)
                {
                    // Ricarica i dati aggiornati nella UI
                    CaricaDatiNellaUI();
                }
            }
        }

        private async void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel?.SchedaCorrente == null || ViewModel.SchedaCorrente.IdSchedaTrasporto == 0)
            {
                MessageBox.Show("Nessuna scheda da cancellare.", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool successo = await ViewModel.CancellaSchedaTrasportoAsync(ViewModel.SchedaCorrente.IdSchedaTrasporto);

            if (successo)
            {
                PulisciCampi();
            }
        }

        private async void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null) return;

            await ViewModel.AnnullaModificheAsync();
            CaricaDatiNellaUI();
        }

        private SchedaTrasporto RaccoliDatiDaUI()
        {
            var scheda = ViewModel?.SchedaCorrente ?? new SchedaTrasporto();

            scheda.VettoreDescrizione = DatiVettoreTextBox.Text ?? string.Empty;
            scheda.VettorePartitaIva = PartitaIvaVettoreTextBox.Text ?? string.Empty;
            scheda.VettoreAlboAutotrasportatori = AlboAutotrasportoTextBox.Text ?? string.Empty;
            scheda.CommittenteDescrizione = DatiCommittenteTextBox.Text ?? string.Empty;
            scheda.CommittentePartitaIva = PartitaIvaCommittenteTextBox.Text ?? string.Empty;
            scheda.CaricatoreDescrizione = DatiCaricatoreTextBox.Text ?? string.Empty;
            scheda.CaricatorePartitaIva = PartitaIvaCaricatoreTextBox.Text ?? string.Empty;
            scheda.ProprietarioDescrizione = DatiProprietarioMerceTextBox.Text ?? string.Empty;
            scheda.ProprietarioPartitaIva = PartitaIvaProprietarioTextBox.Text ?? string.Empty;

            // Gestione placeholder per Dichiarazioni
            var dichiarazioni = EventualiDichiarazioniTextBox.Text ?? string.Empty;
            scheda.Dichiarazioni = dichiarazioni == "Eventuali dichiarazioni (in assenza di proprietario)"
                ? string.Empty
                : dichiarazioni;

            scheda.MerceTipologia = TipologiaMerceTextBox.Text ?? string.Empty;
            scheda.MerceQuantitaPeso = QuantitaPesoTextBox.Text ?? string.Empty;
            scheda.MerceLuogoCarico = LuogoCaricoTextBox.Text ?? string.Empty;
            scheda.MerceLuogoScarico = LuogoScaricoTextBox.Text ?? string.Empty;
            scheda.Luogo = LuogoCompilazioneTextBox.Text ?? string.Empty;
            scheda.Compilatore = CompilatoreTextBox.Text ?? string.Empty;

            return scheda;
        }

        private void CaricaDatiNellaUI()
        {
            if (ViewModel?.SchedaCorrente == null) return;

            _isInitializing = true;

            try
            {
                var scheda = ViewModel.SchedaCorrente;

                DatiVettoreTextBox.Text = scheda.VettoreDescrizione;
                PartitaIvaVettoreTextBox.Text = scheda.VettorePartitaIva;
                AlboAutotrasportoTextBox.Text = scheda.VettoreAlboAutotrasportatori;
                DatiCommittenteTextBox.Text = scheda.CommittenteDescrizione;
                PartitaIvaCommittenteTextBox.Text = scheda.CommittentePartitaIva;
                DatiCaricatoreTextBox.Text = scheda.CaricatoreDescrizione;
                PartitaIvaCaricatoreTextBox.Text = scheda.CaricatorePartitaIva;
                DatiProprietarioMerceTextBox.Text = scheda.ProprietarioDescrizione;
                PartitaIvaProprietarioTextBox.Text = scheda.ProprietarioPartitaIva;

                // Gestione placeholder per Dichiarazioni
                EventualiDichiarazioniTextBox.Text = string.IsNullOrWhiteSpace(scheda.Dichiarazioni)
                    ? "Eventuali dichiarazioni (in assenza di proprietario)"
                    : scheda.Dichiarazioni;

                TipologiaMerceTextBox.Text = scheda.MerceTipologia;
                QuantitaPesoTextBox.Text = scheda.MerceQuantitaPeso;
                LuogoCaricoTextBox.Text = scheda.MerceLuogoCarico;
                LuogoScaricoTextBox.Text = scheda.MerceLuogoScarico;
                LuogoCompilazioneTextBox.Text = scheda.Luogo;
                CompilatoreTextBox.Text = scheda.Compilatore;
            }
            finally
            {
                _isInitializing = false;
            }
        }

        private void PulisciCampi()
        {
            _isInitializing = true;

            try
            {
                DatiVettoreTextBox.Text = string.Empty;
                PartitaIvaVettoreTextBox.Text = string.Empty;
                AlboAutotrasportoTextBox.Text = string.Empty;
                DatiCommittenteTextBox.Text = string.Empty;
                PartitaIvaCommittenteTextBox.Text = string.Empty;
                DatiCaricatoreTextBox.Text = string.Empty;
                PartitaIvaCaricatoreTextBox.Text = string.Empty;
                DatiProprietarioMerceTextBox.Text = string.Empty;
                PartitaIvaProprietarioTextBox.Text = string.Empty;
                EventualiDichiarazioniTextBox.Text = "Eventuali dichiarazioni (in assenza di proprietario)";
                TipologiaMerceTextBox.Text = string.Empty;
                QuantitaPesoTextBox.Text = string.Empty;
                LuogoCaricoTextBox.Text = string.Empty;
                LuogoScaricoTextBox.Text = string.Empty;
                LuogoCompilazioneTextBox.Text = string.Empty;
                CompilatoreTextBox.Text = string.Empty;
            }
            finally
            {
                _isInitializing = false;
            }
        }
    }
}