using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Store2Tab.Data.Models;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Views;
using System.Collections.Generic;
using System.Linq;

namespace Store2Tab.ViewModels
{
    public class CausaliMovimentoViewModel : INotifyPropertyChanged
    {
        private readonly ICausaleMovimentoService _causaleService;
        private CausaleMovimento? _causaleCorrente;
        private string _codiceOriginale = string.Empty;
        private bool _isModified = false;
        private bool _suspendIsModified = false;

        // Proprietà per i campi di ricerca
        private string _filtroCodice = string.Empty;
        private string _filtroDescrizione = string.Empty;

        // Proprietà per i campi di dettaglio
        private string _codice = string.Empty;
        private string _descrizione = string.Empty;
        private string _codiceControMovimento = string.Empty;
        private string _descrizioneControMovimento = string.Empty;
        private bool _codiceReadOnly = false;

        // Collection per il DataGrid
        public ObservableCollection<CausaleMovimentoItem> CausaliMovimento { get; set; }

        public CausaliMovimentoViewModel()
        {
            // Ottieni il service tramite DI
            _causaleService = App.ServiceProvider.GetRequiredService<ICausaleMovimentoService>();
            CausaliMovimento = new ObservableCollection<CausaleMovimentoItem>();

            // Carica i dati iniziali
            _ = Task.Run(async () => await CercaCausaliMovimento());
        }

        #region Proprietà per il binding

        public string FiltroCodice
        {
            get => _filtroCodice;
            set => SetProperty(ref _filtroCodice, value);
        }

        public string FiltroDescrizione
        {
            get => _filtroDescrizione;
            set => SetProperty(ref _filtroDescrizione, value);
        }

        public string Codice
        {
            get => _codice;
            set
            {
                if (SetProperty(ref _codice, value))
                {
                    MarkAsModified();
                }
            }
        }

        public bool CodiceReadOnly
        {
            get => _codiceReadOnly;
            set => SetProperty(ref _codiceReadOnly, value);
        }

        public string Descrizione
        {
            get => _descrizione;
            set
            {
                if (SetProperty(ref _descrizione, value))
                {
                    MarkAsModified();
                }
            }
        }

        public string CodiceControMovimento
        {
            get => _codiceControMovimento;
            set
            {
                if (SetProperty(ref _codiceControMovimento, value))
                {
                    MarkAsModified();
                    // Quando cambia il codice contro movimento, aggiorna anche la descrizione
                    _ = Task.Run(async () => await AggiornaCausaleControMovimento(value));
                }
            }
        }

        public string DescrizioneControMovimento
        {
            get => _descrizioneControMovimento;
            set => SetProperty(ref _descrizioneControMovimento, value);
        }

        public bool IsModified
        {
            get => _isModified;
            private set => SetProperty(ref _isModified, value);
        }

        #endregion

        #region Metodi pubblici per i comandi

        public async Task CercaCausaliMovimento()
        {
            try
            {
                // Salva modifiche pendenti se necessario
                if (IsModified)
                {
                    var result = MessageBox.Show("Salvare le modifiche apportate alla scheda corrente?",
                        "Conferma", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (!await SalvaCausaleMovimento())
                            return;
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }

                var causali = await _causaleService.GetCausaliMovimentoAsync(
                    string.IsNullOrWhiteSpace(FiltroCodice) ? null : FiltroCodice,
                    string.IsNullOrWhiteSpace(FiltroDescrizione) ? null : FiltroDescrizione
                );

                // Aggiorna la collection su UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CausaliMovimento.Clear();
                    foreach (var causale in causali)
                    {
                        CausaliMovimento.Add(new CausaleMovimentoItem
                        {
                            Codice = causale.Codice,
                            Descrizione = causale.Descrizione
                        });
                    }
                });

                // Reset dei campi di dettaglio
                SuspendIsModified(() => ResetCampiDettaglio());
                IsModified = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la ricerca: {ex.Message}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task SelezionaCausaleMovimento(string codice, string descrizione)
        {
            try
            {
                // Salva modifiche pendenti se necessario
                if (IsModified && !await ConfermaAbbandono())
                    return;

                _causaleCorrente = await _causaleService.GetCausaleMovimentoByCodeAsync(codice);

                if (_causaleCorrente != null)
                {
                    _codiceOriginale = _causaleCorrente.Codice;
                    SuspendIsModified(() =>
                    {
                        Codice = _causaleCorrente.Codice;
                        Descrizione = _causaleCorrente.Descrizione;
                        CodiceControMovimento = _causaleCorrente.CodiceControMovimento ?? string.Empty;
                        DescrizioneControMovimento = _causaleCorrente.DescrizioneControMovimento;
                    });
                }

                IsModified = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la selezione: {ex.Message}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task NuovaCausaleMovimento()
        {
            try
            {
                // Salva modifiche pendenti se necessario
                if (IsModified && !await ConfermaAbbandono())
                    return;

                ResetCampiDettaglio();
                _causaleCorrente = null;
                _codiceOriginale = string.Empty;
                IsModified = false;

                // Proponi codice automatico
                string prossimoCodice = await _causaleService.GetNextCodiceAsync();
                var risposta = MessageBox.Show(
                    $"Vuoi usare il codice automatico {prossimoCodice}?\nPremi No per inserirne uno manualmente.",
                    "Nuova Causale - Codice Automatico",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (risposta == MessageBoxResult.Cancel)
                {
                    return;
                }

                if (risposta == MessageBoxResult.Yes)
                {
                    Codice = prossimoCodice;
                    CodiceReadOnly = true; // blocca l'editing del codice
                }
                else
                {
                    CodiceReadOnly = false; // permette inserimento manuale
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore: {ex.Message}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task<bool> SalvaCausaleMovimento()
        {
            try
            {
                // Validazioni
                if (string.IsNullOrWhiteSpace(Codice))
                {
                    MessageBox.Show("Il codice è obbligatorio.", "Errore",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(Descrizione))
                {
                    MessageBox.Show("La descrizione è obbligatoria.", "Errore",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Verifica univocità del codice
                if (await _causaleService.EsisteCodiceAsync(Codice, _codiceOriginale))
                {
                    MessageBox.Show("Il codice inserito è già associato ad un'altra causale.", "Errore",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                var causale = new CausaleMovimento
                {
                    Codice = Codice,
                    Descrizione = Descrizione,
                    CodiceControMovimento = string.IsNullOrWhiteSpace(CodiceControMovimento) ? string.Empty : CodiceControMovimento
                };

                bool success = await _causaleService.SalvaCausaleMovimentoAsync(causale);

                if (success)
                {
                    _causaleCorrente = causale;
                    _codiceOriginale = causale.Codice;
                    IsModified = false;

                    // Aggiorna la lista
                    await CercaCausaliMovimento();

                    MessageBox.Show("Causale movimento salvata con successo.", "Successo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("Errore durante il salvataggio.", "Errore",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il salvataggio: {ex.Message} - {ex.InnerException}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task CancellaCausaleMovimento()
        {
            try
            {
                if (_causaleCorrente == null || string.IsNullOrWhiteSpace(_causaleCorrente.Codice))
                {
                    MessageBox.Show("Nessuna causale selezionata per la cancellazione.", "Attenzione",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                bool success = await _causaleService.CancellaCausaleMovimentoAsync(_causaleCorrente.Codice);

                if (success)
                {
                    // Aggiorna la lista
                    await CercaCausaliMovimento();
                    MessageBox.Show("Causale movimento cancellata con successo.", "Successo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Errore durante la cancellazione.", "Errore",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la cancellazione: {ex.Message}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void CancellaCausaliMultiple(List<CausaleMovimentoItem> causaliSelezionate)
        {
            if (causaliSelezionate == null || causaliSelezionate.Count == 0)
            {
                MessageBox.Show("Nessuna causale selezionata per la cancellazione.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var risultati = new List<string>();
                var cancellate = new List<CausaleMovimentoItem>();
                var nonCancellate = new List<CausaleMovimentoItem>();

                foreach (var item in causaliSelezionate)
                {
                    var conferma = MessageBox.Show(
                        $"Cancellare la causale movimento:\n\n" +
                        $"Codice: {item.Codice}\n" +
                        $"Descrizione: {item.Descrizione}",
                        "Conferma Cancellazione",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (conferma == MessageBoxResult.Cancel)
                    {
                        risultati.Add("Operazione annullata dall'utente.");
                        break;
                    }
                    else if (conferma == MessageBoxResult.No)
                    {
                        risultati.Add($"○ {item.Codice} - {item.Descrizione} - Saltata");
                        nonCancellate.Add(item);
                        continue;
                    }

                    try
                    {
                        bool success = await _causaleService.CancellaCausaleMovimentoAsync(item.Codice);
                        if (success)
                        {
                            cancellate.Add(item);
                            risultati.Add($"✓ {item.Codice} - {item.Descrizione} - Cancellata");
                        }
                        else
                        {
                            risultati.Add($"✗ {item.Codice} - {item.Descrizione} - Errore durante la cancellazione");
                            nonCancellate.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        risultati.Add($"✗ {item.Codice} - {item.Descrizione} - Errore: {ex.Message}");
                        nonCancellate.Add(item);
                    }
                }

                if (cancellate.Count > 0)
                {
                    // Aggiorna la lista locale senza ricaricare tutto se possibile
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var c in cancellate)
                        {
                            var found = CausaliMovimento.FirstOrDefault(x => x.Codice == c.Codice);
                            if (found != null)
                            {
                                CausaliMovimento.Remove(found);
                            }
                        }
                    });
                }

                var messaggio = "Riepilogo cancellazione multipla:\n\n" +
                               string.Join("\n", risultati) +
                               $"\n\nCausali cancellate: {cancellate.Count}\n" +
                               $"Causali saltate/errori: {nonCancellate.Count}";

                MessageBox.Show(messaggio, "Riepilogo Cancellazione",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore generale durante la cancellazione multipla: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AnnullaModifiche()
        {
            try
            {
                if (!IsModified)
                    return;

                if (_causaleCorrente != null)
                {
                    // Ripristina i valori originali
                    SuspendIsModified(() =>
                    {
                        Codice = _causaleCorrente.Codice;
                        Descrizione = _causaleCorrente.Descrizione;
                        CodiceControMovimento = _causaleCorrente.CodiceControMovimento ?? string.Empty;
                        DescrizioneControMovimento = _causaleCorrente.DescrizioneControMovimento;
                    });
                }
                else
                {
                    SuspendIsModified(() => ResetCampiDettaglio());
                }

                IsModified = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore: {ex.Message}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ApriSelezioneControMovimento()
        {
            try
            {
                var selectionWindow = new CausaleMovimentoSelectionWindow();
                var selectionViewModel = new CausaleMovimentoSelectionViewModel();
                selectionWindow.DataContext = selectionViewModel;

                if (selectionWindow.ShowDialog() == true && selectionViewModel.CausaleSelezionata != null)
                {
                    CodiceControMovimento = selectionViewModel.CausaleSelezionata.Codice;
                    DescrizioneControMovimento = selectionViewModel.CausaleSelezionata.Descrizione;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore: {ex.Message}", "Errore",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Metodi privati

        private void MarkAsModified()
        {
            if (_suspendIsModified) return;
            IsModified = true;
        }

        private void ResetCampiDettaglio()
        {
            Codice = string.Empty;
            Descrizione = string.Empty;
            CodiceControMovimento = string.Empty;
            DescrizioneControMovimento = string.Empty;
            CodiceReadOnly = false;
        }

        private async Task<bool> ConfermaAbbandono()
        {
            var result = MessageBox.Show("Salvare le modifiche apportate alla scheda corrente?",
                "Conferma", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

            if (result == MessageBoxResult.Yes)
            {
                return await SalvaCausaleMovimento();
            }
            else if (result == MessageBoxResult.No)
            {
                return true;
            }

            return false; // Cancel
        }

        private async Task AggiornaCausaleControMovimento(string codice)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codice))
                {
                    Application.Current.Dispatcher.Invoke(() => DescrizioneControMovimento = string.Empty);
                    return;
                }

                var causale = await _causaleService.GetCausaleMovimentoByCodeAsync(codice);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    DescrizioneControMovimento = causale?.Descrizione ?? string.Empty;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore in AggiornaCausaleControMovimento: {ex.Message}");
            }
        }

        private void SuspendIsModified(Action action)
        {
            _suspendIsModified = true;
            try
            {
                action();
            }
            finally
            {
                _suspendIsModified = false;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}