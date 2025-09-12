using Store2Tab.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid delle Specie Botaniche
    /// </summary>
    public class SpecieBotanicheItem
    {
        public string Codice { get; set; } = string.Empty;
        public string SpecieBotanica { get; set; } = string.Empty;
    }

    public partial class GestioneSpecieBotanicheView : UserControl
    {
        public GestioneSpecieBotanicheView()
        {
            InitializeComponent();
            DataContext = new SpecieBotanicheViewModel();

            // Popola con dati di esempio
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            var specieEsempio = new List<SpecieBotanicheItem>
            {
                new SpecieBotanicheItem
                {
                    Codice = "1",
                    SpecieBotanica = "Rosa gallica"
                },
                new SpecieBotanicheItem
                {
                    Codice = "2",
                    SpecieBotanica = "Citrus limon"
                },
                new SpecieBotanicheItem
                {
                    Codice = "3",
                    SpecieBotanica = "Olea europaea"
                },
                new SpecieBotanicheItem
                {
                    Codice = "4",
                    SpecieBotanica = "Lavandula angustifolia"
                },
                new SpecieBotanicheItem
                {
                    Codice = "5",
                    SpecieBotanica = "Rosmarinus officinalis"
                },
                new SpecieBotanicheItem
                {
                    Codice = "6",
                    SpecieBotanica = "Prunus persica"
                },
                new SpecieBotanicheItem
                {
                    Codice = "7",
                    SpecieBotanica = "Vitis vinifera"
                }
            };

            foreach (var specie in specieEsempio)
            {
                SpecieBotanicheDataGrid.Items.Add(specie);
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SpecieBotanicheViewModel viewModel)
            {
                viewModel.NuovaSpecie();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SpecieBotanicheViewModel viewModel)
            {
                viewModel.SalvaSpecie();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SpecieBotanicheViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare la specie botanica selezionata?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaSpecie();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SpecieBotanicheViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is SpecieBotanicheViewModel viewModel)
            {
                viewModel.CercaSpecie();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is SpecieBotanicheViewModel viewModel)
            {
                if (SpecieBotanicheDataGrid.SelectedItem is SpecieBotanicheItem selectedItem)
                {
                    viewModel.SelezionaSpecie(selectedItem.Codice, selectedItem.SpecieBotanica);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    SpecieBotanicaTextBox.Text = selectedItem.SpecieBotanica;
                }
            }
        }
    }
}