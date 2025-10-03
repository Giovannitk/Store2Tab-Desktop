using Store2Tab.Core.Interfaces;
using Store2Tab.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Store2Tab.Views
{
    /// <summary>
    /// Classe per gli elementi del DataGrid
    /// </summary>
    public class NumerazioneOrdiniItem : INotifyPropertyChanged
    {
        private string _codice = string.Empty;
        private string _descrizione = string.Empty;
        private string _sigla = string.Empty;
        private string _stampaTelefonoCliente = string.Empty;
        private bool _isSelected;

        public string Codice
        {
            get => _codice;
            set
            {
                if (_codice != value)
                {
                    _codice = value;
                    OnPropertyChanged(nameof(Codice));
                }
            }
        }

        public string Descrizione
        {
            get => _descrizione;
            set
            {
                if (_descrizione != value)
                {
                    _descrizione = value;
                    OnPropertyChanged(nameof(Descrizione));
                }
            }
        }

        public string Sigla
        {
            get => _sigla;
            set
            {
                if (_sigla != value)
                {
                    _sigla = value;
                    OnPropertyChanged(nameof(Sigla));
                }
            }
        }

        public string StampaTelefonoCliente
        {
            get => _stampaTelefonoCliente;
            set
            {
                if (_stampaTelefonoCliente != value)
                {
                    _stampaTelefonoCliente = value;
                    OnPropertyChanged(nameof(StampaTelefonoCliente));
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class GestioneNumerazioneOrdiniView : UserControl
    {
        private NumerazioneOrdiniViewModel? _viewModel;
        private bool _isUpdatingCheckboxes = false;

        public GestioneNumerazioneOrdiniView()
        {
            InitializeComponent();
            _viewModel = new NumerazioneOrdiniViewModel();
            DataContext = _viewModel;

            // Bind delle proprietà del ViewModel ai controlli
            BindControls();
        }

        private void BindControls()
        {
            if (_viewModel == null) return;

            // Binding dei campi di ricerca
            CodiceRicercaTextBox.SetBinding(TextBox.TextProperty,
                new System.Windows.Data.Binding("CodiceRicerca")
                {
                    Source = _viewModel,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });

            DescrizioneRicercaTextBox.SetBinding(TextBox.TextProperty,
                new System.Windows.Data.Binding("DescrizioneRicerca")
                {
                    Source = _viewModel,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });

            // Binding dei campi di dettaglio
            CodiceTextBox.SetBinding(TextBox.TextProperty,
                new System.Windows.Data.Binding("Codice")
                {
                    Source = _viewModel,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });

            NumerazioneDescrizioneTextBox.SetBinding(TextBox.TextProperty,
                new System.Windows.Data.Binding("Descrizione")
                {
                    Source = _viewModel,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });

            SiglaTextBox.SetBinding(TextBox.TextProperty,
                new System.Windows.Data.Binding("Sigla")
                {
                    Source = _viewModel,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });

            StampaTelefonoClienteTextBox.SetBinding(TextBox.TextProperty,
                new System.Windows.Data.Binding("StampaTelefonoCliente")
                {
                    Source = _viewModel,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });

            NumerazioneOrdiniClientiCheckBox.SetBinding(CheckBox.IsCheckedProperty,
                new System.Windows.Data.Binding("IsDefaultCliente")
                {
                    Source = _viewModel,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });

            NumerazioneOrdiniFornitoriCheckBox.SetBinding(CheckBox.IsCheckedProperty,
                new System.Windows.Data.Binding("IsDefaultFornitore")
                {
                    Source = _viewModel,
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
                });

            // Binding della collection
            NumerazioneDataGrid.SetBinding(DataGrid.ItemsSourceProperty,
                new System.Windows.Data.Binding("Numerazioni")
                {
                    Source = _viewModel
                });
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.NuovaNumerazione();
            NumerazioneDescrizioneTextBox.Focus();
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.SalvaNumerazione();
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is NumerazioneOrdiniViewModel viewModel)
            {
                // Verifica se ci sono elementi selezionati per cancellazione multipla
                var selectedItems = NumerazioneDataGrid.Items.Cast<NumerazioneOrdiniItem>()
                .Where(item => item.IsSelected)
                .ToList();

                if (selectedItems.Count == 0)
                {
                    MessageBox.Show("Selezionare una o più numerazioni da cancellare.",
                        "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedItems.Count > 1)
                {
                    var confirm = MessageBox.Show("Si sta per procedere alla cancellazione di più numerazioni. Confermare l'operazione.",
                        "Conferma", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (confirm == MessageBoxResult.OK)
                    {
                        _viewModel?.CancellaMultiple(selectedItems);
                    }
                }
                else
                {
                    // Cancellazione singola: conferma e usa l'ID dell'elemento spuntato
                    var item = selectedItems.First();
                    var confirm = MessageBox.Show($"CONFERMI LA CANCELLAZIONE DELLA NUMERAZIONE CORRENTE?\n\nCodice: {item.Codice}\nDescrizione: {item.Descrizione}",
                        "Conferma", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (confirm == MessageBoxResult.OK)
                    {
                        // Allinea il VM per riutilizzare il metodo esistente
                        _viewModel.Codice = item.Codice;
                        _viewModel.CancellaNumerazione();
                    }
                }
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.AnnullaModifiche();
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.CercaNumerazioni();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null || NumerazioneDataGrid.SelectedItem == null)
                return;

            if (NumerazioneDataGrid.SelectedItem is NumerazioneOrdiniItem selectedItem)
            {
                // Auto-spunta la checkbox dell'elemento selezionato
                selectedItem.IsSelected = true;
                _viewModel.SelezionaNumerazione(
                    selectedItem.Codice,
                    selectedItem.Descrizione,
                    selectedItem.Sigla,
                    selectedItem.StampaTelefonoCliente
                );
            }
        }

        // Handlers rimossi: le checkbox sono ora indipendenti

        private void StampaTelefonoClienteTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Accetta solo 0, 1 o 2
            e.Handled = !(e.Text == "0" || e.Text == "1" || e.Text == "2");
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (_viewModel == null) return;

            switch (e.Key)
            {
                case Key.F1:
                    Nuovo_Click(sender, e);
                    e.Handled = true;
                    break;
                case Key.F2:
                    if (_viewModel.IsDirty)
                    {
                        Salva_Click(sender, e);
                        e.Handled = true;
                    }
                    break;
                case Key.F3:
                    Cerca_Click(sender, e);
                    e.Handled = true;
                    break;
                case Key.F4:
                    Cancella_Click(sender, e);
                    e.Handled = true;
                    break;
                case Key.F8:
                    if (_viewModel.IsDirty)
                    {
                        Annulla_Click(sender, e);
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    if (_viewModel.IsDirty)
                    {
                        var result = MessageBox.Show(
                            "SALVARE LE MODIFICHE APPORTATE ALLA NUMERAZIONE CORRENTE?",
                            "Conferma",
                            MessageBoxButton.YesNoCancel,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            Salva_Click(sender, e);
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            // Chiudi la vista
                            var window = Window.GetWindow(this);
                            window?.Close();
                        }
                    }
                    else
                    {
                        var window = Window.GetWindow(this);
                        window?.Close();
                    }
                    e.Handled = true;
                    break;
            }
        }

        private void CodiceRicercaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Cerca_Click(sender, e);
                e.Handled = true;
            }
        }

        private void DescrizioneRicercaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Cerca_Click(sender, e);
                e.Handled = true;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Imposta il focus sul campo descrizione se non ci sono dati
            if (_viewModel?.Numerazioni.Count == 0)
            {
                NumerazioneDescrizioneTextBox.Focus();
            }
        }
    }
}