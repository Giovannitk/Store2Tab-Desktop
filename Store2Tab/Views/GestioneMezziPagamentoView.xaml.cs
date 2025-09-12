using Store2Tab.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    public partial class GestioneMezziPagamentoView : UserControl
    {
        public GestioneMezziPagamentoView()
        {
            InitializeComponent();
            DataContext = new MezziPagamentoViewModel();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MezziPagamentoViewModel viewModel)
            {
                viewModel.NuovoMezzoPagamento();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MezziPagamentoViewModel viewModel)
            {
                viewModel.SalvaMezzoPagamento();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MezziPagamentoViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare il mezzo di pagamento selezionato?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaMezzoPagamento();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MezziPagamentoViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MezziPagamentoViewModel viewModel)
            {
                viewModel.CercaMezziPagamento();
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Gestione selezione dalla lista
            if (DataContext is MezziPagamentoViewModel viewModel)
            {
                if (MezziPagamentoListBox.SelectedItem is ListBoxItem selectedItem)
                {
                    viewModel.SelezionaMezzoPagamento(selectedItem.Content.ToString());
                }
            }
        }
    }
}