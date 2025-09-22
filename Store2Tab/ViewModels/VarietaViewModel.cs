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
    public class VarietaViewModel : INotifyPropertyChanged
    {
        private readonly IPassPianteCeeVarietaService _varietaService;
        private readonly IPassPianteCeeSpecieService _specieService;

        public VarietaViewModel()
        {
            // Costruttore di comodo senza DI: utilizza la OnConfiguring di AppDbContext
            var factory = new DefaultAppDbContextFactory();
            _varietaService = new PassPianteCeeVarietaService(factory);
            _specieService = new PassPianteCeeSpecieService(factory);
            Varieta = new ObservableCollection<PassPianteCeeVarieta>();
            ElementiSelezionati = new ObservableCollection<PassPianteCeeVarieta>();
            SpecieBotaniche = new ObservableCollection<PassPianteCeeSpecie>();
            _ = CaricaAsync();
        }

        public VarietaViewModel(IPassPianteCeeVarietaService varietaService, IPassPianteCeeSpecieService specieService)
        {
            _varietaService = varietaService;
            _specieService = specieService;
            Varieta = new ObservableCollection<PassPianteCeeVarieta>();
            ElementiSelezionati = new ObservableCollection<PassPianteCeeVarieta>();
            SpecieBotaniche = new ObservableCollection<PassPianteCeeSpecie>();
            _ = CaricaAsync();
        }

        private ObservableCollection<PassPianteCeeVarieta> _varieta = new ObservableCollection<PassPianteCeeVarieta>();
        public ObservableCollection<PassPianteCeeVarieta> Varieta
        {
            get => _varieta;
            set { _varieta = value; OnPropertyChanged(); }
        }

        private PassPianteCeeVarieta? _varietaSelezionata;
        public PassPianteCeeVarieta? VarietaSelezionata
        {
            get => _varietaSelezionata;
            set
            {
                _varietaSelezionata = value;

                // Aggiorna il codice proposto quando selezioni un elemento esistente
                if (_varietaSelezionata != null)
                {
                    // Assicura che i campi obbligatori siano inizializzati
                    if (_varietaSelezionata.Varieta == null) _varietaSelezionata.Varieta = string.Empty;

                    // Aggiorna il codice proposto con l'ID dell'elemento selezionato
                    CodiceProposto = _varietaSelezionata.IdPassPianteCEE_Varieta;
                    SpecieSelezionata = _varietaSelezionata.SpecieBotanica;
                }

                OnPropertyChanged();
            }
        }

        private ObservableCollection<PassPianteCeeVarieta> _elementiSelezionati = new ObservableCollection<PassPianteCeeVarieta>();
        public ObservableCollection<PassPianteCeeVarieta> ElementiSelezionati
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
            set
            {
                _specieSelezionata = value;
                OnPropertyChanged();
            }
        }

        private string _filtroCodice = string.Empty;
        public string FiltroCodice
        {
            get => _filtroCodice;
            set { _filtroCodice = value; OnPropertyChanged(); }
        }

        private string _filtroVarieta = string.Empty;
        public string FiltroVarieta
        {
            get => _filtroVarieta;
            set { _filtroVarieta = value; OnPropertyChanged(); }
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

                // Carica le varietà
                var varietaList = await _varietaService.GetAllAsync();
                Varieta.Clear();
                foreach (var v in varietaList) Varieta.Add(v);

                // Carica le specie botaniche per la ComboBox
                var specieList = await _specieService.GetAllAsync();
                SpecieBotaniche.Clear();
                foreach (var s in specieList) SpecieBotaniche.Add(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore caricamento varietà: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void CancellaVarieta()
        {
            if (VarietaSelezionata == null) return;

            var conferma = MessageBox.Show(
                $"Cancellare la varietà:\n\n" +
                $"Codice: {VarietaSelezionata.IdPassPianteCEE_Varieta}\n" +
                $"Specie: {VarietaSelezionata.SpecieBotanica?.Specie ?? "N/D"}\n" +
                $"Varietà: {VarietaSelezionata.Varieta}",
                "Conferma Cancellazione",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (conferma != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                var ok = await _varietaService.DeleteAsync(VarietaSelezionata.IdPassPianteCEE_Varieta);
                if (ok)
                {
                    Varieta.Remove(VarietaSelezionata);
                    VarietaSelezionata = null;
                    CodiceProposto = 0;
                    SpecieSelezionata = null;
                    MessageBox.Show("Varietà cancellata.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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
        public async void CancellaVarietaMultiple(List<PassPianteCeeVarieta> varietaSelezionate)
        {
            if (varietaSelezionate == null || varietaSelezionate.Count() == 0)
            {
                MessageBox.Show("Nessuna varietà selezionata per la cancellazione.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try 
            {
                IsLoading = true;
                var risultatiCancellazione = new List<string>();
                var varietaCancellate = new List<PassPianteCeeVarieta>();
                var varietaNonCancellate = new List<PassPianteCeeVarieta>();

                foreach (var varieta in varietaSelezionate)
                {
                    // Chiede conferma per ogni singola specie
                    var conferma = MessageBox.Show(
                        $"Cancellare la specie botanica:\n\n" +
                        $"Codice: {varieta.IdPassPianteCEE_Varieta}\n" +
                        $"Specie: {varieta.Varieta}\n\n" +
                        $"Rimanenti da confermare: {varietaSelezionate.Count - varietaSelezionate.IndexOf(varieta) - 1}",
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
                            if (varieta.IdPassPianteCEE_Varieta > 0)
                            {
                                await _varietaService.DeleteAsync(varieta.IdPassPianteCEE_Varieta);
                                varietaCancellate.Add(varieta);
                                risultatiCancellazione.Add($"✓ {varieta.Varieta} - Cancellata");
                            }
                            else
                            {
                                risultatiCancellazione.Add($"✗ {varieta.Varieta} - ID non valido");
                                varietaNonCancellate.Add(varieta);
                            }
                        }
                        catch (Exception ex)
                        {
                            risultatiCancellazione.Add($"✗ {varieta.Varieta} - Errore: {ex.Message}");
                            varietaNonCancellate.Add(varieta);
                        }
                    }
                    else
                    {
                        risultatiCancellazione.Add($"○ {varieta.Varieta} - Saltata");
                        varietaNonCancellate.Add(varieta);
                    }
                }
                
                // Rimuove le varietà cancellate dalla collezione
                foreach (var varieta in varietaCancellate)
                {
                    Varieta.Remove(varieta);
                }

                // Reset della selezione
                VarietaSelezionata = null;
                ElementiSelezionati.Clear();
                CodiceProposto = 0;

                // Mostra il riepilogo
                var messaggio = "Riepilogo cancellazione multipla:\n\n" +
                               string.Join("\n", risultatiCancellazione) +
                               $"\n\nVarietà cancellate: {varietaCancellate.Count}\n" +
                               $"Varietà saltate/errori: {varietaNonCancellate.Count}";

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

        public async void NuovaVarieta()
        {
            try
            {
                var next = await _varietaService.GetNextIdAsync();
                CodiceProposto = next;

                VarietaSelezionata = new PassPianteCeeVarieta
                {
                    IdPassPianteCEE_Varieta = 0, // Non impostato per insert
                    IdPassPianteCEE_Specie = 0,
                    Varieta = string.Empty,
                    SpecieBotanica = null
                };

                // Reset della specie selezionata per consentire la selezione
                SpecieSelezionata = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore creazione nuova varietà: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void SalvaVarieta()
        {
            if (VarietaSelezionata == null) return;

            try
            {
                // Validazione
                if (SpecieSelezionata == null || SpecieSelezionata.IdPassPianteCEE_Specie <= 0)
                {
                    MessageBox.Show("La specie botanica è obbligatoria.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(VarietaSelezionata.Varieta))
                {
                    MessageBox.Show("La varietà è obbligatoria.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsLoading = true;

                // Aggiorna l'ID della specie selezionata
                VarietaSelezionata.IdPassPianteCEE_Specie = SpecieSelezionata.IdPassPianteCEE_Specie;

                var saved = await _varietaService.SaveAsync(VarietaSelezionata);

                var index = Varieta.IndexOf(VarietaSelezionata);
                if (index >= 0)
                {
                    Varieta[index] = saved;
                    VarietaSelezionata = saved;
                }
                else
                {
                    Varieta.Add(saved);
                    VarietaSelezionata = saved;
                }

                // Aggiorna il codice proposto e la specie selezionata dopo il salvataggio
                CodiceProposto = saved.IdPassPianteCEE_Varieta;
                SpecieSelezionata = saved.SpecieBotanica;

                MessageBox.Show("Varietà salvata.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (VarietaSelezionata == null) return;

            try
            {
                if (VarietaSelezionata.IdPassPianteCEE_Varieta > 0)
                {
                    var fresh = await _varietaService.GetByIdAsync(VarietaSelezionata.IdPassPianteCEE_Varieta);
                    if (fresh != null)
                    {
                        VarietaSelezionata = fresh;
                        CodiceProposto = fresh.IdPassPianteCEE_Varieta;
                        SpecieSelezionata = fresh.SpecieBotanica;
                    }
                }
                else
                {
                    // Se è un nuovo elemento, resetta tutto
                    VarietaSelezionata = null;
                    CodiceProposto = 0;
                    SpecieSelezionata = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore annullamento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void CercaVarieta()
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

                var list = await _varietaService.GetAllAsync(FiltroCodice, FiltroVarieta, filtroSpecieId);
                Varieta.Clear();
                foreach (var v in list) Varieta.Add(v);

                // Reset selezione dopo la ricerca
                VarietaSelezionata = null;
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

        public void SelezionaVarieta(string codice, string specieBotanica, string varieta)
        {
            // Trova la varietà corrispondente nella collezione
            var varietaTrovata = Varieta.FirstOrDefault(v =>
                v.IdPassPianteCEE_Varieta.ToString() == codice &&
                v.Varieta == varieta &&
                (v.SpecieBotanica?.Specie == specieBotanica || specieBotanica == ""));

            if (varietaTrovata != null)
            {
                VarietaSelezionata = varietaTrovata;
            }
        }

        public void SelezionaVarieta(PassPianteCeeVarieta varieta)
        {
            VarietaSelezionata = varieta;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}