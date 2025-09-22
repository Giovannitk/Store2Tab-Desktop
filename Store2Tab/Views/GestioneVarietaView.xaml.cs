using Store2Tab.ViewModels;
using Store2Tab.Data.Models;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace Store2Tab.Views
{
    public partial class GestioneVarietaView : UserControl
    {
        private VarietaViewModel? _viewModel;

        public GestioneVarietaView()
        {
            InitializeComponent();
            _viewModel = new VarietaViewModel();
            DataContext = _viewModel;

            // Collegamento degli eventi
            VarietaDataGrid.SelectionChanged += DataGrid_SelectionChanged;
            CodiceRicercaTextBox.KeyDown += CodiceRicerca_KeyDown;
            DescrizioneRicercaTextBox.KeyDown += DescrizioneRicerca_KeyDown;
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.NuovaVarieta();

            // Pulisce i campi di dettaglio per inserimento
            CodiceTextBox.Text = _viewModel?.CodiceProposto.ToString() ?? "0";
            SpecieBotanicaComboBox.SelectedItem = null;
            VarietaTextBox.Text = "";
            VarietaTextBox.Focus();
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.VarietaSelezionata != null)
            {
                // Aggiorna i dati dal form prima di salvare
                if (!string.IsNullOrWhiteSpace(VarietaTextBox.Text))
                {
                    _viewModel.VarietaSelezionata.Varieta = VarietaTextBox.Text.Trim();
                }

                // Aggiorna la specie selezionata dal ComboBox
                if (SpecieBotanicaComboBox.SelectedItem is PassPianteCeeSpecie specieSelezionata)
                {
                    _viewModel.SpecieSelezionata = specieSelezionata;
                }
            }

            _viewModel?.SalvaVarieta();
            AggiornaDettagli();
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.VarietaSelezionata == null)
            {
                MessageBox.Show("Seleziona una varietà da cancellare.", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _viewModel.CancellaVarieta();
        }

        // Nuovo metodo per la cancellazione multipla
        private void CancellaMultipla_Click(object sender, RoutedEventArgs e)
        {
            var elementiSelezionati = VarietaDataGrid.SelectedItems
                .Cast<PassPianteCeeVarieta>()
                .ToList();

            if (elementiSelezionati.Count == 0)
            {
                MessageBox.Show("Selezionare una o più varietà da cancellare.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _viewModel?.CancellaVarietaMultiple(elementiSelezionati);
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.AnnullaModifiche();
            AggiornaDettagli();
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            // Aggiorna i filtri dal form
            if (_viewModel != null)
            {
                _viewModel.FiltroCodice = CodiceRicercaTextBox.Text.Trim();
                _viewModel.FiltroVarieta = DescrizioneRicercaTextBox.Text.Trim();

                // Se c'è una specie selezionata nella ComboBox del filtro, usala per la ricerca
                // Nota: questo richiederebbe una ComboBox separata nei filtri per la specie

                _viewModel.CercaVarieta();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VarietaDataGrid.SelectedItem is PassPianteCeeVarieta selectedItem && _viewModel != null)
            {
                _viewModel.SelezionaVarieta(selectedItem);
                AggiornaDettagli();
            }
        }

        private void AggiornaDettagli()
        {
            if (_viewModel?.VarietaSelezionata != null)
            {
                var varieta = _viewModel.VarietaSelezionata;

                // Aggiorno i campi del pannello dettagli
                CodiceTextBox.Text = varieta.IdPassPianteCEE_Varieta.ToString();
                VarietaTextBox.Text = varieta.Varieta ?? "";

                // Seleziona la specie botanica corrispondente nella ComboBox
                var specie = _viewModel.SpecieBotaniche.FirstOrDefault(s =>
                    s.IdPassPianteCEE_Specie == varieta.IdPassPianteCEE_Specie);
                SpecieBotanicaComboBox.SelectedItem = specie;
            }
            else
            {
                // Pulisce i campi se non c'è selezione
                CodiceTextBox.Text = _viewModel?.CodiceProposto.ToString() ?? "0";
                SpecieBotanicaComboBox.SelectedItem = null;
                VarietaTextBox.Text = "";
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
            if (_viewModel?.VarietaSelezionata != null && SpecieBotanicaComboBox.SelectedItem is PassPianteCeeSpecie specie)
            {
                _viewModel.SpecieSelezionata = specie;
                // Potresti voler attivare qui qualche indicazione di modifica in corso
            }
        }

        private void VarietaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_viewModel?.VarietaSelezionata != null)
            {
                _viewModel.VarietaSelezionata.Varieta = VarietaTextBox.Text;
                // Potresti voler attivare qui qualche indicazione di modifica in corso
            }
        }

        // Gestione dei tasti funzione come nel codice VB6 originale
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