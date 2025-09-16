// Store2Tab/ViewModels/BancheViewModel.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Windows;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data.Models;

namespace Store2Tab.ViewModels
{
    public class BancheViewModel : INotifyPropertyChanged
    {
        private readonly IBancaService _bancaService;
        private ObservableCollection<Banca> _banche;
        private Banca? _selectedBanca;
        private string _searchText = string.Empty;
        private bool _isLoading = false;

        // Per adesso metto una proprietà fake per la licenza
        public bool LicenzaScaduta { get; set; } = false;

        public BancheViewModel(IBancaService bancaService)
        {
            _bancaService = bancaService;
            _banche = new ObservableCollection<Banca>();
            _ = LoadBancheAsync(); // Fire and forget per il caricamento iniziale
        }

        public ObservableCollection<Banca> Banche
        {
            get => _banche;
            set
            {
                _banche = value;
                OnPropertyChanged();
            }
        }

        public Banca? SelectedBanca
        {
            get => _selectedBanca;
            set
            {
                _selectedBanca = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private List<Banca> _elementiSelezionati = new List<Banca>();

        public List<Banca> ElementiSelezionati
        {
            get => _elementiSelezionati;
            set
            {
                _elementiSelezionati = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasMultipleSelection));
                OnPropertyChanged(nameof(SelectionInfo));
            }
        }

        public bool HasMultipleSelection => ElementiSelezionati.Count > 1;

        public string SelectionInfo
        {
            get
            {
                if (ElementiSelezionati.Count == 0) return "Nessuna selezione";
                if (ElementiSelezionati.Count == 1) return $"1 banca selezionata";
                return $"{ElementiSelezionati.Count} banche selezionate";
            }
        }

        public async void NuovaBanca()
        {
            try
            {
                // Crea una nuova banca con tutti i campi inizializzati
                SelectedBanca = new Banca
                {
                    ID = 0, // Sarà auto-generato dal database
                    Codice = "",
                    Denominazione = "",
                    Agenzia = "",
                    ABI = "",
                    CAB = "",
                    CC = "",
                    CIN = "",
                    IBAN = "",
                    SWIFT = "",
                    NoteInterne = "",
                    Predefinita = 0
                };

                MessageBox.Show("Inserisci almeno Codice e Denominazione, poi clicca Salva",
                    "Nuova Banca", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la creazione di una nuova banca: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void SalvaBanca()
        {
            if (SelectedBanca == null) return;

            try
            {
                if (SelectedBanca.ID == 0)
                {
                    var nuova = await _bancaService.CreateBancaAsync(
                        SelectedBanca,
                        licenzaScaduta: LicenzaScaduta,
                        confermaUtente: msg =>
                        {
                            var result = MessageBox.Show(msg, "Conferma", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            return result == MessageBoxResult.Yes;
                        });
                    Banche.Add(nuova);
                }
                else
                {
                    var aggiornata = await _bancaService.UpdateBancaAsync(
                        SelectedBanca,
                        confermaUtente: msg =>
                        {
                            var result = MessageBox.Show(msg, "Conferma", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            return result == MessageBoxResult.Yes;
                        });
                    var index = Banche.IndexOf(SelectedBanca);
                    if (index >= 0)
                    {
                        Banche[index] = aggiornata;
                        SelectedBanca = aggiornata; // Aggiorna la selezione
                    }
                }

                MessageBox.Show("Banca salvata con successo.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public async Task CancellaBancaSingola(Banca banca)
        {
            if (banca?.ID == null || banca.ID == 0)
            {
                MessageBox.Show("Banca non valida per la cancellazione.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                await _bancaService.DeleteBancaAsync(banca.ID);
                Banche.Remove(banca);

                if (SelectedBanca == banca)
                    SelectedBanca = null;

                MessageBox.Show("Banca cancellata con successo!",
                    "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la cancellazione di '{banca.Denominazione}': {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void CancellaBancheMultiple(List<Banca> bancheSelezionate)
        {
            if (bancheSelezionate == null || bancheSelezionate.Count == 0)
            {
                MessageBox.Show("Nessuna banca selezionata per la cancellazione.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;

                var risultatiCancellazione = new List<string>();
                var bancheCancellate = new List<Banca>();
                var bancheNonCancellate = new List<Banca>();

                foreach (var banca in bancheSelezionate)
                {
                    // Chiede conferma per ogni singola banca
                    var conferma = MessageBox.Show(
                        $"Cancellare la banca:\n\n" +
                        $"Codice: {banca.Codice}\n" +
                        $"Denominazione: {banca.Denominazione}\n" +
                        $"Agenzia: {banca.Agenzia}\n\n" +
                        $"Rimanenti da confermare: {bancheSelezionate.Count - bancheSelezionate.IndexOf(banca) - 1}",
                        "Conferma Cancellazione",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (conferma == MessageBoxResult.Cancel)
                    {
                        // L'utente ha annullato l'operazione
                        risultatiCancellazione.Add("Operazione annullata dall'utente.");
                        break;
                    }
                    else if (conferma == MessageBoxResult.Yes)
                    {
                        // Procede con la cancellazione
                        try
                        {
                            if (banca.ID > 0)
                            {
                                await _bancaService.DeleteBancaAsync(banca.ID);
                                bancheCancellate.Add(banca);
                                risultatiCancellazione.Add($"✓ {banca.Denominazione} - Cancellata");
                            }
                            else
                            {
                                risultatiCancellazione.Add($"✗ {banca.Denominazione} - ID non valido");
                                bancheNonCancellate.Add(banca);
                            }
                        }
                        catch (Exception ex)
                        {
                            risultatiCancellazione.Add($"✗ {banca.Denominazione} - Errore: {ex.Message}");
                            bancheNonCancellate.Add(banca);
                        }
                    }
                    else
                    {
                        // L'utente ha detto No per questa banca
                        risultatiCancellazione.Add($"○ {banca.Denominazione} - Saltata");
                        bancheNonCancellate.Add(banca);
                    }
                }

                // Rimuove le banche cancellate dalla collezione
                foreach (var banca in bancheCancellate)
        {
                    Banche.Remove(banca);
                }

                // Reset della selezione
                SelectedBanca = null;
                ElementiSelezionati.Clear();

                // Mostra il riepilogo
                var messaggio = "Riepilogo cancellazione multipla:\n\n" +
                               string.Join("\n", risultatiCancellazione) +
                               $"\n\nBanche cancellate: {bancheCancellate.Count}\n" +
                               $"Banche saltate/errori: {bancheNonCancellate.Count}";

                MessageBox.Show(messaggio, "Riepilogo Cancellazione",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore generale durante la cancellazione multipla: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Mantenuto il metodo CancellaBanca originale per compatibilità (chiamata dai tasti di scelta rapida)
        public async void CancellaBanca()
        {
            if (SelectedBanca?.ID == null || SelectedBanca.ID == 0)
            {
                MessageBox.Show("Nessuna banca selezionata per la cancellazione.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Sei sicuro di voler cancellare la banca '{SelectedBanca.Denominazione}'?",
                "Conferma Cancellazione", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await CancellaBancaSingola(SelectedBanca);
            }
        }

        public async void AnnullaModifiche()
        {
            if (SelectedBanca?.ID != null && SelectedBanca.ID > 0)
            {
                try
                {
                    // Ricarica la banca dal database
                    var bancaOriginale = await _bancaService.GetBancaByIdAsync(SelectedBanca.ID);
                    if (bancaOriginale != null)
                    {
                        SelectedBanca = bancaOriginale;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore durante l'annullamento delle modifiche: {ex.Message}",
                        "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                SelectedBanca = null;
            }
        }

        public async void CercaBanche()
        {
            try
            {
                IsLoading = true;
                var risultati = await _bancaService.SearchBancheAsync(SearchText);

                Banche.Clear();
                foreach (var banca in risultati)
                {
                    Banche.Add(banca);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la ricerca: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadBancheAsync()
        {
            try
            {
                IsLoading = true;
                var banche = await _bancaService.GetAllBancheAsync();

                Banche.Clear();
                foreach (var banca in banche)
                {
                    Banche.Add(banca);
                }
            }
            catch (DbException dbe)
            {
                MessageBox.Show($"Errore di connessione al database: {dbe.Message}",
                    "Errore Database", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show($"Controlla la configurazione della connessione nel file appsettings.json. {dbe.InnerException}",
                    "Configurazione", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"2Errore durante il caricamento delle banche: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show($"2Controlla la configurazione della connessione nel file appsettings.json. {ex.InnerException}",
                    "Configurazione", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}