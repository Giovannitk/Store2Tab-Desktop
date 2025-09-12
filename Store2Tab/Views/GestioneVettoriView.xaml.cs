using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Store2Tab.Views
{
    public partial class GestioneVettoriView : UserControl
    {
        public GestioneVettoriView()
        {
            InitializeComponent();
            DataContext = new VettoriViewModel();
        }

        private void Nuovo_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VettoriViewModel vm)
                vm.NuovoVettore();
        }

        private void Salva_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VettoriViewModel vm)
                vm.SalvaVettore();
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VettoriViewModel vm)
            {
                var result = MessageBox.Show("Sei sicuro di voler cancellare il vettore selezionato?",
                    "Conferma", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                    vm.CancellaVettore();
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VettoriViewModel vm)
                vm.AnnullaModifiche();
        }

        private void Cerca_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is VettoriViewModel vm)
                vm.CercaVettori();
        }
    }
}
