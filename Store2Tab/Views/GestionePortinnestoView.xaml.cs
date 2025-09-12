using Store2Tab.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid dei Portinnesti
    /// </summary>
    public class PortinnestoItem
    {
        public string Codice { get; set; } = string.Empty;
        public string SpecieBotanica { get; set; } = string.Empty;
        public string Portinnesto { get; set; } = string.Empty;
    }

    public partial class GestionePortinnestoView : UserControl
    {
        public GestionePortinnestoView()
        {
            InitializeComponent();
            DataContext = new PortinnestoViewModel();

            // Popola ComboBox e DataGrid
            PopulateSpecieBotanicheComboBox();
            PopulateDataGrid();
        }

        private void PopulateSpecieBotanicheComboBox()
        {
            var specieBotaniche = new List<string>
            {
                "Malus domestica",
                "Pyrus communis",
                "Prunus persica",
                "Prunus armeniaca",
                "Prunus avium",
                "Prunus cerasus",
                "Citrus limon",
                "Citrus sinensis",
                "Vitis vinifera",
                "Olea europaea"
            };

            foreach (var specie in specieBotaniche)
            {
                SpecieBotanicaComboBox.Items.Add(specie);
            }
        }

        private void PopulateDataGrid()
        {
            var portinnestiEsempio = new List<PortinnestoItem>
            {
                new PortinnestoItem
                {
                    Codice = "1",
                    SpecieBotanica = "Malus domestica",
                    Portinnesto = "M9"
                },
                new PortinnestoItem
                {
                    Codice = "2",
                    SpecieBotanica = "Malus domestica",
                    Portinnesto = "MM106"
                },
                new PortinnestoItem
                {
                    Codice = "3",
                    SpecieBotanica = "Pyrus communis",
                    Portinnesto = "Cotogno Adams"
                },
                new PortinnestoItem
                {
                    Codice = "4",
                    SpecieBotanica = "Prunus persica",
                    Portinnesto = "GF 677"
                },
                new PortinnestoItem
                {
                    Codice = "5",
                    SpecieBotanica = "Prunus avium",
                    Portinnesto = "Gisela 5"
                },
                new PortinnestoItem
                {
                    Codice = "6",
                    SpecieBotanica = "Vitis vinifera",
                    Portinnesto = "SO4"
                },
                new PortinnestoItem
                {
                    Codice = "7",
                    SpecieBotanica = "Vitis vinifera",
                    Portinnesto = "1103 Paulsen"
                },
                new PortinnestoItem
                {
                    Codice = "8",
                    SpecieBotanica = "Citrus limon",
                    Portinnesto = "Citrange Carrizo"
                },
                new PortinnestoItem
                {
                    Codice = "9",
                    SpecieBotanica = "Olea europaea",
                    Portinnesto = "Frantoio selvatico"
                }
            };

            foreach (var portinnesto in portinnestiEsempio)
            {
                PortinnestoDataGrid.Items.Add(portinnesto);
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PortinnestoViewModel viewModel)
            {
                viewModel.NuovoPortinnesto();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PortinnestoViewModel viewModel)
            {
                viewModel.SalvaPortinnesto();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PortinnestoViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare il portinnesto selezionato?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaPortinnesto();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PortinnestoViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PortinnestoViewModel viewModel)
            {
                viewModel.CercaPortinnesti();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PortinnestoViewModel viewModel)
            {
                if (PortinnestoDataGrid.SelectedItem is PortinnestoItem selectedItem)
                {
                    viewModel.SelezionaPortinnesto(selectedItem.Codice, selectedItem.SpecieBotanica,
                                                  selectedItem.Portinnesto);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    SpecieBotanicaComboBox.Text = selectedItem.SpecieBotanica;
                    PortinnestoTextBox.Text = selectedItem.Portinnesto;
                }
            }
        }
    }
}