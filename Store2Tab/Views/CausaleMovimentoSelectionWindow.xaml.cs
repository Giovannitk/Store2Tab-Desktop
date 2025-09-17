using Store2Tab.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Store2Tab.Views
{
    public partial class CausaleMovimentoSelectionWindow : Window
    {
        public CausaleMovimentoSelectionWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<CausaleMovimentoSelectionViewModel>();

            // Focus iniziale sul campo di ricerca codice
            Loaded += (s, e) => CodiceRicercaTextBox.Focus();
        }

        private async void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaleMovimentoSelectionViewModel viewModel)
            {
                await viewModel.CercaCausali();
            }
        }

        private async void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is CausaleMovimentoSelectionViewModel viewModel)
                {
                    await viewModel.CercaCausali();
                }
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CausaleMovimentoSelectionViewModel viewModel &&
                viewModel.CausaleSelezionata != null)
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Selezionare una causale movimento.", "Attenzione",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is CausaleMovimentoSelectionViewModel viewModel &&
                viewModel.CausaleSelezionata != null)
            {
                DialogResult = true;
            }
        }
    }
}