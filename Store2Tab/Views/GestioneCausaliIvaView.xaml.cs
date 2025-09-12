using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    public partial class GestioneCausaliIvaView : UserControl
    {
        public GestioneCausaliIvaView()
        {
            InitializeComponent();
            DataContext = new CausaliIvaViewModel();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliIvaViewModel viewModel)
            {
                viewModel.NuovaCausaleIva();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliIvaViewModel viewModel)
            {
                viewModel.SalvaCausaleIva();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliIvaViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare la causale IVA selezionata?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaCausaleIva();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliIvaViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaliIvaViewModel viewModel)
            {
                viewModel.CercaCausaliIva();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Gestione selezione
        }
    }
}