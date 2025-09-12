using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    public partial class GestioneTipiAttivitaView : UserControl
    {
        public GestioneTipiAttivitaView()
        {
            InitializeComponent();
            DataContext = new TipiAttivitaViewModel();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.NuovoTipoAttivita();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.SalvaTipoAttivita();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare il tipo di attività selezionato?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaTipoAttivita();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                viewModel.CercaTipiAttivita();
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is TipiAttivitaViewModel viewModel)
            {
                if (TipiAttivitaListBox.SelectedItem is ListBoxItem selectedItem)
                {
                    string codice = selectedItem.Tag?.ToString() ?? "";
                    string descrizione = selectedItem.Content?.ToString() ?? "";

                    viewModel.SelezionaTipoAttivita(codice, descrizione);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = codice;
                    AttivitaDescrizioneTextBox.Text = descrizione;
                }
            }
        }
    }
}