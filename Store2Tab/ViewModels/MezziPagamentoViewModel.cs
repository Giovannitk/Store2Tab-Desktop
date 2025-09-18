using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Store2Tab.Core.Services;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.ViewModels
{
    public class MezziPagamentoViewModel : INotifyPropertyChanged
    {
        private readonly IPagamentoMezzoService _service;

        public MezziPagamentoViewModel()
        {
            // Costruttore di comodo senza DI: utilizza la OnConfiguring di AppDbContext
            var factory = new DefaultAppDbContextFactory();
            _service = new PagamentoMezzoService(factory);
            Mezzi = new ObservableCollection<PagamentoMezzo>();
            ElementiSelezionati = new ObservableCollection<PagamentoMezzo>();
            _ = CaricaAsync();
        }

        public MezziPagamentoViewModel(IPagamentoMezzoService service)
        {
            _service = service;
            Mezzi = new ObservableCollection<PagamentoMezzo>();
            ElementiSelezionati = new ObservableCollection<PagamentoMezzo>();
            _ = CaricaAsync();
        }

        private ObservableCollection<PagamentoMezzo> _mezzi = new ObservableCollection<PagamentoMezzo>();
        public ObservableCollection<PagamentoMezzo> Mezzi
        {
            get => _mezzi;
            set { _mezzi = value; OnPropertyChanged(); }
        }

        private PagamentoMezzo? _selezionato;
        public PagamentoMezzo? MezzoSelezionato
        {
            get => _selezionato;
            set
            {
                _selezionato = value;

                // FIX: Aggiorna il codice proposto quando viene selezionato un elemento esistente
                if (_selezionato != null)
                {
                    // Assicura che tutti i campi obbligatori siano inizializzati
                    if (_selezionato.pagamentoMezzo == null) _selezionato.pagamentoMezzo = string.Empty;
                    if (_selezionato.Gruppo == null) _selezionato.Gruppo = string.Empty;
                    if (_selezionato.FE_ModPag == null) _selezionato.FE_ModPag = string.Empty;

                    // Aggiorna il codice proposto con l'ID dell'elemento selezionato
                    CodiceProposto = _selezionato.IdPagamentoMezzo;
                }

                OnPropertyChanged();
            }
        }

        // Proprietà per la selezione multipla
        private ObservableCollection<PagamentoMezzo> _elementiSelezionati = new ObservableCollection<PagamentoMezzo>();
        public ObservableCollection<PagamentoMezzo> ElementiSelezionati
        {
            get => _elementiSelezionati;
            set { _elementiSelezionati = value; OnPropertyChanged(); }
        }

        private string _filtroCodice = string.Empty;
        public string FiltroCodice
        {
            get => _filtroCodice;
            set { _filtroCodice = value; OnPropertyChanged(); }
        }

        private string _filtroDescrizione = string.Empty;
        public string FiltroDescrizione
        {
            get => _filtroDescrizione;
            set { _filtroDescrizione = value; OnPropertyChanged(); }
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

        public async Task CaricaAsync()
        {
            try
            {
                IsLoading = true;
                var list = await _service.GetAllAsync();
                Mezzi.Clear();
                foreach (var m in list) Mezzi.Add(m);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore caricamento mezzi pagamento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void CancellaMezzoPagamento()
        {
            if (MezzoSelezionato == null) return;

            var conferma = MessageBox.Show(
                $"Cancellare il mezzo di pagamento:\n\n" +
                $"Codice: {MezzoSelezionato.IdPagamentoMezzo}\n" +
                $"Descrizione: {MezzoSelezionato.pagamentoMezzo}\n" +
                $"Gruppo: {MezzoSelezionato.Gruppo}",
                "Conferma Cancellazione",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (conferma != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                var ok = await _service.DeleteAsync(MezzoSelezionato.IdPagamentoMezzo);
                if (ok)
                {
                    Mezzi.Remove(MezzoSelezionato);
                    MezzoSelezionato = null;
                    CodiceProposto = 0;
                    MessageBox.Show("Mezzo di pagamento cancellato.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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

        // Nuova funzionalità: Cancellazione multipla
        public async void CancellaMezziPagamentoMultipli(List<PagamentoMezzo> mezziSelezionati)
        {
            if (mezziSelezionati == null || mezziSelezionati.Count == 0)
            {
                MessageBox.Show("Nessun mezzo di pagamento selezionato per la cancellazione.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                var risultatiCancellazione = new List<string>();
                var mezziCancellati = new List<PagamentoMezzo>();
                var mezziNonCancellati = new List<PagamentoMezzo>();

                foreach (var mezzo in mezziSelezionati)
                {
                    // Chiede conferma per ogni singolo mezzo
                    var conferma = MessageBox.Show(
                        $"Cancellare il mezzo di pagamento:\n\n" +
                        $"Codice: {mezzo.IdPagamentoMezzo}\n" +
                        $"Descrizione: {mezzo.pagamentoMezzo}\n" +
                        $"Gruppo: {mezzo.Gruppo}\n\n" +
                        $"Rimanenti da confermare: {mezziSelezionati.Count - mezziSelezionati.IndexOf(mezzo) - 1}",
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
                            if (mezzo.IdPagamentoMezzo > 0)
                            {
                                await _service.DeleteAsync(mezzo.IdPagamentoMezzo);
                                mezziCancellati.Add(mezzo);
                                risultatiCancellazione.Add($"✓ {mezzo.pagamentoMezzo} - Cancellato");
                            }
                            else
                            {
                                risultatiCancellazione.Add($"✗ {mezzo.pagamentoMezzo} - ID non valido");
                                mezziNonCancellati.Add(mezzo);
                            }
                        }
                        catch (Exception ex)
                        {
                            risultatiCancellazione.Add($"✗ {mezzo.pagamentoMezzo} - Errore: {ex.Message}");
                            mezziNonCancellati.Add(mezzo);
                        }
                    }
                    else
                    {
                        // L'utente ha detto No per questo mezzo
                        risultatiCancellazione.Add($"○ {mezzo.pagamentoMezzo} - Saltato");
                        mezziNonCancellati.Add(mezzo);
                    }
                }

                // Rimuove i mezzi cancellati dalla collezione
                foreach (var mezzo in mezziCancellati)
                {
                    Mezzi.Remove(mezzo);
                }

                // Reset della selezione
                MezzoSelezionato = null;
                ElementiSelezionati.Clear();
                CodiceProposto = 0;

                // Mostra il riepilogo
                var messaggio = "Riepilogo cancellazione multipla:\n\n" +
                               string.Join("\n", risultatiCancellazione) +
                               $"\n\nMezzi di pagamento cancellati: {mezziCancellati.Count}\n" +
                               $"Mezzi saltati/errori: {mezziNonCancellati.Count}";

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

        public async void NuovoMezzoPagamento()
        {
            try
            {
                if (MezzoSelezionato != null)
                {
                    var conferma = MessageBox.Show(
                    "Creare un nuovo mezzo di pagamento? Le modifiche non salvate andranno perse.",
                    "Conferma Nuovo",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                    if (conferma != MessageBoxResult.Yes) return;
                }
                else 
                {
                    // Nessuna selezione attiva, procedo direttamente
                    MessageBox.Show("Creazione nuovo mezzo di pagamento.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                var next = await _service.GetNextIdAsync();
                CodiceProposto = next;
                MezzoSelezionato = new PagamentoMezzo
                {
                    IdPagamentoMezzo = 0, // Non impostato per insert
                    pagamentoMezzo = string.Empty,
                    Gruppo = string.Empty,
                    PagamentoInterbancario = 0,
                    IdBanca_Emesso = 0,
                    IdBanca_Ricevuto = 0,
                    FE_ModPag = string.Empty
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore creazione nuovo mezzo: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void SalvaMezzoPagamento()
        {
            if (MezzoSelezionato == null) return;
            try
            {
                if (string.IsNullOrWhiteSpace(MezzoSelezionato.pagamentoMezzo))
                {
                    MessageBox.Show("La descrizione è obbligatoria.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                IsLoading = true;
                var saved = await _service.SaveAsync(MezzoSelezionato);
                var index = Mezzi.IndexOf(MezzoSelezionato);
                if (index >= 0)
                {
                    Mezzi[index] = saved;
                    MezzoSelezionato = saved;
                }
                else
                {
                    Mezzi.Add(saved);
                    MezzoSelezionato = saved;
                }
                // Aggiorna il codice proposto dopo il salvataggio
                CodiceProposto = saved.IdPagamentoMezzo;
                MessageBox.Show("Mezzo pagamento salvato.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore salvataggio: {ex.Message} - {ex.ToString()}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void AnnullaModifiche()
        {
            if (MezzoSelezionato == null) return;
            try
            {
                if (MezzoSelezionato.IdPagamentoMezzo > 0)
                {
                    var fresh = await _service.GetByIdAsync(MezzoSelezionato.IdPagamentoMezzo);
                    if (fresh != null)
                    {
                        MezzoSelezionato = fresh;
                        CodiceProposto = fresh.IdPagamentoMezzo;
                    }
                }
                else
                {
                    // Se è un nuovo elemento, resetta tutto
                    MezzoSelezionato = null;
                    CodiceProposto = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore annullamento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void CercaMezziPagamento()
        {
            try
            {
                IsLoading = true;
                var list = await _service.GetAllAsync(FiltroCodice, FiltroDescrizione);
                Mezzi.Clear();
                foreach (var m in list) Mezzi.Add(m);

                // Reset selezione dopo la ricerca
                MezzoSelezionato = null;
                ElementiSelezionati.Clear();
                CodiceProposto = 0;
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

        public void SelezionaMezzoPagamento(string? descrizione)
        {
            // Obsoleto con binding, mantenuto per compatibilità
        }

        public void SelezionaMezzoPagamento(PagamentoMezzo mezzo)
        {
            MezzoSelezionato = mezzo;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}