using Store2Tab.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid dei Componenti
    /// </summary>
    public class ComponenteItem
    {
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
    }

    public partial class GestioneComponentiView : UserControl
    {
        public GestioneComponentiView()
        {
            InitializeComponent();
            DataContext = new ComponentiViewModel();

            // Popola con dati di esempio
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            var componentiEsempio = new List<ComponenteItem>
            {
                new ComponenteItem { Codice = "COMP001", Descrizione = "Componente Base" },
                new ComponenteItem { Codice = "COMP002", Descrizione = "Componente Premium" },
                new ComponenteItem { Codice = "COMP003", Descrizione = "Componente Speciale" },
                new ComponenteItem { Codice = "COMP004", Descrizione = "Componente Standard" },
                new ComponenteItem { Codice = "COMP005", Descrizione = "Componente Avanzato" }
            };

            foreach (var componente in componentiEsempio)
            {
                ComponentiDataGrid.Items.Add(componente);
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ComponentiViewModel viewModel)
            {
                viewModel.NuovoComponente();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ComponentiViewModel viewModel)
            {
                viewModel.SalvaComponente();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ComponentiViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare il componente selezionato?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaComponente();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ComponentiViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ComponentiViewModel viewModel)
            {
                viewModel.CercaComponenti();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ComponentiViewModel viewModel)
            {
                if (ComponentiDataGrid.SelectedItem is ComponenteItem selectedItem)
                {
                    viewModel.SelezionaComponente(selectedItem.Codice, selectedItem.Descrizione);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    ComponenteDescrizioneTextBox.Text = selectedItem.Descrizione;
                }
            }
        }
    }
}