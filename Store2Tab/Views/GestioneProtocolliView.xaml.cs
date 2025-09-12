using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid dei Protocolli
    /// </summary>
    public class ProtocolloItem
    {
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
    }

    public partial class GestioneProtocolliView : UserControl
    {
        public GestioneProtocolliView()
        {
            InitializeComponent();
            DataContext = new ProtocolliViewModel();

            // Popola con dati di esempio
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            var protocolliEsempio = new List<ProtocolloItem>
            {
                new ProtocolloItem { Codice = "1", Descrizione = "Doc. Emessi" },
                new ProtocolloItem { Codice = "2", Descrizione = "Doc. Ricevuti" },
                new ProtocolloItem { Codice = "3", Descrizione = "Contratti" },
                new ProtocolloItem { Codice = "4", Descrizione = "Fatture Elettroniche" }
            };

            foreach (var protocollo in protocolliEsempio)
            {
                ProtocolliDataGrid.Items.Add(protocollo);
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliViewModel viewModel)
            {
                viewModel.NuovoProtocollo();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliViewModel viewModel)
            {
                viewModel.SalvaProtocollo();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare il protocollo selezionato?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaProtocollo();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliViewModel viewModel)
            {
                viewModel.CercaProtocolli();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ProtocolliViewModel viewModel)
            {
                if (ProtocolliDataGrid.SelectedItem is ProtocolloItem selectedItem)
                {
                    viewModel.SelezionaProtocollo(selectedItem.Codice, selectedItem.Descrizione);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    DescrizioneTextBox.Text = selectedItem.Descrizione;
                }
            }
        }
    }
}