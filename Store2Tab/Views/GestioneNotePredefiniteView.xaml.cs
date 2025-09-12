using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid delle Note Predefinite
    /// </summary>
    public class NotaPredefinitaItem
    {
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
    }

    public partial class GestioneNotePredefiniteView : UserControl
    {
        private NotaPredefinitaItem _currentItem;

        public GestioneNotePredefiniteView()
        {
            InitializeComponent();
            DataContext = new NotePredefiniteViewModel();
            _currentItem = new NotaPredefinitaItem();

            // Carica alcuni dati di esempio
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Aggiungi alcuni elementi di esempio al DataGrid
            var sampleItems = new[]
            {
                new NotaPredefinitaItem { Codice = "001", Descrizione = "Nota standard per fattura" },
                new NotaPredefinitaItem { Codice = "002", Descrizione = "Condizioni di pagamento" },
                new NotaPredefinitaItem { Codice = "003", Descrizione = "Clausole di garanzia" },
                new NotaPredefinitaItem { Codice = "004", Descrizione = "Istruzioni per il trasporto" },
                new NotaPredefinitaItem { Codice = "005", Descrizione = "Note legali standard" }
            };

            foreach (var item in sampleItems)
            {
                NotePredefiniteDataGrid.Items.Add(item);
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotePredefiniteViewModel viewModel)
            {
                viewModel.NuovaNota();
                ClearFields();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotePredefiniteViewModel viewModel)
            {
                // Valida i dati prima di salvare
                string codice = CodiceTextBox.Text.Trim();
                string descrizione = DescrizioneNotaTextBox.Text.Trim();

                if (ValidateFields(codice, descrizione))
                {
                    viewModel.SalvaNota();

                    // Aggiorna i dati correnti
                    _currentItem.Codice = codice;
                    _currentItem.Descrizione = descrizione;

                    // Se è un nuovo elemento, aggiungilo al DataGrid
                    if (NotePredefiniteDataGrid.SelectedItem == null)
                    {
                        var newItem = new NotaPredefinitaItem
                        {
                            Codice = codice,
                            Descrizione = descrizione
                        };
                        NotePredefiniteDataGrid.Items.Add(newItem);
                        NotePredefiniteDataGrid.SelectedItem = newItem;
                    }
                    else
                    {
                        // Aggiorna l'elemento esistente
                        if (NotePredefiniteDataGrid.SelectedItem is NotaPredefinitaItem selectedItem)
                        {
                            selectedItem.Codice = codice;
                            selectedItem.Descrizione = descrizione;
                            NotePredefiniteDataGrid.Items.Refresh();
                        }
                    }
                }
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotePredefiniteViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare la nota predefinita selezionata?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaNota();

                    // Rimuovi l'elemento dal DataGrid
                    if (NotePredefiniteDataGrid.SelectedItem is NotaPredefinitaItem selectedItem)
                    {
                        NotePredefiniteDataGrid.Items.Remove(selectedItem);
                        ClearFields();
                    }
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotePredefiniteViewModel viewModel)
            {
                viewModel.AnnullaModifiche();

                // Ripristina i valori originali se c'è un elemento selezionato
                if (NotePredefiniteDataGrid.SelectedItem is NotaPredefinitaItem selectedItem)
                {
                    CodiceTextBox.Text = selectedItem.Codice;
                    DescrizioneNotaTextBox.Text = selectedItem.Descrizione;
                }
                else
                {
                    ClearFields();
                }
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NotePredefiniteViewModel viewModel)
            {
                viewModel.CercaNote();

                // Implementa la logica di ricerca
                string codiceRicerca = CodiceRicercaTextBox.Text.Trim();
                string descrizioneRicerca = DescrizioneRicercaTextBox.Text.Trim();

                // Filtra gli elementi del DataGrid
                FilterDataGrid(codiceRicerca, descrizioneRicerca);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is NotePredefiniteViewModel viewModel)
            {
                if (NotePredefiniteDataGrid.SelectedItem is NotaPredefinitaItem selectedItem)
                {
                    viewModel.SelezionaNota(selectedItem.Codice, selectedItem.Descrizione);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    DescrizioneNotaTextBox.Text = selectedItem.Descrizione;

                    _currentItem = selectedItem;
                }
            }
        }

        private bool ValidateFields(string codice, string descrizione)
        {
            if (string.IsNullOrWhiteSpace(codice))
            {
                MessageBox.Show("Il campo Codice è obbligatorio.", "Validazione",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                CodiceTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(descrizione))
            {
                MessageBox.Show("Il campo Descrizione nota è obbligatorio.", "Validazione",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                DescrizioneNotaTextBox.Focus();
                return false;
            }

            // Verifica che il codice non esista già (escludendo l'elemento corrente)
            foreach (NotaPredefinitaItem item in NotePredefiniteDataGrid.Items)
            {
                if (item != NotePredefiniteDataGrid.SelectedItem &&
                    item.Codice.Equals(codice, System.StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show($"Il codice '{codice}' è già utilizzato da un'altra nota.",
                                  "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CodiceTextBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private void ClearFields()
        {
            CodiceTextBox.Text = "";
            DescrizioneNotaTextBox.Text = "";
            _currentItem = new NotaPredefinitaItem();
        }

        private void FilterDataGrid(string codice, string descrizione)
        {
            // Implementazione semplificata del filtro
            // In una implementazione reale, dovresti utilizzare un CollectionView
            NotePredefiniteDataGrid.Items.Clear();
            LoadSampleData();

            if (!string.IsNullOrWhiteSpace(codice) || !string.IsNullOrWhiteSpace(descrizione))
            {
                var itemsToRemove = new System.Collections.Generic.List<NotaPredefinitaItem>();

                foreach (NotaPredefinitaItem item in NotePredefiniteDataGrid.Items)
                {
                    bool matches = true;

                    if (!string.IsNullOrWhiteSpace(codice))
                    {
                        matches = matches && item.Codice.ToLower().Contains(codice.ToLower());
                    }

                    if (!string.IsNullOrWhiteSpace(descrizione))
                    {
                        matches = matches && item.Descrizione.ToLower().Contains(descrizione.ToLower());
                    }

                    if (!matches)
                    {
                        itemsToRemove.Add(item);
                    }
                }

                foreach (var item in itemsToRemove)
                {
                    NotePredefiniteDataGrid.Items.Remove(item);
                }
            }
        }

        public void LoadNotaPredefinita(NotaPredefinitaItem nota)
        {
            if (nota == null) return;

            CodiceTextBox.Text = nota.Codice;
            DescrizioneNotaTextBox.Text = nota.Descrizione;
            _currentItem = nota;
        }
    }
}