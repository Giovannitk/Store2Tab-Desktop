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
            // Se è stato passato un parametro specifico, si apre direttamente quella sezione
            if (!string.IsNullOrEmpty(parametro))
            {
                switch (parametro.ToUpperInvariant())
                {
                    case "TIPOPAGAMENTO":
                    case "PAGAMENTI":
                        OpenPagamentiTab();
                        break;
                    //case "ARTICOLI":
                    //    OpenArticoliTab();
                    //    break;
                    case "COMPONENTI":
                    case "ARTICOLI_COMPONENTI":
                        OpenComponentiTab();
                        break;

                    case "DEPOSITI":
                    case "ARTICOLI_DEPOSITI":
                        OpenDepositiTab();
                        break;
                    case "BANCHE":
                        OpenBancheTab();
                        break;
                    case "CAUSALIIVA":
                    case "CAUSALI":
                        OpenCausaliIvaTab();
                        break;
                    case "DOCUMENTI":
                        OpenDocumentiTab();
                        break;
                    case "NUMERAZIONE":
                    case "DOCUMENTI_NUMERAZIONE":
                    case "EMESSI_NUMERAZIONE":
                        OpenNumerazioneTab();
                        break;
                    case "CONTABILITA":
                        OpenContabilitaTab();
                        break;
                    //case "VARIE":
                    //    OpenVarieTab();
                    //    break;
                    case "PASSAPORTOCEETIPO":
                    case "PASSAPORTO_CEE_TIPO":
                        OpenPassaportoCeeTipo();
                        break;
                    case "PASSAPORTOCOENUMERAZIONI":
                    case "PASSAPORTO_CEE_NUMERAZIONI":
                    case "VIVAI_NUMERAZIONI":
                        OpenPassaportoCeeNumerazioniTab();
                        break;
                    case "SPECIEBOTANICHE":
                    case "SPECIE_BOTANICHE":
                    case "VIVAI_SPECIE":
                        OpenSpecieBotanicheTab();
                        break;
                    case "VARIETA":
                    case "VARIETÀ":
                    case "VIVAI_VARIETA":
                        OpenVarietaTab();
                        break;
                    case "PORTINNESTO":
                    case "PORTINNESTI":
                    case "VIVAI_PORTINNESTO":
                        OpenPortinnestoTab();
                        break;
                    case "VETTORI":
                        OpenVettoriTab();
                        break;
                    case "MEZZIPAGAMENTO":
                    case "MEZZI":
                        OpenMezziPagamentoTab();
                        break;
                    case "CAUSALIMOVIMENTO":
                    case "CAUSALI_MOVIMENTO":
                        OpenCausaliMovimentoTab();
                        break;
                    case "TIPIATTIVITA":
                    case "TIPI_ATTIVITA":
                    case "ATTIVITA":
                        OpenTipiAttivitaTab();
                        break;
                    case "SCHEDATRASPORTO":
                    case "SCHEDA_TRASPORTO":
                        OpenSchedaTrasporto();
                        break;
                    case "NUMERAZIONEORDINI":
                    case "ORDINI_NUMERAZIONE":
                        OpenNumerazioneOrdiniTab();
                        break;
                    case "NOTEPREDEFINITE":
                    case "NOTE_PREDEFINITE":
                        OpenNotePredefiniteTab();
                        break;
                    case "PROTOCOLLI":
                    case "DOCUMENTI_PROTOCOLLI":
                        OpenProtocolliTab();
                        break;
                    case "PROTOCOLLICONTATORI":
                    case "PROTOCOLLI_CONTATORI":
                    case "DOCUMENTI_PROTOCOLLI_CONTATORI":
                        OpenProtocolliContatoriTab();
                        break;
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
                    case "DOCUMENTI":
                        OpenDocumentiTab();
                        break;
                    case "NUMERAZIONE":
                        OpenNumerazioneTab();
                        break;
                    case "BANCHE":
                        OpenBancheTab();
                        break;
                    //case "ARTICOLI":
                    //    OpenArticoliTab();
                    //    break;
                    case "COMPONENTI":
                        OpenComponentiTab();
                        break;
                    case "DEPOSITI":
                        OpenDepositiTab();
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
                    //case "VARIE":
                    //    OpenVarieTab();
                    //    break;
                    case "PASSAPORTOCEETIPO":
                        OpenPassaportoCeeTipo();
                        break;
                    case "PASSAPORTOCOENUMERAZIONI":
                        OpenPassaportoCeeNumerazioniTab();
                        break;
                    case "SPECIEBOTANICHE":
                        OpenSpecieBotanicheTab();
                        break;
                    case "VARIETA":
                        OpenVarietaTab();
                        break;
                    case "PORTINNESTO":
                        OpenPortinnestoTab();
                        break;
                    case "VETTORI":
                        OpenVettoriTab();
                        break;
                    case "MEZZIPAGAMENTO":
                        OpenMezziPagamentoTab();
                        break;
                    case "CAUSALIMOVIMENTO":
                        OpenCausaliMovimentoTab();
                        break;
                    case "TIPIATTIVITA":
                        OpenTipiAttivitaTab();
                        break;
                    case "SCHEDATRASPORTO":
                        OpenSchedaTrasporto();
                        break;
                    case "NUMERAZIONEORDINI":
                        OpenNumerazioneOrdiniTab();
                        break;
                    case "NOTEPREDEFINITE":
                        OpenNotePredefiniteTab();
                        break;
                    case "PROTOCOLLI":
                        OpenProtocolliTab();
                        break;
                    case "PROTOCOLLICONTATORI":
                        OpenProtocolliContatoriTab();
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
                Tag = "PAGAMENTI",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
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
        private void OpenMezziPagamentoTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("MEZZIPAGAMENTO");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[MEZZI DI PAGAMENTO]",
                Tag = "MEZZIPAGAMENTO",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var mezziPagamentoView = new Views.GestioneMezziPagamentoView();
            tabItem.Content = mezziPagamentoView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenCausaliMovimentoTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("CAUSALIMOVIMENTO");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[CAUSALI MOVIMENTO]",
                Tag = "CAUSALIMOVIMENTO",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var causaliMovimentoView = new Views.GestioneCausaliMovimentoView();
            tabItem.Content = causaliMovimentoView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenTipiAttivitaTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("TIPIATTIVITA");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[ANAGRAFICA - TIPI DI ATTIVITA']",
                Tag = "TIPIATTIVITA",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var tipiAttivitaView = new Views.GestioneTipiAttivitaView();
            tabItem.Content = tipiAttivitaView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
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
                Tag = "DOCUMENTI",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var documentiView = new Views.MenuPrincipaleView(); // Temporaneo
            tabItem.Content = documentiView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenNumerazioneTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("NUMERAZIONE");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[DOCUMENTI EMESSI - NUMERAZIONE]",
                Tag = "NUMERAZIONE",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var numerazioneView = new Views.GestioneNumerazioneView();
            tabItem.Content = numerazioneView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
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
                Tag = "BANCHE",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var bancheView = new Views.GestioneBancheView();
            tabItem.Content = bancheView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        //private void OpenArticoliTab()
        //{
        //    var existingTab = FindTabByTag("ARTICOLI");
        //    if (existingTab != null)
        //    {
        //        MainTabControl.SelectedItem = existingTab;
        //        return;
        //    }

        //    var tabItem = new TabItem
        //    {
        //        Header = "ARTICOLI",
        //        Tag = "ARTICOLI"
        //    };

        //    var articoliView = new Views.MenuPrincipaleView(); // Temporaneo
        //    tabItem.Content = articoliView;

        //    MainTabControl.Items.Add(tabItem);
        //    MainTabControl.SelectedItem = tabItem;
        //}
        private void OpenComponentiTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("COMPONENTI");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[ARTICOLI - COMPONENTI]",
                Tag = "COMPONENTI",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var componentiView = new Views.GestioneComponentiView();
            tabItem.Content = componentiView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenDepositiTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("DEPOSITI");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[ARTICOLI - DEPOSITI]",
                Tag = "DEPOSITI",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var depositiView = new Views.GestioneDepositiView();
            tabItem.Content = depositiView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
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
                Tag = "CAUSALIIVA",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var causaliView = new Views.GestioneCausaliIvaView();
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
                Tag = "CONTABILITA",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var contabilitaView = new Views.MenuPrincipaleView(); // Temporaneo
            tabItem.Content = contabilitaView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        //private void OpenVarieTab()
        //{
        //    var existingTab = FindTabByTag("VARIE");
        //    if (existingTab != null)
        //    {
        //        MainTabControl.SelectedItem = existingTab;
        //        return;
        //    }

        //    var tabItem = new TabItem
        //    {
        //        Header = "VARIE",
        //        Tag = "VARIE"
        //    };

        //    var varieView = new Views.MenuPrincipaleView(); // Temporaneo
        //    tabItem.Content = varieView;

        //    MainTabControl.Items.Add(tabItem);
        //    MainTabControl.SelectedItem = tabItem;
        //}
        private void OpenPassaportoCeeTipo()
        {
            var existingTab = FindTabByTag("PASSAPORTOCEETIPO");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }
            var tabItem = new TabItem
            {
                Header = "PASSAPORTO CEE - TIPO",
                Tag = "PASSAPORTOCEETIPO",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };
            var passaportoCeeTipoView = new Views.GestionePassaportoPianteCeeView(); // Temporaneo
            tabItem.Content = passaportoCeeTipoView;
            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenPassaportoCeeNumerazioniTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("PASSAPORTOCOENUMERAZIONI");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }
            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[VARIE - VIVAI - PASSAPORTO CEE NUMERAZIONI]",
                Tag = "PASSAPORTOCOENUMERAZIONI",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };
            var numerazioniView = new Views.GestionePassaportoCeeNumerazioniView();
            tabItem.Content = numerazioniView;
            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenSpecieBotanicheTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("SPECIEBOTANICHE");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[VARIE - VIVAI - SPECIE BOTANICHE]",
                Tag = "SPECIEBOTANICHE",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var specieBotanicheView = new Views.GestioneSpecieBotanicheView();
            tabItem.Content = specieBotanicheView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenVarietaTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("VARIETA");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[VARIE - VIVAI - VARIETÀ]",
                Tag = "VARIETA",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var varietaView = new Views.GestioneVarietaView();
            tabItem.Content = varietaView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenPortinnestoTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("PORTINNESTO");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[VARIE - VIVAI - PORTINNESTO]",
                Tag = "PORTINNESTO",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var portinnestoView = new Views.GestionePortinnestoView();
            tabItem.Content = portinnestoView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenVettoriTab()
        {
            var existingTab = FindTabByTag("VETTORI");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            var tabItem = new TabItem
            {
                Header = "VETTORI",
                Tag = "VETTORI",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var varieView = new Views.GestioneVettoriView(); // Temporaneo
            tabItem.Content = varieView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenSchedaTrasporto()
        {
            var existingTab = FindTabByTag("SCHEDATRASPORTO");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }
            var tabItem = new TabItem
            {
                Header = "DOCUMENTI EMESSI - SCHEDA TRASPORTO",
                Tag = "SCHEDATRASPORTO",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };
            var schedaTrasportoView = new Views.GestioneSchedaTrasportoView(); // Temporaneo
            tabItem.Content = schedaTrasportoView;
            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenNumerazioneOrdiniTab()
        {
            var existingTab = FindTabByTag("NUMERAZIONEORDINI");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }
            var tabItem = new TabItem
            {
                Header = "ORDINI - NUMERAZIONE",
                Tag = "NUMERAZIONEORDINI",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };
            var numerazioneOrdiniView = new Views.GestioneNumerazioneOrdiniView();
            tabItem.Content = numerazioneOrdiniView;
            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenNotePredefiniteTab()
        {
            var existingTab = FindTabByTag("NOTEPREDEFINITE");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }
            var tabItem = new TabItem
            {
                Header = "NOTE PREDEFINITE",
                Tag = "NOTEPREDEFINITE",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };
            var notePredefiniteView = new Views.GestioneNotePredefiniteView(); // Temporaneo
            tabItem.Content = notePredefiniteView;
            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        }

        private void OpenProtocolliTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("PROTOCOLLI");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[DOCUMENTI - PROTOCOLLI]",
                Tag = "PROTOCOLLI",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var protocolliView = new Views.GestioneProtocolliView();
            tabItem.Content = protocolliView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
        }

        private void OpenProtocolliContatoriTab()
        {
            // Controlla se il tab esiste già
            var existingTab = FindTabByTag("PROTOCOLLICONTATORI");
            if (existingTab != null)
            {
                MainTabControl.SelectedItem = existingTab;
                return;
            }

            // Crea nuovo tab
            var tabItem = new TabItem
            {
                Header = "[PROTOCOLLI CONTATORI]",
                Tag = "PROTOCOLLICONTATORI",
                HeaderTemplate = (DataTemplate)MainTabControl.Resources["ClosableTabHeaderTemplate"]
            };

            var protocolliContatoriView = new Views.GestioneProtocolliContatoriView();
            tabItem.Content = protocolliContatoriView;

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;

            // Nascondi il tab Welcome se necessario
            if (WelcomeTab.IsSelected == false && MainTabControl.Items.Count > 1)
            {
                WelcomeTab.Visibility = Visibility.Collapsed;
            }
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

        // Gestore unico per la chiusura 
        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TabItem tab)
            {
                MainTabControl.Items.Remove(tab);

                // Mostra di nuovo il WelcomeTab se non ci sono altri tab aperti
                if (MainTabControl.Items.Count == 1 && WelcomeTab.Visibility == Visibility.Collapsed)
                {
                    WelcomeTab.Visibility = Visibility.Visible;
                    MainTabControl.SelectedItem = WelcomeTab;
                }
            }
        }

    }
}