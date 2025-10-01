using Store2Tab.Core.Interfaces;
using Store2Tab.Core.Services;
using Store2Tab.Data;
using Store2Tab.Data.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class ProtocolliViewModel : INotifyPropertyChanged
    {
        private readonly IProtocolloService _service;
        private Protocollo? _protocolloOriginale;

        // Flag per indicare operazione di cancellazione multipla in corso
        private bool _isMultiDeleteInProgress = false;

        #region Costruttore
        public ProtocolliViewModel()
        {
            var factory = new DefaultAppDbContextFactory();
            _service = new ProtocolloService(factory);
            Protocolli = new ObservableCollection<Protocollo>();
            ElementiSelezionati = new ObservableCollection<Protocollo>();
            _ = CaricaAsync();
        }

        public ProtocolliViewModel(IProtocolloService service)
        {
            _service = service;
            Protocolli = new ObservableCollection<Protocollo>();
            ElementiSelezionati = new ObservableCollection<Protocollo>();
            _ = CaricaAsync();
        }
        #endregion

        #region Proprietà e campi
        private ObservableCollection<Protocollo> _protocolli;
        public ObservableCollection<Protocollo> Protocolli
        {
            get => _protocolli;
            set { _protocolli = value; OnPropertyChanged(); }
        }

        private Protocollo? _protocolloSelezionato;
        public Protocollo? ProtocolloSelezionato
        {
            get => _protocolloSelezionato;
            set
            {
                // Ignora il controllo delle modifiche durante la cancellazione multipla
                if (_isMultiDeleteInProgress)
                {
                    _protocolloSelezionato = value;
                    OnPropertyChanged();
                    return;
                }

                if (_protocolloSelezionato != value && IsInEditMode)
                {
                    var result = MessageBox.Show(
                        "Ci sono modifiche non salvate. Vuoi salvarle?",
                        "Conferma",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Task.Run(async () => await SalvaProtocolloAsync()).Wait();
                        if (IsInEditMode) return;
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        IsInEditMode = false;
                    }
                }

                _protocolloSelezionato = value;

                if (_protocolloSelezionato != null)
                {
                    if (_protocolloSelezionato.NomeProtocollo == null)
                        _protocolloSelezionato.NomeProtocollo = string.Empty;

                    CodiceProposto = _protocolloSelezionato.IdProtocollo;

                    _protocolloOriginale = new Protocollo
                    {
                        IdProtocollo = _protocolloSelezionato.IdProtocollo,
                        NomeProtocollo = _protocolloSelezionato.NomeProtocollo
                    };
                }
                else
                {
                    CodiceProposto = null;
                    _protocolloOriginale = null;
                }

                OnPropertyChanged();
            }
        }

        private ObservableCollection<Protocollo> _elementiSelezionati;
        public ObservableCollection<Protocollo> ElementiSelezionati
        {
            get => _elementiSelezionati;
            set { _elementiSelezionati = value; OnPropertyChanged(); }
        }

        private string? _filtroCodice = string.Empty;
        public string? FiltroCodice
        {
            get => _filtroCodice;
            set { _filtroCodice = value; OnPropertyChanged(); }
        }

        private string? _filtroProtocollo = string.Empty;
        public string? FiltroProtocollo
        {
            get => _filtroProtocollo;
            set { _filtroProtocollo = value; OnPropertyChanged(); }
        }

        private short? _codiceProposto;
        public short? CodiceProposto
        {
            get => _codiceProposto;
            set { _codiceProposto = value; OnPropertyChanged(); }
        }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get => _IsLoading;
            set { _IsLoading = value; OnPropertyChanged(); }
        }

        private bool _isInEditMode = false;
        public bool IsInEditMode
        {
            get => _isInEditMode;
            set { _isInEditMode = value; OnPropertyChanged(); }
        }
        #endregion

        #region Comandi e metodi
        public async Task CaricaAsync()
        {
            try
            {
                IsLoading = true;
                var lista = await _service.GetAllAsync(FiltroCodice, FiltroProtocollo);

                Protocolli.Clear();
                foreach (var item in lista)
                {
                    Protocolli.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il caricamento dei protocolli: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void NuovoProtocollo()
        {
            if (IsInEditMode)
            {
                var result = MessageBox.Show("Ci sono modifiche non salvate. Vuoi salvarle prima di creare un nuovo protocollo?", "Conferma", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await SalvaProtocolloAsync();
                    if (IsInEditMode) return;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            try
            {
                var next = await _service.GetNextIdAsync();
                CodiceProposto = next;
                ProtocolloSelezionato = new Protocollo
                {
                    IdProtocollo = 0,
                    NomeProtocollo = string.Empty
                };
                IsInEditMode = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la creazione di un nuovo protocollo: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task<bool> SalvaProtocolloAsync()
        {
            if (ProtocolloSelezionato == null)
            {
                MessageBox.Show("Nessun protocollo selezionato da salvare.", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(ProtocolloSelezionato.NomeProtocollo))
                {
                    MessageBox.Show("Il protocollo non può essere vuoto.", "Errore di validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                IsLoading = true;
                var isNuovo = ProtocolloSelezionato.IdProtocollo == 0;
                var saved = await _service.SaveAsync(ProtocolloSelezionato);

                if (isNuovo)
                {
                    Protocolli.Add(saved);
                }
                else
                {
                    var existing = Protocolli.FirstOrDefault(n => n.IdProtocollo == saved.IdProtocollo);
                    if (existing != null)
                    {
                        existing.NomeProtocollo = saved.NomeProtocollo;
                    }
                }

                ProtocolloSelezionato = saved;
                _protocolloOriginale = new Protocollo
                {
                    IdProtocollo = saved.IdProtocollo,
                    NomeProtocollo = saved.NomeProtocollo
                };

                CodiceProposto = saved.IdProtocollo;
                IsInEditMode = false;

                MessageBox.Show("Protocollo salvato con successo.", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il salvataggio del protocollo: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void CercaProtocollo()
        {
            if (IsInEditMode)
            {
                var result = MessageBox.Show("Ci sono modifiche non salvate. Vuoi salvarle prima di cercare?", "Conferma", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await SalvaProtocolloAsync();
                    if (IsInEditMode) return;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            try
            {
                IsLoading = true;
                var list = await _service.GetAllAsync(FiltroCodice, FiltroProtocollo);
                Protocolli.Clear();
                foreach (var nota in list)
                {
                    Protocolli.Add(nota);
                }

                ProtocolloSelezionato = null;
                CodiceProposto = 0;
                IsInEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la ricerca dei protocolli: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void EliminaProtocollo()
        {
            if (ProtocolloSelezionato == null || ProtocolloSelezionato.IdProtocollo == 0)
            {
                MessageBox.Show("Nessun protocollo selezionato da eliminare.", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Sei sicuro di voler eliminare il protocollo '{ProtocolloSelezionato.NomeProtocollo}'?", "Conferma eliminazione", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoading = true;
                var success = await _service.DeleteAsync(ProtocolloSelezionato.IdProtocollo);
                if (success)
                {
                    Protocolli.Remove(ProtocolloSelezionato);
                    ProtocolloSelezionato = null;
                    CodiceProposto = 0;
                    IsInEditMode = false;
                    MessageBox.Show("Protocollo eliminato con successo.", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Il protocollo non è stato trovato o non è stato possibile eliminarlo.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'eliminazione del protocollo: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void EliminaProtocolliMultipli(List<Protocollo> protocolliSelezionati)
        {
            if (protocolliSelezionati == null || protocolliSelezionati.Count == 0)
            {
                MessageBox.Show("Nessun protocollo selezionato da eliminare.", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // IMPORTANTE: Disattiva la modalità edit e attiva il flag di cancellazione multipla
            IsInEditMode = false;
            _protocolloOriginale = null;
            _isMultiDeleteInProgress = true;

            try
            {
                var result = MessageBox.Show(
                    $"Sei sicuro di voler eliminare {protocolliSelezionati.Count} protocolli selezionati?",
                    "Conferma eliminazione",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                IsLoading = true;
                var risultatiCancellazione = new List<string>();
                var protocolliDaRimuovere = new List<Protocollo>();

                foreach (var protocollo in protocolliSelezionati)
                {
                    var conferma = MessageBox.Show(
                        $"Cancellare il protocollo:\n\n" +
                        $"Codice: {protocollo.IdProtocollo}\n" +
                        $"Protocollo: {protocollo.NomeProtocollo}\n\n" +
                        $"Rimanenti da confermare: {protocolliSelezionati.Count - protocolliSelezionati.IndexOf(protocollo) - 1}",
                        "Conferma Cancellazione",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (conferma == MessageBoxResult.Cancel)
                    {
                        risultatiCancellazione.Add("Operazione annullata dall'utente.");
                        break;
                    }
                    else if (conferma == MessageBoxResult.Yes)
                    {
                        try
                        {
                            if (protocollo.IdProtocollo > 0)
                            {
                                await _service.DeleteAsync(protocollo.IdProtocollo);
                                protocolliDaRimuovere.Add(protocollo);
                                risultatiCancellazione.Add($"✓ {protocollo.NomeProtocollo} - Cancellato");
                            }
                            else
                            {
                                risultatiCancellazione.Add($"✗ {protocollo.NomeProtocollo} - ID non valido");
                            }
                        }
                        catch (Exception ex)
                        {
                            risultatiCancellazione.Add($"✗ {protocollo.NomeProtocollo} - Errore: {ex.Message}");
                        }
                    }
                    else
                    {
                        risultatiCancellazione.Add($"○ {protocollo.NomeProtocollo} - Saltato");
                    }
                }

                // Rimuove i protocolli dalla collezione
                foreach (var protocollo in protocolliDaRimuovere)
                {
                    var itemToRemove = Protocolli.FirstOrDefault(p => p.IdProtocollo == protocollo.IdProtocollo);
                    if (itemToRemove != null)
                    {
                        Protocolli.Remove(itemToRemove);
                    }
                }

                // Reset della selezione
                ProtocolloSelezionato = null;
                ElementiSelezionati.Clear();
                CodiceProposto = 0;
                IsInEditMode = false;
                _protocolloOriginale = null;

                // Mostra il riepilogo
                var messaggio = "Riepilogo cancellazione multipla:\n\n" +
                               string.Join("\n", risultatiCancellazione) +
                               $"\n\nProtocolli cancellati: {protocolliDaRimuovere.Count}\n" +
                               $"Protocolli saltati/errori: {protocolliSelezionati.Count - protocolliDaRimuovere.Count}";

                MessageBox.Show(messaggio, "Riepilogo Cancellazione",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'eliminazione dei protocolli: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
                _isMultiDeleteInProgress = false;
            }
        }

        public void AnnullaModifiche()
        {
            if (!IsInEditMode)
                return;

            try
            {
                if (ProtocolloSelezionato != null && _protocolloOriginale != null)
                {
                    ProtocolloSelezionato.NomeProtocollo = _protocolloOriginale.NomeProtocollo;
                    CodiceProposto = _protocolloOriginale.IdProtocollo;
                    OnPropertyChanged(nameof(ProtocolloSelezionato));
                }
                else if (ProtocolloSelezionato != null && ProtocolloSelezionato.IdProtocollo == 0)
                {
                    ProtocolloSelezionato = null;
                    CodiceProposto = 0;
                }

                IsInEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'annullamento delle modifiche: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SelezionaProtocollo(Protocollo protocollo)
        {
            ProtocolloSelezionato = protocollo;
            IsInEditMode = false;
        }

        public void OnProtocolloModificato()
        {
            if (!IsInEditMode && ProtocolloSelezionato != null)
            {
                IsInEditMode = true;
            }
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}