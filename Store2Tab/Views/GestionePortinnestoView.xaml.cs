using Store2Tab.Data.Models;
using Store2Tab.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    public partial class GestionePortinnestoView : UserControl
    {
        private PortinnestoViewModel? _viewModel;
        public GestionePortinnestoView()
        {
            InitializeComponent();
            _viewModel = new PortinnestoViewModel();
            DataContext = _viewModel;

            // Collegamento degli eventi
            PortinnestoDataGrid.SelectionChanged += DataGrid_SelectionChanged;
            CodiceRicercaTextBox.KeyDown += CodiceRicerca_KeyDown;
            DescrizioneRicercaTextBox.KeyDown += DescrizioneRicerca_KeyDown;

            // Popola ComboBox e DataGrid
            //PopulateSpecieBotanicheComboBox();
            //PopulateDataGrid();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.NuovoPortinnesto();

            // Pulisce i campi di dettaglio per inserimento
            CodiceTextBox.Text = _viewModel?.CodiceProposto.ToString() ?? "0";
            SpecieBotanicaComboBox.SelectedItem = null;

            PortinnestoTextBox.Text = "";
            PortinnestoTextBox.Focus();
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.PortinnestoSelezionata != null)
            {
                // Aggiorna i dati dal form prima di salvare
                if (!string.IsNullOrWhiteSpace(PortinnestoTextBox.Text))
                {
                    _viewModel.PortinnestoSelezionata.Portinnesto = PortinnestoTextBox.Text.Trim();
                }

                // Aggiorna la specie selezionata dal ComboBox
                if (SpecieBotanicaComboBox.SelectedItem is PassPianteCeeSpecie specieSelezionata)
                {
                    _viewModel.SpecieSelezionata = specieSelezionata;
                }
            }

            _viewModel?.SalvoPortinnesto();
            AggiornaDettagli();
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.PortinnestoSelezionata == null)
            {
                MessageBox.Show("Seleziona un portinnesto da cancellare.", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Sei sicuro di voler cancellare il portinnesto selezionato?",
                "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _viewModel?.CancellaPortinnesto();
            }
        }

        // Nuovo metodo per la cancellazione multipla
        private void CancellaMultipla_Click(object sender, RoutedEventArgs e)
        {
            var elementiSelezionati = PortinnestoDataGrid.SelectedItems
                .Cast<PassPianteCEE_Portinnesto>()
                .ToList();
            if (elementiSelezionati.Count == 0)
            {
                MessageBox.Show("Selezionare uno o più portinnesti da cancellare.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show($"Sei sicuro di voler cancellare i {elementiSelezionati.Count} portinnesti selezionati?",
                "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _viewModel?.CancellaPortinnestoMultiple(elementiSelezionati);
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.AnnullaModifiche();
            AggiornaDettagli();
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            // Aggiorna i filtri di ricerca dal form
            if (_viewModel != null)
            {
                _viewModel.FiltroCodice = CodiceRicercaTextBox.Text.Trim();
                _viewModel.FiltroPortinnesto = DescrizioneRicercaTextBox.Text.Trim();

                // Se c'è una specie selezionata nella ComboBox del filtro, usala per la ricerca
                // Nota: questo richiederebbe una ComboBox separata nei filtri per la specie

                _viewModel.CercaPortinnesto();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PortinnestoDataGrid.SelectedItem is PassPianteCEE_Portinnesto selectedItem && _viewModel != null)
            {
                _viewModel.SelezionaPortinnesto(selectedItem);
                AggiornaDettagli();
            }
        }

        private void AggiornaDettagli()
        {
            if (_viewModel?.PortinnestoSelezionata != null)
            {
                var portinnesto = _viewModel.PortinnestoSelezionata;

                // Aggiorno i campi del pannello dettagli
                CodiceTextBox.Text = portinnesto.IdPassPianteCEE_Portinnesto.ToString();
                PortinnestoTextBox.Text = portinnesto.Portinnesto ?? "";

                // Seleziona la specie botanica corrispondente nella ComboBox
                var specie = _viewModel.SpecieBotaniche.FirstOrDefault(s =>
                    s.IdPassPianteCEE_Specie == portinnesto.IdPassPianteCEE_Specie);
                SpecieBotanicaComboBox.SelectedItem = specie;
            }
            else
            {
                // Nessun portinnesto selezionato, pulisce i campi
                CodiceTextBox.Text = _viewModel?.CodiceProposto.ToString() ?? "0";
                SpecieBotanicaComboBox.SelectedItem = null;
                PortinnestoTextBox.Text = "";
            }
        }

        // Gestione pressione Enter nei campi di ricerca
        private void CodiceRicerca_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Cerca_Click(sender, e);
            }
        }

        private void DescrizioneRicerca_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Cerca_Click(sender, e);
            }
        }

        // Eventi per attivare il salvataggio quando si modificano i campi
        private void CodiceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Il codice non dovrebbe essere modificabile dall'utente, solo visualizzazione
        }

        private void SpecieBotanicaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // La selezione della specie aggiorna il ViewModel
            if (_viewModel?.PortinnestoSelezionata != null &&
                SpecieBotanicaComboBox.SelectedItem is PassPianteCeeSpecie specieSelezionata)
            {
                _viewModel.SpecieSelezionata = specieSelezionata;
            }
        }

        private void PortinnestoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Aggiorna il ViewModel quando il testo del portinnesto cambia
            if (_viewModel?.PortinnestoSelezionata != null)
            {
                _viewModel.PortinnestoSelezionata.Portinnesto = PortinnestoTextBox.Text;
            }
        }

        // Gestione di tutti i tasti di scelta rapida
        private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.F1:
                    Nuovo_Click(sender, e);
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.F2:
                    Salva_Click(sender, e);
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.F3:
                    Cerca_Click(sender, e);
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.F4:
                    Cancella_Click(sender, e);
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.F5:
                    CancellaMultipla_Click(sender, e);
                    break;
                case System.Windows.Input.Key.F8:
                    Annulla_Click(sender, e);
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.Escape:
                    // Chiude la finestra come nel codice VB6
                    if (Window.GetWindow(this) is Window parentWindow)
                    {
                        parentWindow.Close();
                    }
                    e.Handled = true;
                    break;
            }
        }

    }
}