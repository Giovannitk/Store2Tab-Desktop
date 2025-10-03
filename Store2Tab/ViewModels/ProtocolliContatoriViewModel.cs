using Store2Tab.Core.Interfaces;
using Store2Tab.Core.Services;
using Store2Tab.Data;
using Store2Tab.Data.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class ProtocolliContatoriViewModel : INotifyPropertyChanged
    {
        private readonly IProtocolloContatoreService _service;
        private ProtocolloContatoreDto? _originale;

        public ProtocolliContatoriViewModel()
        {
            var factory = new DefaultAppDbContextFactory();
            _service = new ProtocolloContatoreService(factory);
            ProtocolliContatori = new ObservableCollection<ProtocolloContatoreDto>();
            _ = CaricaAsync();
        }

        public ProtocolliContatoriViewModel(IProtocolloContatoreService service)
        {
            _service = service;
            ProtocolliContatori = new ObservableCollection<ProtocolloContatoreDto>();
            _ = CaricaAsync();
        }

        #region Proprietà
        private ObservableCollection<ProtocolloContatoreDto> _protocolliContatori;
        public ObservableCollection<ProtocolloContatoreDto> ProtocolliContatori
        {
            get => _protocolliContatori;
            set { _protocolliContatori = value; OnPropertyChanged(); }
        }

        private ProtocolloContatoreDto? _selezionato;
        public ProtocolloContatoreDto? Selezionato
        {
            get => _selezionato;
            set
            {
                if (_selezionato != value && IsInEditMode)
                {
                    var result = MessageBox.Show(
                        "Ci sono modifiche non salvate. Vuoi salvarle?",
                        "Conferma",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        Task.Run(async () => await SalvaAsync()).Wait();
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

                _selezionato = value;

                if (_selezionato != null)
                {
                    IdProtocollo = _selezionato.IdProtocollo;
                    Esercizio = _selezionato.Esercizio;
                    NomeProtocollo = _selezionato.NomeProtocollo;
                    Contatore = _selezionato.Contatore;

                    _originale = new ProtocolloContatoreDto
                    {
                        IdProtocollo = _selezionato.IdProtocollo,
                        Esercizio = _selezionato.Esercizio,
                        NomeProtocollo = _selezionato.NomeProtocollo,
                        Contatore = _selezionato.Contatore
                    };
                }
                else
                {
                    IdProtocollo = 0;
                    Esercizio = 0;
                    NomeProtocollo = string.Empty;
                    Contatore = 0;
                    _originale = null;
                }

                OnPropertyChanged();
            }
        }

        private short _idProtocollo;
        public short IdProtocollo
        {
            get => _idProtocollo;
            set { _idProtocollo = value; OnPropertyChanged(); }
        }

        private short _esercizio;
        public short Esercizio
        {
            get => _esercizio;
            set { _esercizio = value; OnPropertyChanged(); }
        }

        private string _nomeProtocollo = string.Empty;
        public string NomeProtocollo
        {
            get => _nomeProtocollo;
            set { _nomeProtocollo = value; OnPropertyChanged(); }
        }

        private int _contatore;
        public int Contatore
        {
            get => _contatore;
            set { _contatore = value; OnPropertyChanged(); }
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

        #region Metodi
        public async Task CaricaAsync()
        {
            try
            {
                IsLoading = true;
                var lista = await _service.GetAllAsync();

                ProtocolliContatori.Clear();
                foreach (var item in lista)
                {
                    ProtocolliContatori.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante il caricamento: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> SalvaAsync()
        {
            if (Selezionato == null || IdProtocollo == 0)
            {
                MessageBox.Show("Nessun protocollo contatore selezionato.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                if (Contatore < 0)
                {
                    MessageBox.Show("Il contatore non può essere negativo.",
                        "Errore di validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                IsLoading = true;
                var saved = await _service.UpdateContatoreAsync(IdProtocollo, Esercizio, Contatore);

                // Aggiorna nella collezione
                var existing = ProtocolliContatori.FirstOrDefault(
                    p => p.IdProtocollo == saved.IdProtocollo && p.Esercizio == saved.Esercizio);

                if (existing != null)
                {
                    existing.Contatore = saved.Contatore;
                }

                // Aggiorna il selezionato
                if (Selezionato != null)
                {
                    Selezionato.Contatore = saved.Contatore;
                }

                Contatore = saved.Contatore;

                _originale = new ProtocolloContatoreDto
                {
                    IdProtocollo = saved.IdProtocollo,
                    Esercizio = saved.Esercizio,
                    NomeProtocollo = saved.Protocollo?.NomeProtocollo ?? string.Empty,
                    Contatore = saved.Contatore
                };

                IsInEditMode = false;

                MessageBox.Show("Contatore salvato con successo.",
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

        public async void Elimina()
        {
            if (Selezionato == null || IdProtocollo == 0)
            {
                MessageBox.Show("Nessun protocollo contatore selezionato.",
                    "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Sei sicuro di voler eliminare il contatore per '{NomeProtocollo}' anno {Esercizio}?",
                "Conferma eliminazione",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                IsLoading = true;
                var success = await _service.DeleteAsync(IdProtocollo, Esercizio);

                if (success)
                {
                    ProtocolliContatori.Remove(Selezionato);
                    Selezionato = null;
                    IsInEditMode = false;
                    MessageBox.Show("Protocollo contatore eliminato con successo.",
                        "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Protocollo contatore non trovato.",
                        "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'eliminazione: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void AnnullaModifiche()
        {
            if (!IsInEditMode)
                return;

            try
            {
                if (_originale != null)
                {
                    Contatore = _originale.Contatore;
                    OnPropertyChanged(nameof(Contatore));
                }

                IsInEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante l'annullamento: {ex.Message}",
                    "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void OnContatoreModificato()
        {
            if (!IsInEditMode && Selezionato != null)
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