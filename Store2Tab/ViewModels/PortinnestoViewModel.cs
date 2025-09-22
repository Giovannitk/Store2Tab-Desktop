using Store2Tab.Core.Interfaces;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Core.Services;
using Store2Tab.Data.Models;
using Store2Tab.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class PortinnestoViewModel : INotifyPropertyChanged
    {
        private readonly IPassPianteCeePortinnestoService? _portinnestoService;
        private readonly IPassPianteCeeSpecieService? _specieService;

        public PortinnestoViewModel()
        {
            // Costruttore di comodo senza DI: utilizza la OnConfiguring di AppDbContext
            var factory = new DefaultAppDbContextFactory();
            _portinnestoService = new PassPianteCeePortinnestoService(factory);
            _specieService = new PassPianteCeeSpecieService(factory);
            Portinnesto = new ObservableCollection<PassPianteCEE_Portinnesto>();
            ElementiSelezionati = new ObservableCollection<PassPianteCEE_Portinnesto>();
            SpecieBotaniche = new ObservableCollection<PassPianteCeeSpecie>();
            _ = CaricaAsync();
        }

        public PortinnestoViewModel(IPassPianteCeePortinnestoService portinnestoService, IPassPianteCeeSpecieService specieService)
        {
            _portinnestoService = portinnestoService;
            _specieService = specieService;
            Portinnesto = new ObservableCollection<PassPianteCEE_Portinnesto>();
            ElementiSelezionati = new ObservableCollection<PassPianteCEE_Portinnesto>();
            SpecieBotaniche = new ObservableCollection<PassPianteCeeSpecie>();
            _ = CaricaAsync();
        }

        private ObservableCollection<PassPianteCEE_Portinnesto> _varieta = new ObservableCollection<PassPianteCEE_Portinnesto>();
        public ObservableCollection<PassPianteCEE_Portinnesto> Portinnesto
        {
            get => _portinnesto;
            set { _portinnesto = value; OnPropertyChanged(); }
        }

        private PassPianteCEE_Portinnesto? _portinnestoSelezionata;
        public PassPianteCEE_Portinnesto? PortinnestoSelezionata
        {
            get => _portinnestoSelezionata;
            set
            {
                _portinnestoSelezionata = value;

                // Aggiorna il codice proposto quando selezioni un elemento esistente
                if (_portinnestoSelezionata != null)
                {
                    // Assicura che i campi obbligatori siano inizializzati
                    if (_portinnestoSelezionata.Portinnesto == null) _portinnestoSelezionata.Portinnesto = string.Empty;

                    // Aggiorna il codice proposto con l'ID dell'elemento selezionato
                    CodiceProposto = _portinnestoSelezionata.IdPassPianteCEE_Portinnesto;
                    SpecieSelezionata = _portinnestoSelezionata.SpecieBotanica;
                }

                OnPropertyChanged();
            }
        }

        private ObservableCollection<PassPianteCEE_Portinnesto> _elementiSelezionati = new ObservableCollection<PassPianteCEE_Portinnesto>();
        public ObservableCollection<PassPianteCEE_Portinnesto> ElementiSelezionati
        {
            get => _elementiSelezionati;
            set { _elementiSelezionati = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PassPianteCeeSpecie> _specieBotaniche = new ObservableCollection<PassPianteCeeSpecie>();
        public ObservableCollection<PassPianteCeeSpecie> SpecieBotaniche
        {
            get => _specieBotaniche;
            set { _specieBotaniche = value; OnPropertyChanged(); }
        }

        private PassPianteCeeSpecie? _specieSelezionata;
        public PassPianteCeeSpecie? SpecieSelezionata
        {
            get => _specieSelezionata;
            set { _specieSelezionata = value; OnPropertyChanged(); }
        }

        private string _filtroCodice = string.Empty;
        public string FiltroCodice
        {
            get => _filtroCodice;
            set
            {
                _filtroCodice = value;
                OnPropertyChanged();
                _ = CaricaAsync();
            }
        }

        private string _filtroPortinnesto = string.Empty;
        public string FiltroPortinnesto
        {
            get => _filtroPortinnesto;
            set
            {
                _filtroPortinnesto = value;
                OnPropertyChanged();
                _ = CaricaAsync();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private short _codiceProposto;
        private ObservableCollection<PassPianteCEE_Portinnesto> _portinnesto;

        public short CodiceProposto
        {
            get => _codiceProposto;
            set { _codiceProposto = value; OnPropertyChanged(); }
        }

        public async Task CaricaAsync()
        {
            if (_portinnestoService == null || _specieService == null) return;
            try
            {
                IsLoading = true;
                var portinnesti = await _portinnestoService.GetAllAsync(_filtroCodice, _filtroPortinnesto);
                Portinnesto.Clear();
                foreach (var item in portinnesti)
                {
                    Portinnesto.Add(item);
                }
                var specie = await _specieService.GetAllAsync();
                SpecieBotaniche.Clear();
                foreach (var item in specie)
                {
                    SpecieBotaniche.Add(item);
                }
                CodiceProposto = await _portinnestoService.GetNextIdAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il caricamento dei dati: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void CancellaPortinnesto()
        {
            if (PortinnestoSelezionata == null) return;

            var conferma = MessageBox.Show(
                $"Cancellare il portinnesto:\n\n" +
                $"Codice: {PortinnestoSelezionata.IdPassPianteCEE_Portinnesto}\n" +
                $"Specie: {PortinnestoSelezionata.SpecieBotanica?.Specie ?? "N/D"}\n" +
                $"Varietà: {PortinnestoSelezionata.Portinnesto}",
                "Conferma Cancellazione",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (conferma != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                var ok = await _portinnestoService.DeleteAsync(PortinnestoSelezionata.IdPassPianteCEE_Portinnesto);
                if (ok)
                {
                    Portinnesto.Remove(PortinnestoSelezionata);
                    PortinnestoSelezionata = null;
                    CodiceProposto = 0;
                    SpecieSelezionata = null;
                    MessageBox.Show("Portinnesto cancellato.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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
        public async void CancellaPortinnestoMultiple(List<PassPianteCEE_Portinnesto> portinnestoSelezionate)
        {
            if (portinnestoSelezionate == null || portinnestoSelezionate.Count() == 0)
            {
                MessageBox.Show("Nessun portinnesto selezionato per la cancellazione.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsLoading = true;
                var risultatiCancellazione = new List<string>();
                var portinnestoCancellate = new List<PassPianteCEE_Portinnesto>();
                var portinnestoNonCancellate = new List<PassPianteCEE_Portinnesto>();

                foreach (var portinnesto in portinnestoSelezionate)
                {
                    // Chiede conferma per ogni singola specie
                    var conferma = MessageBox.Show(
                        $"Cancellare la specie botanica:\n\n" +
                        $"Codice: {portinnesto.IdPassPianteCEE_Portinnesto}\n" +
                        $"Specie: {portinnesto.Portinnesto}\n\n" +
                        $"Rimanenti da confermare: {portinnestoSelezionate.Count - portinnestoSelezionate.IndexOf(portinnesto) - 1}",
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
                            if (portinnesto.IdPassPianteCEE_Portinnesto > 0)
                            {
                                await _portinnestoService.DeleteAsync(portinnesto.IdPassPianteCEE_Portinnesto);
                                portinnestoCancellate.Add(portinnesto);
                                risultatiCancellazione.Add($"✓ {portinnesto.Portinnesto} - Cancellato");
                            }
                            else
                            {
                                risultatiCancellazione.Add($"✗ {portinnesto.Portinnesto} - ID non valido");
                                portinnestoNonCancellate.Add(portinnesto);
                            }
                        }
                        catch (Exception ex)
                        {
                            risultatiCancellazione.Add($"✗ {portinnesto.Portinnesto} - Errore: {ex.Message}");
                            portinnestoNonCancellate.Add(portinnesto);
                        }
                    }
                    else
                    {
                        risultatiCancellazione.Add($"○ {portinnesto.Portinnesto} - Saltato");
                        portinnestoNonCancellate.Add(portinnesto);
                    }
                }

                // Rimuove le varietà cancellate dalla collezione
                foreach (var portinnesto in portinnestoCancellate)
                {
                    Portinnesto.Remove(portinnesto);
                }

                // Reset della selezione
                PortinnestoSelezionata = null;
                ElementiSelezionati.Clear();
                CodiceProposto = 0;

                // Mostra il riepilogo
                var messaggio = "Riepilogo cancellazione multipla:\n\n" +
                               string.Join("\n", risultatiCancellazione) +
                               $"\n\nPortinnesti cancellate: {portinnestoCancellate.Count}\n" +
                               $"Portinnesti saltate/errori: {portinnestoNonCancellate.Count}";

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

        public async void NuovoPortinnesto()
        {
            try
            {
                var next = await _portinnestoService.GetNextIdAsync();
                CodiceProposto = next;

                PortinnestoSelezionata = new PassPianteCEE_Portinnesto
                {
                    IdPassPianteCEE_Portinnesto = 0, // Non impostato per insert
                    IdPassPianteCEE_Specie = 0,
                    Portinnesto = string.Empty,
                    SpecieBotanica = null
                };

                // Reset della specie selezionata per consentire la selezione
                SpecieSelezionata = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore creazione nuovo portinnesto: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void SalvoPortinnesto()
        {
            if (PortinnestoSelezionata == null) return;

            try
            {
                // Validazione
                if (SpecieSelezionata == null || SpecieSelezionata.IdPassPianteCEE_Specie <= 0)
                {
                    MessageBox.Show("La specie botanica è obbligatoria.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(PortinnestoSelezionata.Portinnesto))
                {
                    MessageBox.Show("Il portinnesto è obbligatorio.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsLoading = true;

                // Aggiorna l'ID della specie selezionata
                PortinnestoSelezionata.IdPassPianteCEE_Specie = SpecieSelezionata.IdPassPianteCEE_Specie;

                var saved = await _portinnestoService.SaveAsync(PortinnestoSelezionata);

                var index = Portinnesto.IndexOf(PortinnestoSelezionata);
                if (index >= 0)
                {
                    Portinnesto[index] = saved;
                    PortinnestoSelezionata = saved;
                }
                else
                {
                    Portinnesto.Add(saved);
                    PortinnestoSelezionata = saved;
                }

                // Aggiorna il codice proposto e la specie selezionata dopo il salvataggio
                CodiceProposto = saved.IdPassPianteCEE_Portinnesto;
                SpecieSelezionata = saved.SpecieBotanica;

                MessageBox.Show("Portinnesto salvato.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (PortinnestoSelezionata == null) return;

            try
            {
                if (PortinnestoSelezionata.IdPassPianteCEE_Portinnesto > 0)
                {
                    var fresh = await _portinnestoService.GetByIdAsync(PortinnestoSelezionata.IdPassPianteCEE_Portinnesto);
                    if (fresh != null)
                    {
                        PortinnestoSelezionata = fresh;
                        CodiceProposto = fresh.IdPassPianteCEE_Portinnesto;
                        SpecieSelezionata = fresh.SpecieBotanica;
                    }
                }
                else
                {
                    // Se è un nuovo elemento, resetta tutto
                    PortinnestoSelezionata = null;
                    CodiceProposto = 0;
                    SpecieSelezionata = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore annullamento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void CercaPortinnesto()
        {
            try
            {
                IsLoading = true;
                short? filtroSpecieId = null;

                // Se è specificata una specie nel filtro, usa il suo ID
                if (SpecieSelezionata != null && SpecieSelezionata.IdPassPianteCEE_Specie > 0)
                {
                    filtroSpecieId = SpecieSelezionata.IdPassPianteCEE_Specie;
                }

                var list = await _portinnestoService?.GetAllAsync(FiltroCodice, FiltroPortinnesto, filtroSpecieId);
                Portinnesto.Clear();
                foreach (var v in list) Portinnesto.Add(v);

                // Reset selezione dopo la ricerca
                PortinnestoSelezionata = null;
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

        public void SelezionoPortinnesto(string codice, string specieBotanica, string portinnesto)
        {
            // Trova il portinnesto corrispondente nella collezione
            var portinnestoTrovato = Portinnesto.FirstOrDefault(v =>
                v.IdPassPianteCEE_Portinnesto.ToString() == codice &&
                v.Portinnesto == portinnesto &&
                (v.SpecieBotanica?.Specie == specieBotanica || specieBotanica == ""));

            if (portinnestoTrovato != null)
            {
                PortinnestoSelezionata = portinnestoTrovato;
            }
        }

        public void SelezionaPortinnesto(PassPianteCEE_Portinnesto portinnesto)
        {
            PortinnestoSelezionata = portinnesto;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}