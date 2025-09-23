using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Store2Tab.Views
{
    public partial class GestionePassaportoPianteCeeView : UserControl
    {
        public GestionePassaportoPianteCeeView()
        {
            InitializeComponent();
            DataContext = new PassPianteCeeTipoViewModel();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel viewModel)
            {
                viewModel.NuovoPassaporto();
            }
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel viewModel)
            {
                viewModel.SalvaPassaporto();
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel viewModel)
            {
                viewModel.CancellaPassaporto();
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel viewModel)
            {
                viewModel.AnnullaModifiche();
            }
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel viewModel)
            {
                viewModel?.CercaPassaporti();
            }
        }

        private void CodiceRicercaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is PassPianteCeeTipoViewModel vm)
            {
                vm?.CercaPassaporti();
            }
        }

        private void DescrizioneRicercaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is PassPianteCeeTipoViewModel vm)
            {
                vm?.CercaPassaporti();
            }
        }

        private void NumerazioneComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel vm)
            {
                vm.SegnaModifica();
            }
        }


        private void RaggruppamentoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel vm)
            {
                vm.SegnaModifica();
            }
        }


        // Aggiungi questi event handler al code-behind e collega gli eventi nei checkbox del XAML

        private void PassaportoPianteCeeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel vm && vm.TipoSelezionato != null)
            {
                if (PassaportoPianteCeeCheckBox.IsChecked == true)
                {
                    DocumentoCommercializzazioneCheckBox.IsChecked = false;
                    vm.TipoSelezionato.DocumentoCommerc = 0;
                }
                vm.SegnaModifica();
            }
        }

        private void DocumentoCommercializzazioneCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel vm && vm.TipoSelezionato != null)
            {
                if (DocumentoCommercializzazioneCheckBox.IsChecked == true)
                {
                    PassaportoPianteCeeCheckBox.IsChecked = false;
                    vm.TipoSelezionato.PassaportoCEE = 0;
                }
                vm.SegnaModifica();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel vm)
            {
                vm.SegnaModifica();
            }
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel vm)
            {
                vm.SegnaModifica();
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PassPianteCeeTipoViewModel vm)
            {
                vm.SegnaModifica();
            }
        }
    }
}