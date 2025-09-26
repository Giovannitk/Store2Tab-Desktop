using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Store2Tab.Core.Services;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.ViewModels
{
    public class TipiAttivitaViewModel : INotifyPropertyChanged
    {
        private readonly ITipiAttivitaService _service;

        public TipiAttivitaViewModel()
        {
            // Costruttore di comodo senza DI: utilizza la OnConfiguring di AppDbContext
            var factory = new DefaultAppDbContextFactory();
            _service = new TipiAttivitaService(factory);
            TipiAttivita = new ObservableCollection<TipiAttivita>();
            ElementiSelezionati = new ObservableCollection<TipiAttivita>();
            _ = CaricaAsync();
        }

        public TipiAttivitaViewModel(ITipiAttivitaService service)
        {
            _service = service;
            TipiAttivita = new ObservableCollection<TipiAttivita>();
            ElementiSelezionati = new ObservableCollection<TipiAttivita>();
            _ = CaricaAsync();
        }

        private ObservableCollection<TipiAttivita> _tipiAttivita = new ObservableCollection<TipiAttivita>();
        public ObservableCollection<TipiAttivita> TipiAttivita
        {
            get => _tipiAttivita;
            set { _tipiAttivita = value; OnPropertyChanged(); }
        }

        private TipiAttivita? _tipoAttivitaSelezionato;
        public TipiAttivita? TipoAttivitaSelezionato
        {
            get => _tipoAttivitaSelezionato;
            set
            {
                _tipoAttivitaSelezionato = value;

                // Aggiorna il codice proposto quando selezioni un elemento esistente
                if (_tipoAttivitaSelezionato != null)
                {
                    // Assicura che il campo obbligatorio sia inizializzato
                    if (_tipoAttivitaSelezionato.Attivita == null)
                        _tipoAttivitaSelezionato.Attivita = string.Empty;

                    // Aggiorna il codice proposto con l'ID dell'elemento selezionato
                    CodiceProposto = _tipoAttivitaSelezionato.IdAnagraficaAttivita;
                }

                OnPropertyChanged();
            }
        }

        // Filtri di ricerca (come nel VB6)
        private string _filtroCodice = string.Empty;
        public string FiltroCodice
        {
            get => _filtroCodice;
            set { _filtroCodice = value; OnPropertyChanged(); }
        }

        private string _filtroAttivita = string.Empty;
        public string FiltroAttivita
        {
            get => _filtroAttivita;
            set { _filtroAttivita = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private short _codiceProposto;
        public short CodiceProposto
        {
            get => _codiceProposto;
            set { _codiceProposto = value; OnPropertyChanged(); }
        }

        // Flag per gestire la modalità salvataggio (simile al pctF2.Tag nel VB6)
        private bool _isInEditMode;
        public bool IsInEditMode
        {
            get => _isInEditMode;
            set { _isInEditMode = value; OnPropertyChanged(); }
        }

        public async Task CaricaAsync()
        {
            try
            {
                IsLoading = true;
                var list = await _service.GetAllAsync();
                TipiAttivita.Clear();
                foreach (var t in list) TipiAttivita.Add(t);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore caricamento tipi attività: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // F1 - Nuovo (come pctF1_Click nel VB6)
        public async void NuovoTipoAttivita()
        {
            // Controllo se ci sono modifiche da salvare (come nel VB6)
            if (IsInEditMode)
            {
                var risposta = MessageBox.Show("SALVARE LE MODIFICHE APPORTATE ALLA SCHEDA CORRENTE?",
                    "Conferma", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

                if (risposta == MessageBoxResult.Yes)
                {
                    await SalvaTipoAttivitaAsync();
                    if (IsInEditMode) return; // Se il salvataggio ha fallito, non continuare
                }
                else if (risposta == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            try
            {
                var next = await _service.GetNextIdAsync();
                CodiceProposto = next;
                TipoAttivitaSelezionato = new TipiAttivita
                {
                    IdAnagraficaAttivita = 0, // Non impostato per insert
                    Attivita = string.Empty
                };
                IsInEditMode = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore creazione nuovo tipo attività: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // F2 - Salva (come pctF2_Click nel VB6)
        public async void SalvaTipoAttivita()
        {
            await SalvaTipoAttivitaAsync();
        }

        private async Task<bool> SalvaTipoAttivitaAsync()
        {
            if (TipoAttivitaSelezionato == null) return false;

            try
            {
                // Validazione (come nel VB6 Salva function)
                if (string.IsNullOrWhiteSpace(TipoAttivitaSelezionato.Attivita))
                {
                    MessageBox.Show("IMPOSSIBILE SALVARE!\nIndica l'Attività (denominazione).",
                        "Validazione", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                IsLoading = true;
                var isNuovo = TipoAttivitaSelezionato.IdAnagraficaAttivita == 0;
                var saved = await _service.SaveAsync(TipoAttivitaSelezionato);

                if (isNuovo)
                {
                    // Aggiungi alla lista (come nel VB6 flexG.AddItem)
                    TipiAttivita.Add(saved);
                }
                else
                {
                    // Aggiorna l'elemento esistente
                    var index = TipiAttivita.IndexOf(TipoAttivitaSelezionato);
                    if (index >= 0)
                    {
                        TipiAttivita[index] = saved;
                    }
                }

                TipoAttivitaSelezionato = saved;
                CodiceProposto = saved.IdAnagraficaAttivita;
                IsInEditMode = false;

                MessageBox.Show("Tipo attività salvato.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"IMPOSSIBILE SALVARE!\n{ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // F3 - Cerca (come pctF3_Click nel VB6)
        public async void CercaTipiAttivita()
        {
            // Controllo se ci sono modifiche da salvare (come Ricerca() nel VB6)
            if (IsInEditMode)
            {
                var risposta = MessageBox.Show("SALVARE LE MODIFICHE APPORTATE ALLA SCHEDA CORRENTE?",
                    "Conferma", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

                if (risposta == MessageBoxResult.Yes)
                {
                    await SalvaTipoAttivitaAsync();
                    if (IsInEditMode) return; // Se il salvataggio ha fallito, non continuare
                }
                else if (risposta == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            try
            {
                IsLoading = true;
                var list = await _service.GetAllAsync(FiltroCodice, FiltroAttivita);
                TipiAttivita.Clear();
                foreach (var t in list) TipiAttivita.Add(t);

                // Reset selezione dopo la ricerca
                TipoAttivitaSelezionato = null;
                CodiceProposto = 0;
                IsInEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore ricerca: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // F4 - Cancella (come pctF4_Click nel VB6)
        public async void CancellaTipoAttivita()
        {
            if (TipoAttivitaSelezionato == null) return;

            var conferma = MessageBox.Show("CONFERMI LA CANCELLAZIONE DELLA SCHEDA CORRENTE?",
                "Conferma Cancellazione", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);

            if (conferma != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                var ok = await _service.DeleteAsync(TipoAttivitaSelezionato.IdAnagraficaAttivita);
                if (ok)
                {
                    TipiAttivita.Remove(TipoAttivitaSelezionato);
                    TipoAttivitaSelezionato = null;
                    CodiceProposto = 0;
                    IsInEditMode = false;
                    MessageBox.Show("Tipo attività cancellato.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la cancellazione: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Aggiungi questa proprietà dopo TipoAttivitaSelezionato
        private ObservableCollection<TipiAttivita> _elementiSelezionati = new ObservableCollection<TipiAttivita>();
        public ObservableCollection<TipiAttivita> ElementiSelezionati
        {
            get => _elementiSelezionati;
            set { _elementiSelezionati = value; OnPropertyChanged(); }
        }

        // Aggiungi questo metodo dopo CancellaTipoAttivita()
        public async void CancellaTipiAttivitaMultiple(List<TipiAttivita> tipiSelezionati)
        {
            if (tipiSelezionati == null || tipiSelezionati.Count == 0)
            {
                MessageBox.Show("Nessun tipo di attività selezionato per la cancellazione.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                var risultatiCancellazione = new List<string>();
                var tipiCancellati = new List<TipiAttivita>();
                var tipiNonCancellati = new List<TipiAttivita>();

                foreach (var tipo in tipiSelezionati)
                {
                    var conferma = MessageBox.Show(
                        $"Cancellare il tipo di attività:\n\n" +
                        $"Codice: {tipo.IdAnagraficaAttivita}\n" +
                        $"Attività: {tipo.Attivita}\n\n" +
                        $"Rimanenti da confermare: {tipiSelezionati.Count - tipiSelezionati.IndexOf(tipo) - 1}",
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
                            if (tipo.IdAnagraficaAttivita > 0)
                            {
                                await _service.DeleteAsync(tipo.IdAnagraficaAttivita);
                                tipiCancellati.Add(tipo);
                                risultatiCancellazione.Add($"✓ {tipo.Attivita} - Cancellato");
                            }
                            else
                            {
                                risultatiCancellazione.Add($"✗ {tipo.Attivita} - ID non valido");
                                tipiNonCancellati.Add(tipo);
                            }
                        }
                        catch (Exception ex)
                        {
                            risultatiCancellazione.Add($"✗ {tipo.Attivita} - Errore: {ex.Message}");
                            tipiNonCancellati.Add(tipo);
                        }
                    }
                    else
                    {
                        risultatiCancellazione.Add($"○ {tipo.Attivita} - Saltato");
                        tipiNonCancellati.Add(tipo);
                    }
                }

                // Rimuove i tipi cancellati dalla collezione
                foreach (var tipo in tipiCancellati)
                {
                    TipiAttivita.Remove(tipo);
                }

                // Reset della selezione
                TipoAttivitaSelezionato = null;
                ElementiSelezionati.Clear();
                CodiceProposto = 0;
                IsInEditMode = false;

                // Mostra il riepilogo
                var messaggio = "Riepilogo cancellazione multipla:\n\n" +
                               string.Join("\n", risultatiCancellazione) +
                               $"\n\nTipi di attività cancellati: {tipiCancellati.Count}\n" +
                               $"Tipi saltati/errori: {tipiNonCancellati.Count}";

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

        // F8 - Annulla (come pctF8_Click nel VB6)
        public async void AnnullaModifiche()
        {
            if (!IsInEditMode) return;

            try
            {
                if (TipoAttivitaSelezionato != null && TipoAttivitaSelezionato.IdAnagraficaAttivita > 0)
                {
                    // Ricarica l'elemento dal database (come flexG_RowColChange nel VB6)
                    var fresh = await _service.GetByIdAsync(TipoAttivitaSelezionato.IdAnagraficaAttivita);
                    if (fresh != null)
                    {
                        TipoAttivitaSelezionato = fresh;
                        CodiceProposto = fresh.IdAnagraficaAttivita;
                    }
                }
                else
                {
                    // Se è un nuovo elemento, resetta tutto (come SvuotaCampi nel VB6)
                    TipoAttivitaSelezionato = null;
                    CodiceProposto = 0;
                }

                IsInEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore annullamento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metodo per la selezione dalla ListBox (simile a flexG_RowColChange nel VB6)
        public void SelezionaTipoAttivita(string codice, string descrizione)
        {
            // Trova il tipo attività corrispondente nella collezione
            var tipo = TipiAttivita.FirstOrDefault(t =>
                t.IdAnagraficaAttivita.ToString() == codice &&
                t.Attivita == descrizione);

            if (tipo != null)
            {
                TipoAttivitaSelezionato = tipo;
                IsInEditMode = false; // Reset della modalità edit quando si seleziona un elemento esistente
            }
        }

        public void SelezionaTipoAttivita(TipiAttivita tipoAttivita)
        {
            TipoAttivitaSelezionato = tipoAttivita;
            IsInEditMode = false; // Reset della modalità edit quando si seleziona un elemento esistente
        }

        // Metodo chiamato quando l'utente modifica il campo descrizione (simile a txtAnagraficaAttivita_Change nel VB6)
        public void OnAttivitaChanged()
        {
            if (!IsLoading && TipoAttivitaSelezionato != null)
            {
                IsInEditMode = true;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}