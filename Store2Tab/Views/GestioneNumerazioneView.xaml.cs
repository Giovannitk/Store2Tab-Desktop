using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid
    /// </summary>
    public class NumerazioneItem
    {
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public string DocumentoElettronico { get; set; } = "False";
        public string TipoDocumentoElettronico { get; set; } = string.Empty;
    }

    public partial class GestioneNumerazioneView : UserControl
    {
        public GestioneNumerazioneView()
        {
            InitializeComponent();
            DataContext = new NumerazioneViewModel();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NumerazioneViewModel viewModel)
            {
                viewModel.NuovaNumerazione();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NumerazioneViewModel viewModel)
            {
                viewModel.SalvaNumerazione();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NumerazioneViewModel viewModel)
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
            if (DataContext is NumerazioneViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NumerazioneViewModel viewModel)
            {
                viewModel.CercaNumerazioni();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is NumerazioneViewModel viewModel)
            {
                if (NumerazioneDataGrid.SelectedItem is NumerazioneItem selectedItem)
                {
                    viewModel.SelezionaNumerazione(selectedItem.Codice, selectedItem.Descrizione,
                                                  selectedItem.Sigla, selectedItem.DocumentoElettronico,
                                                  selectedItem.TipoDocumentoElettronico);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    NumerazioneDescrizioneTextBox.Text = selectedItem.Descrizione;
                    SiglaTextBox.Text = selectedItem.Sigla;
                    DocumentoElettronicoCheckBox.IsChecked = selectedItem.DocumentoElettronico == "True";
                    TipoDocumentoElettronicoTextBox.Text = selectedItem.TipoDocumentoElettronico;
                }
            }
        }
    }
}