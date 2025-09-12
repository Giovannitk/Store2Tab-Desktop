using Store2Tab.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid delle Varietà
    /// </summary>
    public class VarietaItem
    {
        public string Codice { get; set; } = string.Empty;
        public string SpecieBotanica { get; set; } = string.Empty;
        public string Varieta { get; set; } = string.Empty;
    }

    public partial class GestioneVarietaView : UserControl
    {
        public GestioneVarietaView()
        {
            InitializeComponent();
            DataContext = new VarietaViewModel();

            // Popola ComboBox e DataGrid
            PopulateSpecieBotanicheComboBox();
            PopulateDataGrid();
        }

        private void PopulateSpecieBotanicheComboBox()
        {
            var specieBotaniche = new List<string>
            {
                "Rosa gallica",
                "Citrus limon",
                "Olea europaea",
                "Lavandula angustifolia",
                "Rosmarinus officinalis",
                "Prunus persica",
                "Vitis vinifera",
                "Malus domestica",
                "Pyrus communis",
                "Fragaria × ananassa"
            };

            foreach (var specie in specieBotaniche)
            {
                SpecieBotanicaComboBox.Items.Add(specie);
            }
        }

        private void PopulateDataGrid()
        {
            var varietaEsempio = new List<VarietaItem>
            {
                new VarietaItem
                {
                    Codice = "1",
                    SpecieBotanica = "Rosa gallica",
                    Varieta = "Charles de Mills"
                },
                new VarietaItem
                {
                    Codice = "2",
                    SpecieBotanica = "Citrus limon",
                    Varieta = "Eureka"
                },
                new VarietaItem
                {
                    Codice = "3",
                    SpecieBotanica = "Olea europaea",
                    Varieta = "Frantoio"
                },
                new VarietaItem
                {
                    Codice = "4",
                    SpecieBotanica = "Lavandula angustifolia",
                    Varieta = "Hidcote Blue"
                },
                new VarietaItem
                {
                    Codice = "5",
                    SpecieBotanica = "Prunus persica",
                    Varieta = "Red Haven"
                },
                new VarietaItem
                {
                    Codice = "6",
                    SpecieBotanica = "Vitis vinifera",
                    Varieta = "Sangiovese"
                },
                new VarietaItem
                {
                    Codice = "7",
                    SpecieBotanica = "Malus domestica",
                    Varieta = "Golden Delicious"
                },
                new VarietaItem
                {
                    Codice = "8",
                    SpecieBotanica = "Pyrus communis",
                    Varieta = "Williams"
                },
                new VarietaItem
                {
                    Codice = "9",
                    SpecieBotanica = "Fragaria × ananassa",
                    Varieta = "Albion"
                }
            };

            foreach (var varieta in varietaEsempio)
            {
                VarietaDataGrid.Items.Add(varieta);
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VarietaViewModel viewModel)
            {
                viewModel.NuovaVarieta();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VarietaViewModel viewModel)
            {
                viewModel.SalvaVarieta();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VarietaViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare la varietà selezionata?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaVarieta();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VarietaViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VarietaViewModel viewModel)
            {
                viewModel.CercaVarieta();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is VarietaViewModel viewModel)
            {
                if (VarietaDataGrid.SelectedItem is VarietaItem selectedItem)
                {
                    viewModel.SelezionaVarieta(selectedItem.Codice, selectedItem.SpecieBotanica,
                                              selectedItem.Varieta);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    SpecieBotanicaComboBox.Text = selectedItem.SpecieBotanica;
                    VarietaTextBox.Text = selectedItem.Varieta;
                }
            }
        }
    }
}