using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid
    /// </summary>
    public class CausaleMovimentoItem
    {
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
    }

    public partial class GestioneCausaliMovimentoView : UserControl
    {
        public GestioneCausaliMovimentoView()
        {
            InitializeComponent();
            DataContext = new CausaliMovimentoViewModel();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliMovimentoViewModel viewModel)
            {
                viewModel.NuovaCausaleMovimento();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliMovimentoViewModel viewModel)
            {
                viewModel.SalvaCausaleMovimento();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliMovimentoViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare la causale movimento selezionata?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaCausaleMovimento();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliMovimentoViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliMovimentoViewModel viewModel)
            {
                viewModel.CercaCausaliMovimento();
            }
        }

        private void CercaControMovimento_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliMovimentoViewModel viewModel)
            {
                viewModel.CercaCausaliMovimento();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is CausaliMovimentoViewModel viewModel)
            {
                if (CausaliMovimentoDataGrid.SelectedItem is CausaleMovimentoItem selectedItem)
                {
                    viewModel.SelezionaCausaleMovimento(selectedItem.Codice, selectedItem.Descrizione);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    CausaleMovimentoTextBox.Text = selectedItem.Descrizione;
                }
            }
        }

        private void ControMovimento_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliMovimentoViewModel viewModel)
            {
                viewModel.ApriSelezioneControMovimento();
            }
        }
    }
}