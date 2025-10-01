using Store2Tab.ViewModels;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Store2Tab.Views
{
    public partial class GestioneProtocolliView : UserControl
    {
        private ProtocolliViewModel ViewModel => (ProtocolliViewModel)DataContext;

        public GestioneProtocolliView()
        {
            InitializeComponent();
            DataContext = new ProtocolliViewModel();

            // Gestione eventi tastiera (F1-F8)
            this.KeyDown += GestioneProtocolliView_KeyDown;
            this.Focusable = true;
            this.Loaded += (s, e) => this.Focus();

            // Gestione selezione multipla
            ProtocolliDataGrid.SelectionChanged += ProtocolliDataGrid_SelectionChanged;
        }

        /// <summary>
        /// Gestione tasti funzione come nel VB6
        /// </summary>
        private void GestioneProtocolliView_KeyDown(object sender, KeyEventArgs e)
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
        /// Aggiorna la collezione ElementiSelezionati quando cambia la selezione
        /// </summary>
        private void ProtocolliDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel != null && sender is DataGrid dataGrid)
            {
                // Aggiorna la collezione degli elementi selezionati nel ViewModel
                ViewModel.ElementiSelezionati.Clear();
                foreach (Store2Tab.Data.Models.Protocollo item in dataGrid.SelectedItems)
                {
                    ViewModel.ElementiSelezionati.Add(item);
                }

                // Se c'è un solo elemento selezionato, lo imposta come ProtocolloSelezionato
                if (dataGrid.SelectedItems.Count == 1 && e.AddedItems.Count > 0)
                {
                    if (e.AddedItems[0] is Store2Tab.Data.Models.Protocollo protocolloSelezionato)
                    {
                        ViewModel.SelezionaProtocollo(protocolloSelezionato);
                    }
                }
                else if (dataGrid.SelectedItems.Count == 0)
                {
                    // Se non c'è nulla di selezionato, azzera la selezione
                    ViewModel.ProtocolloSelezionato = null;
                }
            }
        }

        /// <summary>
        /// F1 - Nuovo
        /// </summary>
        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NuovoProtocollo();
        }

        /// <summary>
        /// F2 - Salva
        /// </summary>
        private async void Salva_Click(object sender, RoutedEventArgs e)
        {
            bool risultato = await ViewModel.SalvaProtocolloAsync();
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
            ViewModel.CercaProtocollo();
        }

        /// <summary>
        /// F4 - Cancella
        /// </summary>
        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliViewModel viewModel)
            {
                viewModel.EliminaProtocollo();
            }
        }

        /// <summary>
        /// F5 - Cancellazione multipla
        /// </summary>
        private void CancellaMultipla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliViewModel viewModel)
            {
                var elementiSelezionati = ProtocolliDataGrid.SelectedItems
                    .Cast<Store2Tab.Data.Models.Protocollo>()
                    .ToList();

                if (elementiSelezionati.Count == 0)
                {
                    MessageBox.Show("Selezionare uno o più protocolli da cancellare.",
                        "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                viewModel.EliminaProtocolliMultipli(elementiSelezionati);
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
        private void DescrizioneProtocollo_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.OnProtocolloModificato();
        }
    }
}