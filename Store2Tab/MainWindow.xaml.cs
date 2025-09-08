using System.Windows;
using System.Windows.Controls;

namespace Store2Tab
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowWithParam(string? parametro)
        {
            // Se è stato passato un parametro specifico, apri direttamente quella sezione
            if (!string.IsNullOrEmpty(parametro))
            {
                switch (parametro.ToUpperInvariant())
                {
                    case "TIPOPAGAMENTO":
                    case "PAGAMENTI":
                        OpenPagamentiTab();
                        break;
                    case "ANAGRAFICHE":
                        OpenAnagraficheTab();
                        break;
                    case "ARTICOLI":
                        OpenArticoliTab();
                        break;
                    case "BANCHE":
                        OpenBancheTab();
                        break;
                    // Aggiungi altri casi secondo necessità
                    default:
                        // Parametro non riconosciuto, mostra il menu principale
                        break;
                }
            }

            Show();
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string tag)
            {
                switch (tag.ToUpperInvariant())
                {
                    case "ANAGRAFICHE":
                        OpenAnagraficheTab();
                        break;
                    case "DOCUMENTI":
                        OpenDocumentiTab();
                        break;
                    case "BANCHE":
                        OpenBancheTab();
                        break;
                    case "ARTICOLI":
                        OpenArticoliTab();
                        break;
                    case "CAUSALIIVA":
                        OpenCausaliIvaTab();
                        break;
                    case "CONTABILITA":
                        OpenContabilitaTab();
                        break;
                    case "PAGAMENTI":
                        OpenPagamentiTab();
                        break;
                    case "VARIE":
                        OpenVarieTab();
                        break;
                }
            }
        }

        private void OpenPagamentiTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("PAGAMENTI");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[TIPI DI PAGAMENTO]",
                Tag = "PAGAMENTI"
            };

            var pagamentiView = new Views.GestioneTipiPagamentoView();
            tabItem.Content = pagamentiView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenAnagraficheTab()
        {
            var existingTab = FindTabByTag("ANAGRAFICHE");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            var tabItem = new TabItem
            {
                Header = "ANAGRAFICHE",
                Tag = "ANAGRAFICHE"
            };

            // Crea la vista per le anagrafiche (dovrai crearla)
            var anagraficheView = new Views.MenuPrincipaleView(); // Temporaneo
            tabItem.Content = anagraficheView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenDocumentiTab()
        {
            var existingTab = FindTabByTag("DOCUMENTI");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            var tabItem = new TabItem
            {
                Header = "DOCUMENTI",
                Tag = "DOCUMENTI"
            };

            var documentiView = new Views.MenuPrincipaleView(); // Temporaneo
            tabItem.Content = documentiView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenBancheTab()
        {
            var existingTab = FindTabByTag("BANCHE");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            var tabItem = new TabItem
            {
                Header = "BANCHE",
                Tag = "BANCHE"
            };

            var bancheView = new Views.MenuPrincipaleView(); // Temporaneo
            tabItem.Content = bancheView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenArticoliTab()
        {
            var existingTab = FindTabByTag("ARTICOLI");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            var tabItem = new TabItem
            {
                Header = "ARTICOLI",
                Tag = "ARTICOLI"
            };

            var articoliView = new Views.MenuPrincipaleView(); // Temporaneo
            tabItem.Content = articoliView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenCausaliIvaTab()
        {
            var existingTab = FindTabByTag("CAUSALIIVA");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            var tabItem = new TabItem
            {
                Header = "CAUSALI IVA",
                Tag = "CAUSALIIVA"
            };

            var causaliView = new Views.MenuPrincipaleView(); // Temporaneo
            tabItem.Content = causaliView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenContabilitaTab()
        {
            var existingTab = FindTabByTag("CONTABILITA");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            var tabItem = new TabItem
            {
                Header = "CONTABILITA'",
                Tag = "CONTABILITA"
            };

            var contabilitaView = new Views.MenuPrincipaleView(); // Temporaneo
            tabItem.Content = contabilitaView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenVarieTab()
        {
            var existingTab = FindTabByTag("VARIE");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            var tabItem = new TabItem
            {
                Header = "VARIE",
                Tag = "VARIE"
            };

            var varieView = new Views.MenuPrincipaleView(); // Temporaneo
            tabItem.Content = varieView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private TabItem? FindTabByTag(string tag)
        {
            foreach (TabItem item in MainTabControl.Items)
            {
                if (item.Tag?.ToString() == tag)
                {
                    return item;
                }
            }
            return null;
        }
    }
}