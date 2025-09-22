using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Store2Tab.Data.Models;
using Store2Tab.ViewModels;

namespace Store2Tab.Views
{
    public partial class GestionePassaportoCeeNumerazioniView : UserControl
    {
        private PassaportoCeeNumerazioniViewModel ViewModel => (PassaportoCeeNumerazioniViewModel)DataContext;

        public GestionePassaportoCeeNumerazioniView()
        {
            InitializeComponent();
            DataContext = new PassaportoCeeNumerazioniViewModel();

            // Associa i tasti funzione
            KeyDown += GestionePassaportoCeeNumerazioniView_KeyDown;
            Focusable = true;
        }

        private void GestionePassaportoCeeNumerazioniView_KeyDown(object sender, KeyEventArgs e)
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
                case Key.Escape:
                    // Chiudi finestra o deseleziona
                    NumerazioniDataGrid.UnselectAll();
                    ViewModel.NumerazioneSelezionata = null;
                    break;
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NuovaNumerazione();
            DescrizioneTextBox.Focus();
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SalvaNumerazione();
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CancellaNumerazione();
        }

        // Nuovo metodo per la cancellazione multipla
        private void CancellaMultipla_Click(object sender, RoutedEventArgs e)
        {
            var elementiSelezionati = NumerazioniDataGrid.SelectedItems
                .Cast<PassPianteCeeNumerazione>()
                .ToList();

            if (elementiSelezionati.Count == 0)
            {
                MessageBox.Show("Selezionare una o più numerazioni da cancellare.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ViewModel.CancellaNumerazioniMultiple(elementiSelezionati);
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CercaNumerazioni();
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AnnullaModifiche();
        }

        // Gestisce la selezione nella DataGrid
        private void NumerazioniDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                // Aggiorna la collezione degli elementi selezionati nel ViewModel
                ViewModel.ElementiSelezionati.Clear();
                foreach (PassPianteCeeNumerazione item in dataGrid.SelectedItems)
                {
                    ViewModel.ElementiSelezionati.Add(item);
                }

                // Se c'è un solo elemento selezionato, lo imposta come NumerazioneSelezionata
                // questo mantiene la compatibilità con la visualizzazione dei dettagli
                if (dataGrid.SelectedItems.Count == 1)
                {
                    var selected = dataGrid.SelectedItems[0] as PassPianteCeeNumerazione;
                    if (selected != ViewModel.NumerazioneSelezionata)
                    {
                        ViewModel.NumerazioneSelezionata = selected;
                    }
                }
                else if (dataGrid.SelectedItems.Count == 0)
                {
                    // Se non c'è nulla di selezionato, azzera la selezione
                    ViewModel.NumerazioneSelezionata = null;
                }
                // Se ci sono più elementi selezionati, mantiene l'ultimo NumerazioneSelezionata
                // ma aggiorna comunque ElementiSelezionati per la cancellazione multipla
            }
        }

        // Gestori per i filtri di ricerca - ricerca automatica
        private void CodiceRicercaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Opzionale: ricerca automatica mentre si digita
            // ViewModel.CercaNumerazioni();
        }

        private void DescrizioneRicercaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Opzionale: ricerca automatica mentre si digita  
            // ViewModel.CercaNumerazioni();
        }

        // Gestori per Enter nei campi di ricerca
        private void CodiceRicercaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.CercaNumerazioni();
            }
        }

        private void DescrizioneRicercaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.CercaNumerazioni();
            }
        }

        // Metodi opzionali per selezione multipla
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            NumerazioniDataGrid.SelectAll();
        }

        private void UnselectAll_Click(object sender, RoutedEventArgs e)
        {
            NumerazioniDataGrid.UnselectAll();
        }

        // Override per gestire la navigazione con le frecce nel DataGrid
        private void NumerazioniDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Permette la navigazione con le frecce anche quando è selezionato un singolo elemento
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.PageUp || e.Key == Key.PageDown)
            {
                // Lascia che il DataGrid gestisca la navigazione
                return;
            }

            // Per altri tasti, propaga l'evento alla finestra principale per i tasti funzione
            if (e.Key >= Key.F1 && e.Key <= Key.F12)
            {
                GestionePassaportoCeeNumerazioniView_KeyDown(sender, e);
            }
        }

        // Metodo per il refresh completo dei dati
        public async void RefreshData()
        {
            await ViewModel.CaricaAsync();
        }

        // Evento per gestire il caricamento della View
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus automatico sulla ricerca per descrizione quando la view si carica
            if (NumerazioniDataGrid.Items.Count == 0)
            {
                DescrizioneTextBox.Focus();
            }
        }
    }
}