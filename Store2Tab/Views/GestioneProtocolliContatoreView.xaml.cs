using Store2Tab.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Store2Tab.Views
{
    public partial class GestioneProtocolliContatoriView : UserControl
    {
        private ProtocolliContatoriViewModel ViewModel => (ProtocolliContatoriViewModel)DataContext;

        public GestioneProtocolliContatoriView()
        {
            InitializeComponent();
            DataContext = new ProtocolliContatoriViewModel();

            // Gestione eventi tastiera
            this.KeyDown += GestioneProtocolliContatoriView_KeyDown;
            this.Focusable = true;
            this.Loaded += (s, e) => this.Focus();
        }

        private void GestioneProtocolliContatoriView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2: // Salva
                    if (ViewModel.IsInEditMode)
                    {
                        Salva_Click(sender, e);
                        e.Handled = true;
                    }
                    break;
                case Key.F4: // Cancella
                    Cancella_Click(sender, e);
                    e.Handled = true;
                    break;
                case Key.F8: // Annulla
                    if (ViewModel.IsInEditMode)
                    {
                        Annulla_Click(sender, e);
                        e.Handled = true;
                    }
                    break;
                case Key.Escape: // Chiudi form
                    Window.GetWindow(this)?.Close();
                    e.Handled = true;
                    break;
            }
        }

        private async void Salva_Click(object sender, RoutedEventArgs e)
        {
            bool risultato = await ViewModel.SalvaAsync();
            if (!risultato)
            {
                MessageBox.Show("Errore durante il salvataggio.");
            }
        }

        private void Cancella_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProtocolliContatoriViewModel viewModel)
            {
                viewModel.Elimina();
            }
        }

        private void Annulla_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AnnullaModifiche();
        }

        private void Contatore_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.OnContatoreModificato();
        }
    }
}