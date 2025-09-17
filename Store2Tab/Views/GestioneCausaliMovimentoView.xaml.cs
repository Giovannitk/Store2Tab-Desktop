using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Store2Tab.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Store2Tab.Data.Models;

namespace Store2Tab.Views
{
    public partial class GestioneCausaliMovimentoView : UserControl
    {
        private CausaliMovimentoViewModel ViewModel => (CausaliMovimentoViewModel)DataContext;

        public GestioneCausaliMovimentoView()
        {
            InitializeComponent();

            // Imposta DataContext via DI per evitare provider null
            DataContext = App.ServiceProvider.GetRequiredService<CausaliMovimentoViewModel>();

            // Gestione tasti funzione
            this.KeyDown += OnKeyDown;
            this.Focusable = true;
        }

        #region Event Handlers per i pulsanti

        private async void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.NuovaCausaleMovimento();
            CodiceTextBox.Focus();
        }

        private async void Salva_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.SalvaCausaleMovimento();
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            // Gestione cancellazione singola o multipla
            var selectedItems = CausaliMovimentoDataGrid.SelectedItems;
            if (selectedItems != null && selectedItems.Count > 1)
            {
                var result = MessageBox.Show(
                    $"Hai selezionato {selectedItems.Count} causali per la cancellazione.\n\n" +
                    "Verrai chiesto una conferma per ogni causale.\n\nContinuare?",
                    "Cancellazione Multipla", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var list = new System.Collections.Generic.List<CausaleMovimentoItem>();
                    foreach (var obj in selectedItems)
                    {
                        if (obj is CausaleMovimentoItem item)
                            list.Add(item);
                    }

                    ViewModel.CancellaCausaliMultiple(list);
                }
            }
            else
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare questa causale movimento?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _ = ViewModel.CancellaCausaleMovimento();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AnnullaModifiche();
        }

        private async void Cerca_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.CercaCausaliMovimento();
        }

        #endregion

        #region Event Handlers per il DataGrid

        private async void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is CausaleMovimentoItem causale)
            {
                await ViewModel.SelezionaCausaleMovimento(causale.Codice, causale.Descrizione);
            }
        }

        #endregion

        #region Event Handlers per la ricerca contro movimento

        private void CercaControMovimento_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ApriSelezioneControMovimento();
        }

        #endregion

        #region Gestione tasti funzione

        private async void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    e.Handled = true;
                    await ViewModel.NuovaCausaleMovimento();
                    CodiceTextBox.Focus();
                    break;

                case Key.F2:
                    e.Handled = true;
                    await ViewModel.SalvaCausaleMovimento();
                    break;

                case Key.F3:
                    e.Handled = true;
                    await ViewModel.CercaCausaliMovimento();
                    break;

                case Key.F4:
                    e.Handled = true;
                    // Stessa logica del click su Cancella per gestire selezione multipla
                    var selectedItems = CausaliMovimentoDataGrid.SelectedItems;
                    if (selectedItems != null && selectedItems.Count > 1)
                    {
                        var confirm = MessageBox.Show(
                            $"Hai selezionato {selectedItems.Count} causali per la cancellazione.\n\n" +
                            "Verrai chiesto una conferma per ogni causale.\n\nContinuare?",
                            "Cancellazione Multipla", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (confirm == MessageBoxResult.Yes)
                        {
                            var list = new System.Collections.Generic.List<CausaleMovimentoItem>();
                            foreach (var obj in selectedItems)
                            {
                                if (obj is CausaleMovimentoItem item)
                                    list.Add(item);
                            }
                            ViewModel.CancellaCausaliMultiple(list);
                        }
                    }
                    else
                    {
                        var result = MessageBox.Show("Sei sicuro di voler cancellare questa causale movimento?",
                            "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            await ViewModel.CancellaCausaleMovimento();
                        }
                    }
                    break;

                case Key.F8:
                    e.Handled = true;
                    ViewModel.AnnullaModifiche();
                    break;

                case Key.Enter:
                    // Se siamo sui campi di ricerca, esegui la ricerca
                    if (CodiceRicercaTextBox.IsFocused || CausaleRicercaTextBox.IsFocused)
                    {
                        e.Handled = true;
                        await ViewModel.CercaCausaliMovimento();
                    }
                    break;

                case Key.Escape:
                    // Annulla modifiche
                    e.Handled = true;
                    ViewModel.AnnullaModifiche();
                    break;
            }
        }

        #endregion

        #region Loaded Event

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Imposta il focus per la gestione dei tasti
            this.Focus();

            // Carica i dati iniziali se necessario
            if (ViewModel.CausaliMovimento.Count == 0)
            {
                await ViewModel.CercaCausaliMovimento();
            }
        }

        #endregion
    }
}