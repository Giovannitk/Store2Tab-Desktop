// Store2Tab/Views/GestioneBancheView.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using Store2Tab.ViewModels;
using Store2Tab.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    public partial class GestioneBancheView : UserControl
    {
        public GestioneBancheView()
        {
            InitializeComponent();

            // Ottiene il ViewModel tramite Dependency Injection
            var viewModel = App.ServiceProvider.GetRequiredService<BancheViewModel>();
            DataContext = viewModel;

            // Imposta l'ItemsSource per il DataGrid
            BancheDataGrid.ItemsSource = viewModel.Banche;
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
                // Ottiene gli elementi selezionati
                var elementiSelezionati = BancheDataGrid.SelectedItems.Cast<Banca>().ToList();

                if (elementiSelezionati.Count == 0)
                {
                    MessageBox.Show("Nessuna banca selezionata per la cancellazione.",
                        "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (elementiSelezionati.Count == 1)
                {
                    // Cancellazione singola - comportamento originale
                    var result = MessageBox.Show(
                        $"Sei sicuro di voler cancellare la banca '{elementiSelezionati[0].Denominazione}'?",
                        "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        viewModel.CancellaBancaSingola(elementiSelezionati[0]);
                    }
                }
                else
                {
                    // Cancellazione multipla
                    var result = MessageBox.Show(
                        $"Hai selezionato {elementiSelezionati.Count} banche per la cancellazione.\n\n" +
                        "Verrai chiesto una conferma per ogni banca.\n\nContinuare?",
                        "Cancellazione Multipla", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        viewModel.CancellaBancheMultiple(elementiSelezionati);
                    }
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
            // Aggiorna il ViewModel con le selezioni multiple
            if (DataContext is BancheViewModel viewModel)
            {
                var elementiSelezionati = BancheDataGrid.SelectedItems.Cast<Banca>().ToList();
                viewModel.ElementiSelezionati = elementiSelezionati;

                // Mantiene il comportamento della selezione singola per il binding
                if (elementiSelezionati.Count == 1)
                {
                    viewModel.SelectedBanca = elementiSelezionati[0];
                }
                else if (elementiSelezionati.Count == 0)
                {
                    viewModel.SelectedBanca = null;
                }
                // Per selezione multipla, SelectedBanca rimane quello che è
            }
        }
    }
}