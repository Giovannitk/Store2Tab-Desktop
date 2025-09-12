using Store2Tab.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid delle Numerazioni Passaporto CEE
    /// </summary>
    public class PassaportoCeeNumerazioniItem
    {
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public string Prefisso { get; set; } = string.Empty;

        // Proprietà computed per il DataGrid
        public string SiglaPrefisso => $"{Sigla}/{Prefisso}";
    }

    public partial class GestionePassaportoCeeNumerazioniView : UserControl
    {
        public GestionePassaportoCeeNumerazioniView()
        {
            InitializeComponent();
            DataContext = new PassaportoCeeNumerazioniViewModel();

            // Popola con dati di esempio
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            var numerazioniEsempio = new List<PassaportoCeeNumerazioniItem>
            {
                new PassaportoCeeNumerazioniItem
                {
                    Codice = "1",
                    Descrizione = "Numerazione Standard Rose",
                    Sigla = "RSE",
                    Prefisso = "R"
                },
                new PassaportoCeeNumerazioniItem
                {
                    Codice = "2",
                    Descrizione = "Numerazione Agrumi Certificati",
                    Sigla = "AGR",
                    Prefisso = "A"
                },
                new PassaportoCeeNumerazioniItem
                {
                    Codice = "3",
                    Descrizione = "Numerazione Olivi Biologici",
                    Sigla = "OLV",
                    Prefisso = "O"
                },
                new PassaportoCeeNumerazioniItem
                {
                    Codice = "4",
                    Descrizione = "Numerazione Piante Ornamentali",
                    Sigla = "ORN",
                    Prefisso = "P"
                },
                new PassaportoCeeNumerazioniItem
                {
                    Codice = "5",
                    Descrizione = "Numerazione Frutta Secca",
                    Sigla = "FRT",
                    Prefisso = "F"
                }
            };

            foreach (var numerazione in numerazioniEsempio)
            {
                NumerazioniDataGrid.Items.Add(numerazione);
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassaportoCeeNumerazioniViewModel viewModel)
            {
                viewModel.NuovaNumerazione();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassaportoCeeNumerazioniViewModel viewModel)
            {
                viewModel.SalvaNumerazione();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassaportoCeeNumerazioniViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare la numerazione selezionata?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaNumerazione();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassaportoCeeNumerazioniViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassaportoCeeNumerazioniViewModel viewModel)
            {
                viewModel.CercaNumerazioni();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PassaportoCeeNumerazioniViewModel viewModel)
            {
                if (NumerazioniDataGrid.SelectedItem is PassaportoCeeNumerazioniItem selectedItem)
                {
                    viewModel.SelezionaNumerazione(selectedItem.Codice, selectedItem.Descrizione,
                                                  selectedItem.Sigla, selectedItem.Prefisso);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    DescrizioneTextBox.Text = selectedItem.Descrizione;
                    SiglaTextBox.Text = selectedItem.Sigla;
                    PrefissoTextBox.Text = selectedItem.Prefisso;
                }
            }
        }
    }
}