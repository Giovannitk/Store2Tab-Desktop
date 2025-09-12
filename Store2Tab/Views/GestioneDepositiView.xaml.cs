using Store2Tab.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid dei Depositi
    /// </summary>
    public class DepositoItem
    {
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public bool DepositoPrincipale { get; set; } = false;
        public bool VisualizzaNelleRicerche { get; set; } = true;
        public string Sigla { get; set; } = string.Empty;
        public string OrdineVisualizzazione { get; set; } = "1";
    }

    public partial class GestioneDepositiView : UserControl
    {
        public GestioneDepositiView()
        {
            InitializeComponent();
            DataContext = new DepositiViewModel();

            // Popola con dati di esempio
            PopulateDataGrid();
        }

        private void PopulateDataGrid()
        {
            var depositiEsempio = new List<DepositoItem>
            {
                new DepositoItem
                {
                    Codice = "1",
                    Descrizione = "Principale",
                    DepositoPrincipale = true,
                    VisualizzaNelleRicerche = true,
                    Sigla = "PRIN",
                    OrdineVisualizzazione = "1"
                },
                new DepositoItem
                {
                    Codice = "2",
                    Descrizione = "Magazzino Nord",
                    DepositoPrincipale = false,
                    VisualizzaNelleRicerche = true,
                    Sigla = "NORD",
                    OrdineVisualizzazione = "2"
                },
                new DepositoItem
                {
                    Codice = "3",
                    Descrizione = "Magazzino Sud",
                    DepositoPrincipale = false,
                    VisualizzaNelleRicerche = false,
                    Sigla = "SUD",
                    OrdineVisualizzazione = "3"
                },
                new DepositoItem
                {
                    Codice = "4",
                    Descrizione = "Deposito Temporaneo",
                    DepositoPrincipale = false,
                    VisualizzaNelleRicerche = true,
                    Sigla = "TEMP",
                    OrdineVisualizzazione = "4"
                }
            };

            foreach (var deposito in depositiEsempio)
            {
                DepositiDataGrid.Items.Add(deposito);
            }
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is DepositiViewModel viewModel)
            {
                viewModel.NuovoDeposito();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is DepositiViewModel viewModel)
            {
                viewModel.SalvaDeposito();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is DepositiViewModel viewModel)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare il deposito selezionato?",
                    "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    viewModel.CancellaDeposito();
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is DepositiViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is DepositiViewModel viewModel)
            {
                viewModel.CercaDepositi();
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is DepositiViewModel viewModel)
            {
                if (DepositiDataGrid.SelectedItem is DepositoItem selectedItem)
                {
                    viewModel.SelezionaDeposito(selectedItem.Codice, selectedItem.Descrizione,
                                               selectedItem.DepositoPrincipale, selectedItem.VisualizzaNelleRicerche,
                                               selectedItem.Sigla, selectedItem.OrdineVisualizzazione);

                    // Aggiorna i campi del pannello dettagli
                    CodiceTextBox.Text = selectedItem.Codice;
                    DepositoDescrizioneTextBox.Text = selectedItem.Descrizione;
                    DepositoPrincipaleCheckBox.IsChecked = selectedItem.DepositoPrincipale;
                    VisualizzaNelleRicercheCheckBox.IsChecked = selectedItem.VisualizzaNelleRicerche;
                    SiglaTextBox.Text = selectedItem.Sigla;
                    OrdineVisualizzazioneTextBox.Text = selectedItem.OrdineVisualizzazione;
                }
            }
        }
    }
}