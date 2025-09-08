using Store2Tab.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    public partial class GestioneTipiPagamentoView : UserControl
    {
        public GestioneTipiPagamentoView()
        {
            InitializeComponent();
            DataContext = new TipiPagamentoViewModel();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiPagamentoViewModel viewModel)
            {
                viewModel.NuovoTipoPagamento();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiPagamentoViewModel viewModel)
            {
                viewModel.SalvaTipoPagamento();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiPagamentoViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare il tipo di pagamento selezionato?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaTipoPagamento();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiPagamentoViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiPagamentoViewModel viewModel)
            {
                viewModel.CercaTipiPagamento();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Gestione selezione
        }
    }
}