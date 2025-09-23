using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Store2Tab.Core.Services;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.ViewModels
{
    public class RaggruppamentoOption
    {
        public byte Valore { get; set; }
        public string Descrizione { get; set; } = string.Empty;
    }

    public class PassPianteCeeTipoViewModel : INotifyPropertyChanged
    {
        private readonly IPassPianteCeeTipoService _tipoService;
        private readonly IPassPianteCeeNumerazioneService _numerazioneService;

        private PassPianteCeeTipo? _tipoSelezionato;
        private bool _isLoading;
        private bool _isModified;
        private string _filtroCodice = string.Empty;
        private string _filtroDescrizione = string.Empty;
        private string _codiceProposto = string.Empty;

        public ObservableCollection<RaggruppamentoOption> OpzioniRaggruppamento { get; }
            = new ObservableCollection<RaggruppamentoOption>();

        private void InizializzaOpzioniRaggruppamento()
        {
            OpzioniRaggruppamento.Clear();

            OpzioniRaggruppamento.Add(new RaggruppamentoOption { Valore = 250, Descrizione = "" });
            OpzioniRaggruppamento.Add(new RaggruppamentoOption { Valore = 0, Descrizione = "un Pass. per Specie, Varietà e Portinnesto (unico numero)" });
            OpzioniRaggruppamento.Add(new RaggruppamentoOption { Valore = 1, Descrizione = "Pass. per Specie, Varietà e Portinnesto (unico pass.)" });
            OpzioniRaggruppamento.Add(new RaggruppamentoOption { Valore = 2, Descrizione = "un Pass. per Specie, Varietà e Portinnesto" });
            OpzioniRaggruppamento.Add(new RaggruppamentoOption { Valore = 4, Descrizione = "un Pass. per Specie, Varietà e Cod. Tracciabilità" });
        }

        public string GetDescrizioneRaggruppamento(byte valore)
        {
            return valore switch
            {
                0 => "un Pass. per S, V e P (unico numero)",
                1 => "Pass. per S, V e P (unico pass.)",
                2 => "un Pass. per S, V e P",
                4 => "un Pass. per S, V e Cod. Trac.",
                _ => ""
            };
        }

        public PassPianteCeeTipoViewModel()
        {
            // Per design time e fallback
            var factory = new DefaultAppDbContextFactory();
            _tipoService = new PassPianteCeeTipoService(factory);
            _numerazioneService = new PassPianteCeeNumerazioneService(factory);

            Tipi = new ObservableCollection<PassPianteCeeTipo>();
            Numerazioni = new ObservableCollection<PassPianteCeeNumerazione>();

            _ = CaricaAsync();

            InizializzaOpzioniRaggruppamento();
        }

        public PassPianteCeeTipoViewModel(IPassPianteCeeTipoService tipoService, IPassPianteCeeNumerazioneService numerazioneService)
        {
            _tipoService = tipoService;
            _numerazioneService = numerazioneService;

            Tipi = new ObservableCollection<PassPianteCeeTipo>();
            Numerazioni = new ObservableCollection<PassPianteCeeNumerazione>();

            _ = CaricaAsync();
            InizializzaOpzioniRaggruppamento();
        }

        #region Properties

        public ObservableCollection<PassPianteCeeTipo> Tipi { get; }
        public ObservableCollection<PassPianteCeeNumerazione> Numerazioni { get; }

        public PassPianteCeeTipo? TipoSelezionato
        {
            get => _tipoSelezionato;
            set
            {
                if (SetProperty(ref _tipoSelezionato, value))
                {
                    OnPropertyChanged(nameof(HasSelection));
                    _isModified = false;
                }
            }
        }

        public bool HasSelection => _tipoSelezionato != null;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsModified
        {
            get => _isModified;
            set => SetProperty(ref _isModified, value);
        }

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

        public string CodiceProposto
        {
            get => _codiceProposto;
            set => SetProperty(ref _codiceProposto, value);
        }

        #endregion

        #region Commands/Actions

        public async void NuovoPassaporto()
        {
            try
            {
                // Verifica se ci sono modifiche non salvate
                if (IsModified)
                {
                    var result = MessageBox.Show(
                        "SALVARE LE MODIFICHE APPORTATE ALLA SCHEDA CORRENTE?",
                        "Conferma",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question,
                        MessageBoxResult.Cancel);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (!await SalvaPassaportoInternal())
                            return;
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }

                // Crea nuovo tipo
                var nuovoTipo = new PassPianteCeeTipo
                {
                    IdPassPianteCEE_Tipo = 0,
                    IdPassPianteCEE_Numerazione = Numerazioni.FirstOrDefault()?.IdPassPianteCEE_Numerazione ?? 0,
                    Descrizione = string.Empty,
                    ServizioFitosanitario = string.Empty,
                    CodiceProduttore = string.Empty,
                    CodiceProduttoreOrig = string.Empty,
                    PaeseOrigine = string.Empty,
                    DescrizioneStamp = string.Empty,
                    StampaTesserino = 0,
                    PassaportoCEE = 0,
                    DocumentoCommerc = 0,
                    CatCertCAC = 0,
                    Dal = null,
                    Al = null,
                    Raggruppamento = 0
                };

                TipoSelezionato = nuovoTipo;
                CodiceProposto = "Nuovo";
                IsModified = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la creazione di un nuovo passaporto: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void SalvaPassaporto()
        {
            await SalvaPassaportoInternal();
        }

        private async Task<bool> SalvaPassaportoInternal()
        {
            if (TipoSelezionato == null)
            {
                MessageBox.Show("Nessun passaporto selezionato da salvare.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                // Validazioni come nel codice VB6
                if (TipoSelezionato.IdPassPianteCEE_Numerazione == 0)
                {
                    MessageBox.Show("IMPOSSIBILE SALVARE!\nIndica la Numerazione.",
                        "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(TipoSelezionato.Descrizione))
                {
                    MessageBox.Show("IMPOSSIBILE SALVARE!\nIndica la Descrizione.",
                        "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(TipoSelezionato.Dal.ToString()))
                {
                    MessageBox.Show("IMPOSSIBILE SALVARE!\nIndica la data inizio validità.",
                        "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(TipoSelezionato.Al.ToString()))
                {
                    MessageBox.Show("IMPOSSIBILE SALVARE!\nIndica la data fine validità.",
                        "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                IsLoading = true;

                bool isNew = TipoSelezionato.IdPassPianteCEE_Tipo == 0;
                var tipoSalvato = await _tipoService.SaveAsync(TipoSelezionato);

                if (isNew)
                {
                    // Aggiungi alla collezione
                    Tipi.Add(tipoSalvato);
                    TipoSelezionato = tipoSalvato;
                }
                else
                {
                    // Aggiorna nella collezione
                    var index = Tipi.ToList().FindIndex(t => t.IdPassPianteCEE_Tipo == tipoSalvato.IdPassPianteCEE_Tipo);
                    if (index >= 0)
                    {
                        Tipi[index] = tipoSalvato;
                        TipoSelezionato = tipoSalvato;
                    }
                }

                CodiceProposto = tipoSalvato.IdPassPianteCEE_Tipo.ToString();
                IsModified = false;

                MessageBox.Show("Passaporto salvato con successo!",
                    "Successo", MessageBoxButton.OK, MessageBoxImage.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il salvataggio: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void CancellaPassaporto()
        {
            if (TipoSelezionato == null)
            {
                MessageBox.Show("Nessun passaporto selezionato da cancellare.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                "CONFERMI LA CANCELLAZIONE DELLA SCHEDA CORRENTE?",
                "Conferma Cancellazione",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.Cancel);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoading = true;

                if (TipoSelezionato.IdPassPianteCEE_Tipo > 0)
                {
                    var success = await _tipoService.DeleteAsync(TipoSelezionato.IdPassPianteCEE_Tipo);
                    if (success)
                    {
                        Tipi.Remove(TipoSelezionato);
                        MessageBox.Show("Passaporto cancellato con successo!",
                            "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                // Seleziona il prossimo elemento o azzera
                TipoSelezionato = Tipi.FirstOrDefault();
                CodiceProposto = TipoSelezionato?.IdPassPianteCEE_Tipo.ToString() ?? string.Empty;
                IsModified = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la cancellazione: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void AnnullaModifiche()
        {
            if (!IsModified)
                return;

            var result = MessageBox.Show(
                "Annullare le modifiche apportate?",
                "Conferma",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Ricarica il tipo selezionato dal database
                if (TipoSelezionato?.IdPassPianteCEE_Tipo > 0)
                {
                    _ = RicaricaTipoSelezionato();
                }
                else
                {
                    TipoSelezionato = null;
                    CodiceProposto = string.Empty;
                }

                IsModified = false;
            }
        }

        public async Task CercaPassaporti()
        {
            try
            {
                // Verifica modifiche non salvate come nel codice VB6
                if (IsModified)
                {
                    var result = MessageBox.Show(
                        "SALVARE LE MODIFICHE APPORTATE ALLA SCHEDA CORRENTE?",
                        "Conferma",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question,
                        MessageBoxResult.Cancel);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (!await SalvaPassaportoInternal())
                            return;
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }

                IsLoading = true;

                var tipi = await _tipoService.GetAllAsync(FiltroCodice, FiltroDescrizione);

                Tipi.Clear();
                foreach (var tipo in tipi)
                {
                    Tipi.Add(tipo);
                }

                TipoSelezionato = Tipi.FirstOrDefault();
                CodiceProposto = TipoSelezionato?.IdPassPianteCEE_Tipo.ToString() ?? string.Empty;
                IsModified = false;
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

        public void SelezionaPassaporto(string codice, DateTime? dal, DateTime? al,
            string numerazione, string descrizione, string descrizioneStampare,
            string servizioFitosanitario, string codiceProduttore,
            string codiceProduttoreOriginario, bool passaportoPianteCee,
            bool categoriaC, bool documentoCommercializzazione,
            bool stampaTesserino, string paeseDiOrigine)
        {
            // Questo metodo viene mantenuto per compatibilità con il codice esistente
            // ma la selezione viene gestita direttamente tramite TipoSelezionato
            if (short.TryParse(codice, out short id))
            {
                var tipo = Tipi.FirstOrDefault(t => t.IdPassPianteCEE_Tipo == id);
                if (tipo != null)
                {
                    TipoSelezionato = tipo;
                }
            }
        }

        public void SegnaModifica()
        {
            if (!IsLoading)
            {
                IsModified = true;
            }
        }

        #endregion

        #region Private Methods

        public async Task CaricaAsync()
        {
            try
            {
                IsLoading = true;

                // Carica numerazioni per le combobox
                var numerazioni = await _numerazioneService.GetAllAsync();
                Numerazioni.Clear();

                // Aggiungi elemento vuoto per permettere nessuna selezione
                Numerazioni.Add(new PassPianteCeeNumerazione
                {
                    IdPassPianteCEE_Numerazione = 0,
                    Descrizione = ""
                });

                foreach (var numerazione in numerazioni)
                {
                    Numerazioni.Add(numerazione);
                }

                // Carica tutti i tipi inizialmente
                await CercaPassaporti();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il caricamento dei dati: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task RicaricaTipoSelezionato()
        {
            if (TipoSelezionato?.IdPassPianteCEE_Tipo > 0)
            {
                try
                {
                    var tipoAggiornato = await _tipoService.GetByIdAsync(TipoSelezionato.IdPassPianteCEE_Tipo);
                    if (tipoAggiornato != null)
                    {
                        TipoSelezionato = tipoAggiornato;
                        CodiceProposto = tipoAggiornato.IdPassPianteCEE_Tipo.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore durante il ricaricamento: {ex.Message}",
                        "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}