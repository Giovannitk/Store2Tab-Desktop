using Store2Tab.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid dei Protocolli Contatori
    /// </summary>
    public class ProtocolloContatoreItem
    {
        public string Anno { get; set; } = string.Empty;
        public string ProtocolloDescrizione { get; set; } = string.Empty;
        public string Contatore { get; set; } = string.Empty;
    }

    public partial class GestioneProtocolliContatoriView : UserControl
    {
        public GestioneProtocolliContatoriView()
        {
            InitializeComponent();
            DataContext = new ProtocolliContatoriViewModel();

            // Popola con dati di esempio
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            var protocolliContatoriEsempio = new List<ProtocolloContatoreItem>
            {
                new ProtocolloContatoreItem { Anno = "2025", ProtocolloDescrizione = "Doc. Emessi", Contatore = "1" },
                new ProtocolloContatoreItem { Anno = "2025", ProtocolloDescrizione = "Doc. Ricevuti", Contatore = "1" },
                new ProtocolloContatoreItem { Anno = "2024", ProtocolloDescrizione = "Doc. Emessi", Contatore = "150" },
                new ProtocolloContatoreItem { Anno = "2024", ProtocolloDescrizione = "Contratti", Contatore = "25" },
                new ProtocolloContatoreItem { Anno = "2023", ProtocolloDescrizione = "Fatture Elettroniche", Contatore = "500" }
            };

            foreach (var item in protocolliContatoriEsempio)
            {
                ProtocolliContatoriDataGrid.Items.Add(item);
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliContatoriViewModel viewModel)
            {
                viewModel.NuovoProtocolloContatore();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliContatoriViewModel viewModel)
            {
                viewModel.SalvaProtocolloContatore();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliContatoriViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare il protocollo contatore selezionato?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaProtocolloContatore();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliContatoriViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliContatoriViewModel viewModel)
            {
                viewModel.CercaProtocolliContatori();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ProtocolliContatoriViewModel viewModel)
            {
                if (ProtocolliContatoriDataGrid.SelectedItem is ProtocolloContatoreItem selectedItem)
                {
                    viewModel.SelezionaProtocolloContatore(selectedItem.Anno, selectedItem.ProtocolloDescrizione, selectedItem.Contatore);

                    // Aggiorna i campi del pannello dettagli
                    AnnoTextBox.Text = selectedItem.Anno;
                    ProtocolloTextBox.Text = selectedItem.ProtocolloDescrizione;
                    ContatoreTextBox.Text = selectedItem.Contatore;
                }
            }
        }
    }
}