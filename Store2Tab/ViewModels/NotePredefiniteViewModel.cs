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
    public class NotePredefiniteViewModel : INotifyPropertyChanged
    {
        private readonly INotaDocumentoService _service;
        private NotaDocumento? _notaOriginale; // Per ripristinare in caso di annullamento

        #region Costruttore
        public NotePredefiniteViewModel()
        {
            var factory = new DefaultAppDbContextFactory();
            _service = new NotaDocumentoService(factory);
            NoteDocumento = new ObservableCollection<Data.Models.NotaDocumento>();
            ElementiSelezionati = new ObservableCollection<Data.Models.NotaDocumento>();
            _ = CaricaAsync();
        }

        public NotePredefiniteViewModel(INotaDocumentoService service)
        {
            _service = service;
            NoteDocumento = new ObservableCollection<Data.Models.NotaDocumento>();
            ElementiSelezionati = new ObservableCollection<Data.Models.NotaDocumento>();
            _ = CaricaAsync();
        }
        #endregion

        #region Proprietà e campi
        private ObservableCollection<Data.Models.NotaDocumento> _noteDocumento = new ObservableCollection<NotaDocumento>();
        public ObservableCollection<Data.Models.NotaDocumento> NoteDocumento
        {
            get => _noteDocumento;
            set { _noteDocumento = value; OnPropertyChanged(); }
        }

        private NotaDocumento? _notaSelezionata;
        public NotaDocumento? NotaSelezionata
        {
            get => _notaSelezionata;
            set
            {
                // CORREZIONE: Non bloccare la selezione se in modalità edit
                // Salva invece la nota originale per poter annullare
                if (_notaSelezionata != value && IsInEditMode)
                {
                    var result = MessageBox.Show(
                        "Ci sono modifiche non salvate. Vuoi salvarle?",
                        "Conferma",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Task.Run(async () => await SalvaNotaDocumentoAsync()).Wait();
                        if (IsInEditMode) return; // Se il salvataggio non è riuscito, esco
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return; // Non cambiare selezione
                    }
                    else // No - annulla modifiche
                    {
                        IsInEditMode = false;
                    }
                }

                _notaSelezionata = value;

                if (_notaSelezionata != null)
                {
                    if (_notaSelezionata.Nota == null)
                        _notaSelezionata.Nota = string.Empty;

                    CodiceProposto = _notaSelezionata.IdNotaDocumento;

                    // Salva una copia per poter annullare
                    _notaOriginale = new NotaDocumento
                    {
                        IdNotaDocumento = _notaSelezionata.IdNotaDocumento,
                        Nota = _notaSelezionata.Nota
                    };
                }
                else
                {
                    CodiceProposto = 0;
                    _notaOriginale = null;
                }

                OnPropertyChanged();
            }
        }

        private ObservableCollection<Data.Models.NotaDocumento> _elementiSelezionati = new ObservableCollection<NotaDocumento>();
        public ObservableCollection<Data.Models.NotaDocumento> ElementiSelezionati
        {
            get => _elementiSelezionati;
            set { _elementiSelezionati = value; OnPropertyChanged(); }
        }

        // Filtri ricerca
        private string? _filtroCodice = string.Empty;
        public string? FiltroCodice
        {
            get => _filtroCodice;
            set { _filtroCodice = value; OnPropertyChanged(); }
        }

        private string? _filtroNota = string.Empty;
        public string? FiltroNota
        {
            get => _filtroNota;
            set { _filtroNota = value; OnPropertyChanged(); }
        }

        private short _codiceProposto;
        public short CodiceProposto
        {
            get => _codiceProposto;
            set { _codiceProposto = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private bool _isInEditMode;
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
                var lista = await _service.GetAllAsync(FiltroCodice, FiltroNota);
                NoteDocumento.Clear();
                foreach (var nota in lista)
                {
                    NoteDocumento.Add(nota);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il caricamento delle note documento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // F1 - Nuovo
        public async void NuovaNotaDocumento()
        {
            if (IsInEditMode)
            {
                var result = MessageBox.Show("Ci sono modifiche non salvate. Vuoi salvarle prima di creare una nuova nota documento?", "Conferma", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await SalvaNotaDocumentoAsync();
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
                NotaSelezionata = new NotaDocumento
                {
                    IdNotaDocumento = 0,
                    Nota = string.Empty
                };
                IsInEditMode = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la creazione di una nuova nota documento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // F2 - Salva
        public async Task<bool> SalvaNotaDocumentoAsync()
        {
            if (NotaSelezionata == null)
            {
                MessageBox.Show("Nessuna nota documento selezionata da salvare.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(NotaSelezionata.Nota))
                {
                    MessageBox.Show("La nota documento non può essere vuota.", "Errore di validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                IsLoading = true;
                var isNuovo = NotaSelezionata.IdNotaDocumento == 0;
                var saved = await _service.SaveAsync(NotaSelezionata);

                if (isNuovo)
                {
                    NoteDocumento.Add(saved);
                }
                else
                {
                    var index = NoteDocumento.IndexOf(NoteDocumento.First(n => n.IdNotaDocumento == saved.IdNotaDocumento));
                    if (index >= 0)
                    {
                        NoteDocumento[index] = saved;
                    }
                }

                NotaSelezionata = saved;
                CodiceProposto = saved.IdNotaDocumento;
                IsInEditMode = false;

                MessageBox.Show("Nota documento salvata con successo.", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il salvataggio della nota documento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // F3 - Cerca
        public async void CercaNoteDocumento()
        {
            if (IsInEditMode)
            {
                var result = MessageBox.Show("Ci sono modifiche non salvate. Vuoi salvarle prima di effettuare una ricerca?", "Conferma", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await SalvaNotaDocumentoAsync();
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
                var list = await _service.GetAllAsync(FiltroCodice, FiltroNota);
                NoteDocumento.Clear();
                foreach (var nota in list)
                {
                    NoteDocumento.Add(nota);
                }

                NotaSelezionata = null;
                CodiceProposto = 0;
                IsInEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la ricerca delle note documento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // F4 - Elimina
        public async void EliminaNotaDocumento()
        {
            if (NotaSelezionata == null || NotaSelezionata.IdNotaDocumento == 0)
            {
                MessageBox.Show("Nessuna nota documento selezionata da eliminare.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Sei sicuro di voler eliminare la nota documento con ID {NotaSelezionata.IdNotaDocumento}?", "Conferma eliminazione", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoading = true;
                var success = await _service.DeleteAsync(NotaSelezionata.IdNotaDocumento);
                if (success)
                {
                    NoteDocumento.Remove(NotaSelezionata);
                    NotaSelezionata = null;
                    CodiceProposto = 0;
                    IsInEditMode = false;
                    MessageBox.Show("Nota documento eliminata con successo.", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("La nota documento non è stata trovata o non è stata eliminata.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'eliminazione della nota documento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // F5 - Elimina multipla
        public async void EliminaNoteDocumentoMultiple(List<NotaDocumento> noteSelezionate)
        {
            if (noteSelezionate == null || noteSelezionate.Count == 0)
            {
                MessageBox.Show("Nessuna nota documento selezionata da eliminare.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                IsLoading = true;
                var risultatiCancellazione = new List<string>();
                var noteCancellate = new List<NotaDocumento>();
                var noteNonCancellate = new List<NotaDocumento>();

                foreach (var nota in noteSelezionate)
                {
                    var conferma = MessageBox.Show(
                        $"Cancellare il tipo di note documento:\n\n" +
                        $"Codice: {nota.IdNotaDocumento}\n" +
                        $"Nota: {nota.Nota}\n\n" +
                        $"Rimanenti da confermare: {noteSelezionate.Count - noteSelezionate.IndexOf(nota) - 1}",
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
                            if (nota.IdNotaDocumento > 0)
                            {
                                await _service.DeleteAsync(nota.IdNotaDocumento);
                                noteCancellate.Add(nota);
                                risultatiCancellazione.Add($"✓ {nota.Nota} - Cancellato");
                            }
                            else
                            {
                                risultatiCancellazione.Add($"✗ {nota.Nota} - ID non valido");
                                noteNonCancellate.Add(nota);
                            }
                        }
                        catch (Exception ex)
                        {
                            risultatiCancellazione.Add($"✗ {nota.Nota} - Errore: {ex.Message}");
                            noteNonCancellate.Add(nota);
                        }
                    }
                    else
                    {
                        risultatiCancellazione.Add($"○ {nota.Nota} - Saltata");
                        noteNonCancellate.Add(nota);
                    }
                }

                // Rimuove le note cancellate dalla collezione
                foreach (var nota in noteCancellate)
                {
                    NoteDocumento.Remove(nota);
                }

                // Reset della selezione
                NotaSelezionata = null;
                ElementiSelezionati.Clear();
                CodiceProposto = 0;
                IsInEditMode = false;

                // Ricarica per aggiornare la lista
                await CaricaAsync();

                // Mostra il riepilogo
                var messaggio = "Riepilogo cancellazione multipla:\n\n" +
                               string.Join("\n", risultatiCancellazione) +
                               $"\n\nNote cancellate: {noteCancellate.Count}\n" +
                               $"Note saltate/errori: {noteNonCancellate.Count}";

                MessageBox.Show(messaggio, "Riepilogo Cancellazione",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'eliminazione delle note documento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // F8 - Annulla
        public void AnnullaModifiche()
        {
            if (!IsInEditMode)
                return;

            try
            {
                if (NotaSelezionata != null && _notaOriginale != null)
                {
                    // Ripristina i valori originali
                    NotaSelezionata.Nota = _notaOriginale.Nota;
                    CodiceProposto = _notaOriginale.IdNotaDocumento;

                    // Forza l'aggiornamento dell'UI
                    OnPropertyChanged(nameof(NotaSelezionata));
                }
                else if (NotaSelezionata != null && NotaSelezionata.IdNotaDocumento == 0)
                {
                    // Nuova nota non salvata, resetto i campi
                    NotaSelezionata = null;
                    CodiceProposto = 0;
                }

                IsInEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'annullamento delle modifiche: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Metodo per la selezione dalla griglia (simile a TipiAttivita)
        public void SelezionaNotaDocumento(NotaDocumento nota)
        {
            NotaSelezionata = nota;
            IsInEditMode = false; // Reset della modalità edit quando si seleziona un elemento esistente
        }

        public void OnNotaModificata()
        {
            if (!IsLoading && NotaSelezionata != null && !IsInEditMode)
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