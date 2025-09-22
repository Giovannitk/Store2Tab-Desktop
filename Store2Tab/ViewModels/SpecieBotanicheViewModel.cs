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
    public class SpecieBotanicheViewModel : INotifyPropertyChanged
    {
        private readonly IPassPianteCeeSpecieService _service;

        public SpecieBotanicheViewModel()
        {
            // Costruttore di comodo senza DI: utilizza la OnConfiguring di AppDbContext
            var factory = new DefaultAppDbContextFactory();
            _service = new PassPianteCeeSpecieService(factory);
            Specie = new ObservableCollection<PassPianteCeeSpecie>();
            ElementiSelezionati = new ObservableCollection<PassPianteCeeSpecie>();
            _ = CaricaAsync();
        }

        public SpecieBotanicheViewModel(IPassPianteCeeSpecieService service)
        {
            _service = service;
            Specie = new ObservableCollection<PassPianteCeeSpecie>();
            ElementiSelezionati = new ObservableCollection<PassPianteCeeSpecie>();
            _ = CaricaAsync();
        }

        private ObservableCollection<PassPianteCeeSpecie> _specie = new ObservableCollection<PassPianteCeeSpecie>();
        public ObservableCollection<PassPianteCeeSpecie> Specie
        {
            get => _specie;
            set { _specie = value; OnPropertyChanged(); }
        }

        private PassPianteCeeSpecie? _specieSelezionata;
        public PassPianteCeeSpecie? SpecieSelezionata
        {
            get => _specieSelezionata;
            set
            {
                _specieSelezionata = value;

                // Aggiorna il codice proposto quando selezioni un elemento esistente
                if (_specieSelezionata != null)
                {
                    // Assicura che il campo obbligatorio sia inizializzato
                    if (_specieSelezionata.Specie == null) _specieSelezionata.Specie = string.Empty;

                    // Aggiorna il codice proposto con l'ID dell'elemento selezionato
                    CodiceProposto = _specieSelezionata.IdPassPianteCEE_Specie;
                }

                OnPropertyChanged();
            }
        }

        // Proprietà per la selezione multipla
        private ObservableCollection<PassPianteCeeSpecie> _elementiSelezionati = new ObservableCollection<PassPianteCeeSpecie>();
        public ObservableCollection<PassPianteCeeSpecie> ElementiSelezionati
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

        private string _filtroSpecie = string.Empty;
        public string FiltroSpecie
        {
            get => _filtroSpecie;
            set { _filtroSpecie = value; OnPropertyChanged(); }
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
                Specie.Clear();
                foreach (var s in list) Specie.Add(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore caricamento specie botaniche: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void CancellaSpecie()
        {
            if (SpecieSelezionata == null) return;

            var conferma = MessageBox.Show(
                $"Cancellare la specie botanica:\n\n" +
                $"Codice: {SpecieSelezionata.IdPassPianteCEE_Specie}\n" +
                $"Specie: {SpecieSelezionata.Specie}",
                "Conferma Cancellazione",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (conferma != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                var ok = await _service.DeleteAsync(SpecieSelezionata.IdPassPianteCEE_Specie);
                if (ok)
                {
                    Specie.Remove(SpecieSelezionata);
                    SpecieSelezionata = null;
                    CodiceProposto = 0;
                    MessageBox.Show("Specie botanica cancellata.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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
        public async void CancellaSpecieMultiple(List<PassPianteCeeSpecie> specieSelezionate)
        {
            if (specieSelezionate == null || specieSelezionate.Count == 0)
            {
                MessageBox.Show("Nessuna specie botanica selezionata per la cancellazione.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                var risultatiCancellazione = new List<string>();
                var specieCancellate = new List<PassPianteCeeSpecie>();
                var specieNonCancellate = new List<PassPianteCeeSpecie>();

                foreach (var specie in specieSelezionate)
                {
                    // Chiede conferma per ogni singola specie
                    var conferma = MessageBox.Show(
                        $"Cancellare la specie botanica:\n\n" +
                        $"Codice: {specie.IdPassPianteCEE_Specie}\n" +
                        $"Specie: {specie.Specie}\n\n" +
                        $"Rimanenti da confermare: {specieSelezionate.Count - specieSelezionate.IndexOf(specie) - 1}",
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
                            if (specie.IdPassPianteCEE_Specie > 0)
                            {
                                await _service.DeleteAsync(specie.IdPassPianteCEE_Specie);
                                specieCancellate.Add(specie);
                                risultatiCancellazione.Add($"✓ {specie.Specie} - Cancellata");
                            }
                            else
                            {
                                risultatiCancellazione.Add($"✗ {specie.Specie} - ID non valido");
                                specieNonCancellate.Add(specie);
                            }
                        }
                        catch (Exception ex)
                        {
                            risultatiCancellazione.Add($"✗ {specie.Specie} - Errore: {ex.Message}");
                            specieNonCancellate.Add(specie);
                        }
                    }
                    else
                    {
                        risultatiCancellazione.Add($"○ {specie.Specie} - Saltata");
                        specieNonCancellate.Add(specie);
                    }
                }

                // Rimuove le specie cancellate dalla collezione
                foreach (var specie in specieCancellate)
                {
                    Specie.Remove(specie);
                }

                // Reset della selezione
                SpecieSelezionata = null;
                ElementiSelezionati.Clear();
                CodiceProposto = 0;

                // Mostra il riepilogo
                var messaggio = "Riepilogo cancellazione multipla:\n\n" +
                               string.Join("\n", risultatiCancellazione) +
                               $"\n\nSpecie botaniche cancellate: {specieCancellate.Count}\n" +
                               $"Specie saltate/errori: {specieNonCancellate.Count}";

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

        public async void NuovaSpecie()
        {
            try
            {
                var next = await _service.GetNextIdAsync();
                CodiceProposto = next;
                SpecieSelezionata = new PassPianteCeeSpecie
                {
                    IdPassPianteCEE_Specie = 0, // Non impostato per insert
                    Specie = string.Empty
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore creazione nuova specie: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void SalvaSpecie()
        {
            if (SpecieSelezionata == null) return;
            try
            {
                if (string.IsNullOrWhiteSpace(SpecieSelezionata.Specie))
                {
                    MessageBox.Show("La specie botanica è obbligatoria.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsLoading = true;
                var saved = await _service.SaveAsync(SpecieSelezionata);
                var index = Specie.IndexOf(SpecieSelezionata);
                if (index >= 0)
                {
                    Specie[index] = saved;
                    SpecieSelezionata = saved;
                }
                else
                {
                    Specie.Add(saved);
                    SpecieSelezionata = saved;
                }

                // Aggiorna il codice proposto dopo il salvataggio
                CodiceProposto = saved.IdPassPianteCEE_Specie;
                MessageBox.Show("Specie botanica salvata.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                // Ricarica la lista nelle altre tab collegate alla specie per assicurarsi che sia aggiornata
                // (es. Varietà)
                OnPropertyChanged(nameof(Specie));
                // Potrebbe essere necessario un evento più sofisticato in un'app reale
                // per notificare altre ViewModel collegate.
                // Esempio: EventAggregator, Messenger, ecc.
                // Qui si assume che ci sia un metodo statico per notificare altre ViewModel
                // EventAggregator.Publish(new SpecieAggiornataEvent(saved));
                // Per adesso, si lascia così com'è, e l'utente deve chiudere e riaprire 
                // le altre tab per aggiornare la tabella.
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
            if (SpecieSelezionata == null) return;
            try
            {
                if (SpecieSelezionata.IdPassPianteCEE_Specie > 0)
                {
                    var fresh = await _service.GetByIdAsync(SpecieSelezionata.IdPassPianteCEE_Specie);
                    if (fresh != null)
                    {
                        SpecieSelezionata = fresh;
                        CodiceProposto = fresh.IdPassPianteCEE_Specie;
                    }
                }
                else
                {
                    // Se è un nuovo elemento, resetta tutto
                    SpecieSelezionata = null;
                    CodiceProposto = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore annullamento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void CercaSpecie()
        {
            try
            {
                IsLoading = true;
                var list = await _service.GetAllAsync(FiltroCodice, FiltroSpecie);
                Specie.Clear();
                foreach (var s in list) Specie.Add(s);

                // Reset selezione dopo la ricerca
                SpecieSelezionata = null;
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

        public void SelezionaSpecie(string codice, string specieBotanica)
        {
            // Trova la specie corrispondente nella collezione
            var specie = Specie.FirstOrDefault(s =>
                s.IdPassPianteCEE_Specie.ToString() == codice &&
                s.Specie == specieBotanica);

            if (specie != null)
            {
                SpecieSelezionata = specie;
            }
        }

        public void SelezionaSpecie(PassPianteCeeSpecie specie)
        {
            SpecieSelezionata = specie;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}