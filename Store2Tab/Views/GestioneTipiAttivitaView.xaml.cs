using System.Linq;
using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Store2Tab.Views
{
    public partial class GestioneTipiAttivitaView : UserControl
    {
        public GestioneTipiAttivitaView()
        {
            InitializeComponent();
            DataContext = new TipiAttivitaViewModel();

            // Configura il controllo per ricevere il focus e i tasti funzione
            this.Focusable = true;
        }

        // F1 - Nuovo (come pctF1_Click nel VB6)
        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.NuovoTipoAttivita();
            }
        }

        // F2 - Salva (come pctF2_Click nel VB6)
        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.SalvaTipoAttivita();
            }
        }

        // F3 - Cerca (come pctF3_Click nel VB6)
        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.CercaTipiAttivita();
            }
        }

        // F4 - Cancella (come pctF4_Click nel VB6)
        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                // Il controllo di conferma è già gestito nel ViewModel
                viewModel.CancellaTipoAttivita();
            }
        }

        // F5 - Cancellazione multipla
        private void CancellaMultipla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                var elementiSelezionati = TipiAttivitaDataGrid.SelectedItems
                    .Cast<Store2Tab.Data.Models.TipiAttivita>()
                    .ToList();

                if (elementiSelezionati.Count == 0)
                {
                    MessageBox.Show("Selezionare uno o più tipi di attività da cancellare.",
                        "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                viewModel.CancellaTipiAttivitaMultiple(elementiSelezionati);
            }
        }

        // F8 - Annulla (come pctF8_Click nel VB6)
        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        // Gestione selezione DataGrid (simile a flexG_RowColChange nel VB6)
        private void TipiAttivitaDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                // Gestione selezione multipla per cancellazione (come nell'esempio SpecieBotaniche)
                if (sender is DataGrid dataGrid)
                {
                    // Aggiorna la collezione degli elementi selezionati nel ViewModel
                    viewModel.ElementiSelezionati.Clear();
                    foreach (Store2Tab.Data.Models.TipiAttivita item in dataGrid.SelectedItems)
                    {
                        viewModel.ElementiSelezionati.Add(item);
                    }

                    // Se c'è un solo elemento selezionato, lo imposta come TipoAttivitaSelezionato
                    if (dataGrid.SelectedItems.Count == 1 && e.AddedItems.Count > 0)
                    {
                        if (e.AddedItems[0] is Store2Tab.Data.Models.TipiAttivita tipoSelezionato)
                        {
                            viewModel.SelezionaTipoAttivita(tipoSelezionato);
                        }
                    }
                    else if (dataGrid.SelectedItems.Count == 0)
                    {
                        // Se non c'è nulla di selezionato, azzera la selezione
                        viewModel.TipoAttivitaSelezionato = null;
                    }
                }
            }
        }

        // Gestione modifica campo descrizione (simile a txtAnagraficaAttivita_Change nel VB6)
        private void AttivitaDescrizioneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.OnAttivitaChanged();
            }
        }

        // Gestione Enter nei campi di ricerca (come txtIdAnagraficaAttivita_Ric_KeyPress nel VB6)
        private void CodiceRicercaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.CercaTipiAttivita();
                e.Handled = true;
            }
        }

        // Gestione Enter nei campi di ricerca (come txtAnagraficaAttivita_Ric_KeyPress nel VB6)
        private void DescrizioneRicercaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.CercaTipiAttivita();
                e.Handled = true;
            }
        }

        // Gestione tasti funzione globali (come Form_KeyDown nel VB6)
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                switch (e.Key)
                {
                    case Key.Escape: // ESC - Chiudi form
                        // In WPF, gestito dal parent window
                        break;
                    case Key.F1: // F1 - Nuovo
                        viewModel.NuovoTipoAttivita();
                        e.Handled = true;
                        break;
                    case Key.F2: // F2 - Salva
                        if (viewModel.IsInEditMode)
                        {
                            viewModel.SalvaTipoAttivita();
                        }
                        e.Handled = true;
                        break;
                    case Key.F3: // F3 - Cerca
                        viewModel.CercaTipiAttivita();
                        e.Handled = true;
                        break;
                    case Key.F4: // F4 - Cancella
                        if (viewModel.TipoAttivitaSelezionato != null)
                        {
                            viewModel.CancellaTipoAttivita();
                        }
                        e.Handled = true;
                        break;
                    case Key.F5: // F5 - Cancellazione multipla
                        CancellaMultipla_Click(this, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.F8: // F8 - Annulla
                        if (viewModel.IsInEditMode)
                        {
                            viewModel.AnnullaModifiche();
                        }
                        e.Handled = true;
                        break;
                }
            }

            base.OnKeyDown(e);
        }

        // Gestione del caricamento del controllo
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Assicura che il controllo possa ricevere il focus per i tasti funzione
            this.Focusable = true;
            this.Focus();
        }

        // Gestione del focus quando il form viene attivato (simile a Form_Activate nel VB6)
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                // Se non ci sono elementi nella griglia, da il focus al campo descrizione
                // (simile alla logica di Form_Activate nel VB6)
                if (viewModel.TipiAttivita.Count == 0)
                {
                    AttivitaDescrizioneTextBox.Focus();
                }
            }
        }
    }
}