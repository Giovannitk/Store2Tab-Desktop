using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Store2Tab.Data.Models;
using Store2Tab.ViewModels;

namespace Store2Tab.Views
{
    public partial class GestioneMezziPagamentoView : UserControl
    {
        private MezziPagamentoViewModel ViewModel => (MezziPagamentoViewModel)DataContext;

        public GestioneMezziPagamentoView()
        {
            InitializeComponent();
            DataContext = new MezziPagamentoViewModel();

            // Associa i tasti funzione
            KeyDown += GestioneMezziPagamentoView_KeyDown;
            Focusable = true;
        }

        private void GestioneMezziPagamentoView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    Nuovo_Click(sender, e);
                    break;
                case Key.F2:
                    Salva_Click(sender, e);
                    break;
                case Key.F3:
                    Cerca_Click(sender, e);
                    break;
                case Key.F4:
                    Cancella_Click(sender, e);
                    break;
                case Key.F5:
                    CancellaMultipla_Click(sender, e);
                    break;
                case Key.F8:
                    Annulla_Click(sender, e);
                    break;
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NuovoMezzoPagamento();
            DescrizioneMezzoPagamentoTextBox.Focus();
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SalvaMezzoPagamento();
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CancellaMezzoPagamento();
        }

        // Nuovo metodo per la cancellazione multipla
        private void CancellaMultipla_Click(object sender, RoutedEventArgs e)
        {
            var elementiSelezionati = MezziPagamentoDataGrid.SelectedItems
                .Cast<PagamentoMezzo>()
                .ToList();

            if (elementiSelezionati.Count == 0)
            {
                MessageBox.Show("Selezionare uno o più mezzi di pagamento da cancellare.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ViewModel.CancellaMezziPagamentoMultipli(elementiSelezionati);
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CercaMezziPagamento();
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AnnullaModifiche();
        }

        // Gestisce la selezione multipla nella DataGrid
        private void MezziPagamentoDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                // Aggiorno la collezione degli elementi selezionati nel ViewModel
                ViewModel.ElementiSelezionati.Clear();
                foreach (PagamentoMezzo item in dataGrid.SelectedItems)
                {
                    ViewModel.ElementiSelezionati.Add(item);
                }

                // Se c'è un solo elemento selezionato, lo imposta come MezzoSelezionato
                // questo mantiene la compatibilità con la visualizzazione dei dettagli
                if (dataGrid.SelectedItems.Count == 1)
                {
                    var selected = dataGrid.SelectedItems[0] as PagamentoMezzo;
                    if (selected != ViewModel.MezzoSelezionato)
                    {
                        ViewModel.MezzoSelezionato = selected;
                    }
                }
                else if (dataGrid.SelectedItems.Count == 0)
                {
                    // Se non c'è nulla di selezionato, azzera la selezione
                    ViewModel.MezzoSelezionato = null;
                }
                // Se ci sono più elementi selezionati, mantiene l'ultimo MezzoSelezionato
                // ma aggiorna comunque ElementiSelezionati per la cancellazione multipla
            }
        }

        // Opzionale: metodo per selezionare tutto
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            MezziPagamentoDataGrid.SelectAll();
        }

        // Opzionale: metodo per deselezionare tutto
        private void UnselectAll_Click(object sender, RoutedEventArgs e)
        {
            MezziPagamentoDataGrid.UnselectAll();
        }
    }
}