using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid
    /// </summary>
    public class NumerazioneOrdiniItem
    {
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public string StampaTelefonoCliente { get; set; } = string.Empty;
    }

    public partial class GestioneNumerazioneOrdiniView : UserControl
    {
        public GestioneNumerazioneOrdiniView()
        {
            InitializeComponent();
            DataContext = new NumerazioneOrdiniViewModel();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NumerazioneOrdiniViewModel viewModel)
            {
                viewModel.NuovaNumerazione();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NumerazioneOrdiniViewModel viewModel)
            {
                viewModel.SalvaNumerazione();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NumerazioneOrdiniViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare la numerazione selezionata?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaNumerazione();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NumerazioneOrdiniViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NumerazioneOrdiniViewModel viewModel)
            {
                viewModel.CercaNumerazioni();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is NumerazioneOrdiniViewModel viewModel)
            {
                if (NumerazioneDataGrid.SelectedItem is NumerazioneOrdiniItem selectedItem)
                {
                    viewModel.SelezionaNumerazione(selectedItem.Codice, selectedItem.Descrizione,
                                                  selectedItem.Sigla,
                                                  selectedItem.StampaTelefonoCliente);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    NumerazioneDescrizioneTextBox.Text = selectedItem.Descrizione;
                    SiglaTextBox.Text = selectedItem.Sigla;
                    StampaTelefonoClienteTextBox.Text = selectedItem.StampaTelefonoCliente;
                }
            }
        }

        private void NumerazioneOrdiniClientiCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            NumerazioneOrdiniFornitoriCheckBox.IsChecked = false;
        }

        private void NumerazioneOrdiniFornitoriCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            NumerazioneOrdiniClientiCheckBox.IsChecked = false;
        }

        private void StampaTelefonoClienteTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // accetta solo 1 o 2
            e.Handled = !(e.Text == "1" || e.Text == "2");
        }


    }
}