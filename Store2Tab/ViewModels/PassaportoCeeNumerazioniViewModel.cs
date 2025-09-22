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
    public class PassaportoCeeNumerazioniViewModel : INotifyPropertyChanged
    {
        private readonly IPassPianteCeeNumerazioneService _service;

        public PassaportoCeeNumerazioniViewModel()
        {
            // Costruttore di comodo senza DI: utilizza la OnConfiguring di AppDbContext
            var factory = new DefaultAppDbContextFactory();
            _service = new PassPianteCeeNumerazioneService(factory);
            Numerazioni = new ObservableCollection<PassPianteCeeNumerazione>();
            ElementiSelezionati = new ObservableCollection<PassPianteCeeNumerazione>();
            _ = CaricaAsync();
        }

        public PassaportoCeeNumerazioniViewModel(IPassPianteCeeNumerazioneService service)
        {
            _service = service;
            Numerazioni = new ObservableCollection<PassPianteCeeNumerazione>();
            ElementiSelezionati = new ObservableCollection<PassPianteCeeNumerazione>();
            _ = CaricaAsync();
        }

        private ObservableCollection<PassPianteCeeNumerazione> _numerazioni = new ObservableCollection<PassPianteCeeNumerazione>();
        public ObservableCollection<PassPianteCeeNumerazione> Numerazioni
        {
            get => _numerazioni;
            set { _numerazioni = value; OnPropertyChanged(); }
        }

        private PassPianteCeeNumerazione? _numerazioneSelezionata;
        public PassPianteCeeNumerazione? NumerazioneSelezionata
        {
            get => _numerazioneSelezionata;
            set
            {
                _numerazioneSelezionata = value;
                
                // Aggiorna il codice proposto quando selezioni un elemento esistente
                if (_numerazioneSelezionata != null)
                {
                    // Assicura che tutti i campi obbligatori siano inizializzati
                    if (_numerazioneSelezionata.Descrizione == null) _numerazioneSelezionata.Descrizione = string.Empty;
                    if (_numerazioneSelezionata.Sigla == null) _numerazioneSelezionata.Sigla = string.Empty;
                    if (_numerazioneSelezionata.Prefisso == null) _numerazioneSelezionata.Prefisso = string.Empty;
                    
                    // Aggiorna il codice proposto con l'ID dell'elemento selezionato
                    CodiceProposto = _numerazioneSelezionata.IdPassPianteCEE_Numerazione;
                }
                
                OnPropertyChanged();
            }
        }

        // Proprietà per la selezione multipla
        private ObservableCollection<PassPianteCeeNumerazione> _elementiSelezionati = new ObservableCollection<PassPianteCeeNumerazione>();
        public ObservableCollection<PassPianteCeeNumerazione> ElementiSelezionati
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

        private int _codiceProposto;
        public int CodiceProposto
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
                Numerazioni.Clear();
                foreach (var n in list) Numerazioni.Add(n);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore caricamento numerazioni: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void CancellaNumerazione()
        {
            if (NumerazioneSelezionata == null) return;
            
            var conferma = MessageBox.Show(
                $"Cancellare la numerazione:\n\n" +
                $"Codice: {NumerazioneSelezionata.IdPassPianteCEE_Numerazione}\n" +
                $"Descrizione: {NumerazioneSelezionata.Descrizione}\n" +
                $"Sigla: {NumerazioneSelezionata.Sigla}\n" +
                $"Prefisso: {NumerazioneSelezionata.Prefisso}",
                "Conferma Cancellazione",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (conferma != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                var ok = await _service.DeleteAsync(NumerazioneSelezionata.IdPassPianteCEE_Numerazione);
                if (ok)
                {
                    Numerazioni.Remove(NumerazioneSelezionata);
                    NumerazioneSelezionata = null;
                    CodiceProposto = 0;
                    MessageBox.Show("Numerazione cancellata.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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

        // Cancellazione multipla
        public async void CancellaNumerazioniMultiple(List<PassPianteCeeNumerazione> numerazioniSelezionate)
        {
            if (numerazioniSelezionate == null || numerazioniSelezionate.Count == 0)
            {
                MessageBox.Show("Nessuna numerazione selezionata per la cancellazione.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                var risultatiCancellazione = new List<string>();
                var numerazioniCancellate = new List<PassPianteCeeNumerazione>();
                var numerazioniNonCancellate = new List<PassPianteCeeNumerazione>();

                foreach (var numerazione in numerazioniSelezionate)
                {
                    // Chiede conferma per ogni singola numerazione
                    var conferma = MessageBox.Show(
                        $"Cancellare la numerazione:\n\n" +
                        $"Codice: {numerazione.IdPassPianteCEE_Numerazione}\n" +
                        $"Descrizione: {numerazione.Descrizione}\n" +
                        $"Sigla: {numerazione.Sigla}\n" +
                        $"Prefisso: {numerazione.Prefisso}\n\n" +
                        $"Rimanenti da confermare: {numerazioniSelezionate.Count - numerazioniSelezionate.IndexOf(numerazione) - 1}",
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
                            if (numerazione.IdPassPianteCEE_Numerazione > 0)
                            {
                                await _service.DeleteAsync(numerazione.IdPassPianteCEE_Numerazione);
                                numerazioniCancellate.Add(numerazione);
                                risultatiCancellazione.Add($"✓ {numerazione.Descrizione} - Cancellata");
                            }
                            else
                            {
                                risultatiCancellazione.Add($"✗ {numerazione.Descrizione} - ID non valido");
                                numerazioniNonCancellate.Add(numerazione);
                            }
                        }
                        catch (Exception ex)
                        {
                            risultatiCancellazione.Add($"✗ {numerazione.Descrizione} - Errore: {ex.Message}");
                            numerazioniNonCancellate.Add(numerazione);
                        }
                    }
                    else
                    {
                        risultatiCancellazione.Add($"○ {numerazione.Descrizione} - Saltata");
                        numerazioniNonCancellate.Add(numerazione);
                    }
                }

                // Rimuove le numerazioni cancellate dalla collezione
                foreach (var numerazione in numerazioniCancellate)
                {
                    Numerazioni.Remove(numerazione);
                }

                // Reset della selezione
                NumerazioneSelezionata = null;
                ElementiSelezionati.Clear();
                CodiceProposto = 0;

                // Mostra il riepilogo
                var messaggio = "Riepilogo cancellazione multipla:\n\n" +
                               string.Join("\n", risultatiCancellazione) +
                               $"\n\nNumerazioni cancellate: {numerazioniCancellate.Count}\n" +
                               $"Numerazioni saltate/errori: {numerazioniNonCancellate.Count}";

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

        public async void NuovaNumerazione()
        {
            try
            {
                var next = await _service.GetNextIdAsync();
                CodiceProposto = next;
                NumerazioneSelezionata = new PassPianteCeeNumerazione
                {
                    IdPassPianteCEE_Numerazione = 0, // Non impostato per insert
                    Descrizione = string.Empty,
                    Sigla = string.Empty,
                    Prefisso = string.Empty
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore creazione nuova numerazione: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void SalvaNumerazione()
        {
            if (NumerazioneSelezionata == null) return;
            try
            {
                if (string.IsNullOrWhiteSpace(NumerazioneSelezionata.Descrizione))
                {
                    MessageBox.Show("La descrizione è obbligatoria.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsLoading = true;
                var saved = await _service.SaveAsync(NumerazioneSelezionata);
                var index = Numerazioni.IndexOf(NumerazioneSelezionata);
                if (index >= 0)
                {
                    Numerazioni[index] = saved;
                    NumerazioneSelezionata = saved;
                }
                else
                {
                    Numerazioni.Add(saved);
                    NumerazioneSelezionata = saved;
                }
                
                // Aggiorna il codice proposto dopo il salvataggio
                CodiceProposto = saved.IdPassPianteCEE_Numerazione;
                MessageBox.Show("Numerazione salvata.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore salvataggio: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void AnnullaModifiche()
        {
            if (NumerazioneSelezionata == null) return;
            try
            {
                if (NumerazioneSelezionata.IdPassPianteCEE_Numerazione > 0)
                {
                    var fresh = await _service.GetByIdAsync(NumerazioneSelezionata.IdPassPianteCEE_Numerazione);
                    if (fresh != null) 
                    {
                        NumerazioneSelezionata = fresh;
                        CodiceProposto = fresh.IdPassPianteCEE_Numerazione;
                    }
                }
                else
                {
                    // Se è un nuovo elemento, resetta tutto
                    NumerazioneSelezionata = null;
                    CodiceProposto = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore annullamento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void CercaNumerazioni()
        {
            try
            {
                IsLoading = true;
                var list = await _service.GetAllAsync(FiltroCodice, FiltroDescrizione);
                Numerazioni.Clear();
                foreach (var n in list) Numerazioni.Add(n);
                
                // Reset selezione dopo la ricerca
                NumerazioneSelezionata = null;
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

        public void SelezionaNumerazione(string codice, string descrizione, string sigla, string prefisso)
        {
            // Trova la numerazione corrispondente nella collezione
            var numerazione = Numerazioni.FirstOrDefault(n => 
                n.IdPassPianteCEE_Numerazione.ToString() == codice && 
                n.Descrizione == descrizione);
            
            if (numerazione != null)
            {
                NumerazioneSelezionata = numerazione;
            }
        }

        public void SelezionaNumerazione(PassPianteCeeNumerazione numerazione)
        {
            NumerazioneSelezionata = numerazione;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}