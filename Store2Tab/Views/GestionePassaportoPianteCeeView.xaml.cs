using Store2Tab.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid dei Passaporti Piante CEE
    /// </summary>
    public class PassaportoPianteCeeItem
    {
        public string Codice { get; set; } = string.Empty;
        public string ValidoDal { get; set; } = string.Empty;
        public string ValidoAl { get; set; } = string.Empty;
        public string Numerazione { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public string DescrizioneStampare { get; set; } = string.Empty;
        public string ServizioFitosanitario { get; set; } = string.Empty;
        public string CodiceProduttore { get; set; } = string.Empty;
        public string CodiceProduttoreOriginario { get; set; } = string.Empty;
        public bool PassaportoPianteCee { get; set; } = false;
        public bool CategoriaC { get; set; } = false;
        public bool DocumentoCommercializzazione { get; set; } = false;
        public bool StampaTesserino { get; set; } = false;
        public string PaeseDiOrigine { get; set; } = string.Empty;
    }

    public partial class GestionePassaportoPianteCeeView : UserControl
    {
        public GestionePassaportoPianteCeeView()
        {
            InitializeComponent();
            DataContext = new PassaportoPianteCeeViewModel();

            // Popola con dati di esempio
            PopulateDataGrid();
            PopulateComboBoxes();
        }

        private void PopulateDataGrid()
        {
            var passaportiEsempio = new List<PassaportoPianteCeeItem>
            {
                new PassaportoPianteCeeItem
                {
                    Codice = "1",
                    ValidoDal = "01/01/2024",
                    ValidoAl = "31/12/2024",
                    Numerazione = "NUM001",
                    Descrizione = "Passaporto Rose",
                    DescrizioneStampare = "Passaporto per Rose da stampa",
                    ServizioFitosanitario = "Servizio Fitosanitario Regionale",
                    CodiceProduttore = "PROD001",
                    CodiceProduttoreOriginario = "ORIG001",
                    PassaportoPianteCee = true,
                    CategoriaC = false,
                    DocumentoCommercializzazione = true,
                    StampaTesserino = true,
                    PaeseDiOrigine = "Italia"
                },
                new PassaportoPianteCeeItem
                {
                    Codice = "2",
                    ValidoDal = "01/06/2024",
                    ValidoAl = "31/05/2025",
                    Numerazione = "NUM002",
                    Descrizione = "Passaporto Agrumi",
                    DescrizioneStampare = "Passaporto per Agrumi certificati",
                    ServizioFitosanitario = "Servizio Fitosanitario Provinciale",
                    CodiceProduttore = "PROD002",
                    CodiceProduttoreOriginario = "ORIG002",
                    PassaportoPianteCee = true,
                    CategoriaC = true,
                    DocumentoCommercializzazione = false,
                    StampaTesserino = false,
                    PaeseDiOrigine = "Spagna"
                },
                new PassaportoPianteCeeItem
                {
                    Codice = "3",
                    ValidoDal = "01/03/2024",
                    ValidoAl = "28/02/2025",
                    Numerazione = "NUM003",
                    Descrizione = "Passaporto Olivi",
                    DescrizioneStampare = "Passaporto per Oliveti biologici",
                    ServizioFitosanitario = "Servizio Nazionale Fitosanitario",
                    CodiceProduttore = "PROD003",
                    CodiceProduttoreOriginario = "ORIG003",
                    PassaportoPianteCee = false,
                    CategoriaC = false,
                    DocumentoCommercializzazione = true,
                    StampaTesserino = true,
                    PaeseDiOrigine = "Francia"
                }
            };

            foreach (var passaporto in passaportiEsempio)
            {
                PassaportiDataGrid.Items.Add(passaporto);
            }
        }

        private void PopulateComboBoxes()
        {
            // Popola ComboBox Numerazione
            NumerazioneComboBox.Items.Clear();
            NumerazioneComboBox.Items.Add("NUM001 - Numerazione Standard");
            NumerazioneComboBox.Items.Add("NUM002 - Numerazione Speciale");
            NumerazioneComboBox.Items.Add("NUM003 - Numerazione Biologico");

            // Popola ComboBox CrePassaporto (già popolato in XAML)
            CreaPassaportoComboBox.SelectedIndex = 0;
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassaportoPianteCeeViewModel viewModel)
            {
                viewModel.NuovoPassaporto();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassaportoPianteCeeViewModel viewModel)
            {
                viewModel.SalvaPassaporto();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassaportoPianteCeeViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare il passaporto selezionato?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaPassaporto();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassaportoPianteCeeViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassaportoPianteCeeViewModel viewModel)
            {
                viewModel.CercaPassaporti();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PassaportoPianteCeeViewModel viewModel)
            {
                if (PassaportiDataGrid.SelectedItem is PassaportoPianteCeeItem selectedItem)
                {
                    viewModel.SelezionaPassaporto(selectedItem.Codice, selectedItem.ValidoDal, selectedItem.ValidoAl,
                                                 selectedItem.Numerazione, selectedItem.Descrizione, selectedItem.DescrizioneStampare,
                                                 selectedItem.ServizioFitosanitario, selectedItem.CodiceProduttore,
                                                 selectedItem.CodiceProduttoreOriginario, selectedItem.PassaportoPianteCee,
                                                 selectedItem.CategoriaC, selectedItem.DocumentoCommercializzazione,
                                                 selectedItem.StampaTesserino, selectedItem.PaeseDiOrigine);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    ValidoDalTextBox.Text = selectedItem.ValidoDal;
                    ValidoAlTextBox.Text = selectedItem.ValidoAl;

                    // Seleziona l'item corretto nella ComboBox
                    foreach (var item in NumerazioneComboBox.Items)
                    {
                        if (item.ToString().StartsWith(selectedItem.Numerazione))
                        {
                            NumerazioneComboBox.SelectedItem = item;
                            break;
                        }
                    }

                    PaeseOrigineTextBox.Text = selectedItem.PaeseDiOrigine;
                    DescrizioneTextBox.Text = selectedItem.Descrizione;
                    DescrizioneStampareTextBox.Text = selectedItem.DescrizioneStampare;
                    ServizioFitosanitarioTextBox.Text = selectedItem.ServizioFitosanitario;
                    CodiceProduttoreTextBox.Text = selectedItem.CodiceProduttore;
                    CodiceProduttoreOriginarioTextBox.Text = selectedItem.CodiceProduttoreOriginario;
                    PassaportoPianteCeeCheckBox.IsChecked = selectedItem.PassaportoPianteCee;
                    CategoriaCheckBox.IsChecked = selectedItem.CategoriaC;
                    DocumentoCommercializzazioneCheckBox.IsChecked = selectedItem.DocumentoCommercializzazione;
                    StampaTesserinoCheckBox.IsChecked = selectedItem.StampaTesserino;
                }
            }
        }
    }
}