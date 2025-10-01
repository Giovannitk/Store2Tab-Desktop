using Store2Tab.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Store2Tab.Views
{
    public partial class GestioneNotePredefiniteView : UserControl
    {
        private NotePredefiniteViewModel ViewModel => (NotePredefiniteViewModel)DataContext;

        public GestioneNotePredefiniteView()
        {
            InitializeComponent();
            DataContext = new NotePredefiniteViewModel();

            // Gestione eventi tastiera (F1-F8)
            this.KeyDown += GestioneNotePredefiniteView_KeyDown;
            this.Focusable = true;
            this.Loaded += (s, e) => this.Focus();

            // CORREZIONE: Aggiungi handler per la selezione multipla
            NoteDocumentoDataGrid.SelectionChanged += NoteDocumentoDataGrid_SelectionChanged;
        }

        /// <summary>
        /// NUOVO: Aggiorna la collezione ElementiSelezionati quando cambia la selezione
        /// </summary>
        private void NoteDocumentoDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel != null && sender is DataGrid dataGrid)
            {
                // Aggiorna la collezione degli elementi selezionati nel ViewModel
                ViewModel.ElementiSelezionati.Clear();
                foreach (Store2Tab.Data.Models.NotaDocumento item in dataGrid.SelectedItems)
                {
                    ViewModel.ElementiSelezionati.Add(item);
                }

                // Se c'è un solo elemento selezionato, lo imposta come NotaSelezionata
                if (dataGrid.SelectedItems.Count == 1 && e.AddedItems.Count > 0)
                {
                    if (e.AddedItems[0] is Store2Tab.Data.Models.NotaDocumento notaSelezionata)
                    {
                        ViewModel.SelezionaNotaDocumento(notaSelezionata);
                    }
                }
                else if (dataGrid.SelectedItems.Count == 0)
                {
                    // Se non c'è nulla di selezionato, azzera la selezione
                    ViewModel.NotaSelezionata = null;
                }
            }
        }

        /// <summary>
        /// Gestione tasti funzione come nel VB6
        /// </summary>
        private void GestioneNotePredefiniteView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1: // Nuovo
                    Nuovo_Click(sender, e);
                    e.Handled = true;
                    break;
                case Key.F2: // Salva
                    if (ViewModel.IsInEditMode)
                    {
                        Salva_Click(sender, e);
                        e.Handled = true;
                    }
                    break;
                case Key.F3: // Cerca
                    Cerca_Click(sender, e);
                    e.Handled = true;
                    break;
                case Key.F4: // Cancella
                    Cancella_Click(sender, e);
                    e.Handled = true;
                    break;
                case Key.F5: // Cancella multipla
                    CancellaMultipla_Click(sender, e);
                    e.Handled = true;
                    break;
                case Key.F8: // Annulla
                    if (ViewModel.IsInEditMode)
                    {
                        Annulla_Click(sender, e);
                        e.Handled = true;
                    }
                    break;
                case Key.Escape: // Chiudi form
                    Window.GetWindow(this)?.Close();
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// F1 - Nuovo
        /// </summary>
        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NuovaNotaDocumento();
        }

        /// <summary>
        /// F2 - Salva
        /// </summary>
        private async void Salva_Click(object sender, RoutedEventArgs e)
        {
            bool risultato = await ViewModel.SalvaNotaDocumentoAsync();
            if (!risultato)
            {
                MessageBox.Show("Errore durante il salvataggio.");
            }
        }

        /// <summary>
        /// F3 - Cerca
        /// </summary>
        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CercaNoteDocumento();
        }

        /// <summary>
        /// F4 - Cancella
        /// </summary>
        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotePredefiniteViewModel viewModel)
            {
                viewModel.EliminaNotaDocumento();
            }
        }

        /// <summary>
        /// F5 - Cancellazione multipla
        /// </summary>
        private void CancellaMultipla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotePredefiniteViewModel viewModel)
            {
                var elementiSelezionati = NoteDocumentoDataGrid.SelectedItems
                    .Cast<Store2Tab.Data.Models.NotaDocumento>()
                    .ToList();

                if (elementiSelezionati.Count == 0)
                {
                    MessageBox.Show("Selezionare uno o più note di documento da cancellare.",
                        "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                viewModel.EliminaNoteDocumentoMultiple(elementiSelezionati);
            }
        }

        /// <summary>
        /// F8 - Annulla modifiche
        /// </summary>
        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AnnullaModifiche();
        }

        /// <summary>
        /// Evento di modifica del testo - attiva la modalità edit
        /// </summary>
        private void DescrizioneNota_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.OnNotaModificata();
        }
    }
}