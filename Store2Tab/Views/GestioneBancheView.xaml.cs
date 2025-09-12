using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    public partial class GestioneBancheView : UserControl
    {
        public GestioneBancheView()
        {
            InitializeComponent();
            DataContext = new BancheViewModel();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BancheViewModel viewModel)
            {
                viewModel.NuovaBanca();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BancheViewModel viewModel)
            {
                viewModel.SalvaBanca();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BancheViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare la banca selezionata?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaBanca();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BancheViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is BancheViewModel viewModel)
            {
                viewModel.CercaBanche();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Gestione selezione
        }
    }
}