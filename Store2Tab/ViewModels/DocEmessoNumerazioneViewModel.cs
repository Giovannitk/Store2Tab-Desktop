using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Store2Tab.Core.Interfaces;
using Store2Tab.Core.Services;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.ViewModels
{
    public class DocEmessoNumerazioneViewModel : INotifyPropertyChanged
    {
        private readonly IDocEmessoNumerazioneService _service;
        private DocEmessoNumerazione? _currentEntity;
        private bool _isDirty;
        private bool _isInEditMode;
        private bool _isLoading;

        private string _codice = string.Empty;
        private string _descrizione = string.Empty;
        private string _sigla = string.Empty;
        private bool _documentoElettronico;
        private string _feTipoDoc = string.Empty;
        private string _stampa = string.Empty;

        private string _codiceRicerca = string.Empty;
        private string _descrizioneRicerca = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public DocEmessoNumerazioneViewModel()
        {
            var factory = new DefaultAppDbContextFactory();
            _service = new DocEmessoNumerazioneService(factory);
            Documenti = new ObservableCollection<DocEmessoNumerazioneItem>();
            _ = LoadDataAsync();
        }

        public ObservableCollection<DocEmessoNumerazioneItem> Documenti { get; }

        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanSaveOrCancel));
                }
            }
        }

        public bool IsInEditMode
        {
            get => _isInEditMode;
            set
            {
                if (_isInEditMode != value)
                {
                    _isInEditMode = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanSaveOrCancel));
                }
            }
        }

        public bool CanSaveOrCancel => IsInEditMode || IsDirty;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Codice
        {
            get => _codice;
            set { if (_codice != value) { _codice = value; OnPropertyChanged(); } }
        }

        public string Descrizione
        {
            get => _descrizione;
            set { if (_descrizione != value) { _descrizione = value; OnPropertyChanged(); MarkAsModified(); } }
        }

        public string Sigla
        {
            get => _sigla;
            set { if (_sigla != value) { _sigla = value; OnPropertyChanged(); MarkAsModified(); } }
        }

        public bool DocumentoElettronico
        {
            get => _documentoElettronico;
            set { if (_documentoElettronico != value) { _documentoElettronico = value; OnPropertyChanged(); MarkAsModified(); } }
        }

        public string FETipoDoc
        {
            get => _feTipoDoc;
            set { if (_feTipoDoc != value) { _feTipoDoc = value; OnPropertyChanged(); MarkAsModified(); } }
        }

        public string DocEmessoTipoStampa
        {
            get => _stampa;
            set { if (_stampa != value) { _stampa = value; OnPropertyChanged(); MarkAsModified(); } }
        }

        public string CodiceRicerca
        {
            get => _codiceRicerca;
            set { if (_codiceRicerca != value) { _codiceRicerca = value; OnPropertyChanged(); } }
        }

        public string DescrizioneRicerca
        {
            get => _descrizioneRicerca;
            set { if (_descrizioneRicerca != value) { _descrizioneRicerca = value; OnPropertyChanged(); } }
        }

        private void MarkAsModified()
        {
            if (!IsLoading)
            {
                IsDirty = true;
                IsInEditMode = true;
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                var data = await _service.GetAllAsync();
                Documenti.Clear();
                foreach (var item in data)
                {
                    Documenti.Add(new DocEmessoNumerazioneItem
                    {
                        Codice = item.IdDocEmessoNumerazione.ToString(),
                        Descrizione = item.DocEmessoNumerazioneDescrizione,
                        Sigla = item.NumerazioneSigla,
                        DocumentoElettronico = item.DocumentoElettronico == 1,
                        FETipoDoc = item.FE_TipoDoc,
                        DocEmessoTipoStampa = item.DocEmessoTipo_Stampa,
                        IsSelected = false
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nel caricamento dei dati: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void NuovaNumerazione()
        {
            if (IsInEditMode)
            {
                var result = MessageBox.Show(
                    "SALVARE LE MODIFICHE APPORTATE ALLA NUMERAZIONE CORRENTE?",
                    "Conferma",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Cancel);

                if (result == MessageBoxResult.Yes)
                {
                    await SalvaAsync();
                    if (IsInEditMode) return;
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            _currentEntity = null;
            Codice = string.Empty;
            Descrizione = string.Empty;
            Sigla = string.Empty;
            DocumentoElettronico = false;
            FETipoDoc = string.Empty;
            DocEmessoTipoStampa = string.Empty;
            IsDirty = false;
            IsInEditMode = true;
        }

        public async void SalvaNumerazione()
        {
            await SalvaAsync();
        }

        private async Task<bool> SalvaAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Descrizione))
                {
                    MessageBox.Show("IMPOSSIBILE SALVARE!\nIndica la Numerazione (descrizione).", "Errore", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                IsLoading = true;
                var isNew = _currentEntity == null;

                if (isNew)
                {
                    var nuova = new DocEmessoNumerazione
                    {
                        DocEmessoNumerazioneDescrizione = Descrizione,
                        NumerazioneSigla = Sigla ?? string.Empty,
                        DocumentoElettronico = (byte)(DocumentoElettronico ? 1 : 0),
                        FE_TipoDoc = FETipoDoc ?? string.Empty,
                        DocEmessoTipo_Stampa = DocEmessoTipoStampa ?? string.Empty
                    };

                    var created = await _service.CreateAsync(nuova);
                    _currentEntity = created;
                    Codice = created.IdDocEmessoNumerazione.ToString();

                    Documenti.Add(new DocEmessoNumerazioneItem
                    {
                        Codice = Codice,
                        Descrizione = Descrizione,
                        Sigla = Sigla,
                        DocumentoElettronico = DocumentoElettronico,
                        FETipoDoc = FETipoDoc,
                        DocEmessoTipoStampa = DocEmessoTipoStampa,
                        IsSelected = false
                    });
                }
                else
                {
                    _currentEntity.DocEmessoNumerazioneDescrizione = Descrizione;
                    _currentEntity.NumerazioneSigla = Sigla ?? string.Empty;
                    _currentEntity.DocumentoElettronico = (byte)(DocumentoElettronico ? 1 : 0);
                    _currentEntity.FE_TipoDoc = FETipoDoc ?? string.Empty;
                    _currentEntity.DocEmessoTipo_Stampa = DocEmessoTipoStampa ?? string.Empty;

                    await _service.UpdateAsync(_currentEntity);

                    var existingItem = Documenti.FirstOrDefault(n => n.Codice == Codice);
                    if (existingItem != null)
                    {
                        existingItem.Descrizione = Descrizione;
                        existingItem.Sigla = Sigla;
                        existingItem.DocumentoElettronico = DocumentoElettronico;
                        existingItem.FETipoDoc = FETipoDoc;
                        existingItem.DocEmessoTipoStampa = DocEmessoTipoStampa;
                    }
                }

                IsDirty = false;
                IsInEditMode = false;
                MessageBox.Show("Numerazione salvata con successo!", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
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

        public async void CancellaNumerazione()
        {
            try
            {
                // Selezioni multiple
                var selezionati = Documenti.Where(d => d.IsSelected).ToList();
                if (selezionati.Count > 1)
                {
                    var confermaMultipla = MessageBox.Show("Si sta per procedere alla cancellazione di più numerazioni. Confermare l'operazione.",
                        "Conferma", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (confermaMultipla != MessageBoxResult.OK) return;

                    IsLoading = true;
                    var risultati = new System.Collections.Generic.List<string>();
                    var cancellate = new System.Collections.Generic.List<DocEmessoNumerazioneItem>();
                    foreach (var num in selezionati)
                    {
                        var conferma = MessageBox.Show($"Cancellare la numerazione:\n\nCodice: {num.Codice}\nDescrizione: {num.Descrizione}",
                            "Conferma Cancellazione", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        if (conferma == MessageBoxResult.Cancel) break;
                        if (conferma == MessageBoxResult.Yes)
                        {
                            try
                            {
                                if (short.TryParse(num.Codice, out short id) && id > 0)
                                {
                                    await _service.DeleteAsync(id);
                                    cancellate.Add(num);
                                    risultati.Add($"✓ {num.Descrizione} - Cancellato");
                                }
                                else
                                {
                                    risultati.Add($"✗ {num.Descrizione} - ID non valido");
                                }
                            }
                            catch (Exception ex)
                            {
                                risultati.Add($"✗ {num.Descrizione} - Errore: {ex.Message}");
                            }
                        }
                        else
                        {
                            risultati.Add($"○ {num.Descrizione} - Saltato");
                        }
                    }

                    foreach (var num in cancellate)
                    {
                        Documenti.Remove(num);
                    }

                    // Reset dettagli
                    Codice = string.Empty;
                    Descrizione = string.Empty;
                    Sigla = string.Empty;
                    DocumentoElettronico = false;
                    FETipoDoc = string.Empty;
                    DocEmessoTipoStampa = string.Empty;
                    IsDirty = false;
                    IsInEditMode = false;

                    IsLoading = false;

                    MessageBox.Show("Riepilogo cancellazione multipla:\n\n" + string.Join("\n", risultati),
                        "Riepilogo Cancellazione", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (string.IsNullOrEmpty(Codice))
                {
                    MessageBox.Show("Nessuna numerazione selezionata.", "Avviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    "CONFERMI LA CANCELLAZIONE DELLA NUMERAZIONE CORRENTE?",
                    "Conferma",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Cancel);

                if (result == MessageBoxResult.Yes)
                {
                    IsLoading = true;
                    if (short.TryParse(Codice, out short id))
                    {
                        await _service.DeleteAsync(id);
                        var item = Documenti.FirstOrDefault(n => n.Codice == Codice);
                        if (item != null)
                        {
                            Documenti.Remove(item);
                        }

                        Codice = string.Empty;
                        Descrizione = string.Empty;
                        Sigla = string.Empty;
                        DocumentoElettronico = false;
                        FETipoDoc = string.Empty;
                        DocEmessoTipoStampa = string.Empty;
                        IsDirty = false;
                        IsInEditMode = false;

                        MessageBox.Show("Numerazione cancellata con successo!", "Successo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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

        public async void AnnullaModifiche()
        {
            if (!IsInEditMode) return;

            try
            {
                if (_currentEntity != null && !string.IsNullOrEmpty(Codice))
                {
                    var fresh = await _service.GetByIdAsync(_currentEntity.IdDocEmessoNumerazione);
                    if (fresh != null)
                    {
                        IsLoading = true;
                        _currentEntity = fresh;
                        Codice = fresh.IdDocEmessoNumerazione.ToString();
                        Descrizione = fresh.DocEmessoNumerazioneDescrizione;
                        Sigla = fresh.NumerazioneSigla;
                        DocumentoElettronico = fresh.DocumentoElettronico == 1;
                        FETipoDoc = fresh.FE_TipoDoc;
                        DocEmessoTipoStampa = fresh.DocEmessoTipo_Stampa;
                        IsLoading = false;
                    }
                }
                else
                {
                    Codice = string.Empty;
                    Descrizione = string.Empty;
                    Sigla = string.Empty;
                    DocumentoElettronico = false;
                    FETipoDoc = string.Empty;
                    DocEmessoTipoStampa = string.Empty;
                }

                IsDirty = false;
                IsInEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore annullamento: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void CercaNumerazioni()
        {
            if (IsInEditMode)
            {
                var result = MessageBox.Show(
                    "SALVARE LE MODIFICHE APPORTATE ALLA NUMERAZIONE CORRENTE?",
                    "Conferma",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question,
                    MessageBoxResult.Cancel);

                if (result == MessageBoxResult.Yes)
                {
                    await SalvaAsync();
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
                var results = await _service.SearchAsync(CodiceRicerca, DescrizioneRicerca);
                Documenti.Clear();
                foreach (var item in results)
                {
                    Documenti.Add(new DocEmessoNumerazioneItem
                    {
                        Codice = item.IdDocEmessoNumerazione.ToString(),
                        Descrizione = item.DocEmessoNumerazioneDescrizione,
                        Sigla = item.NumerazioneSigla,
                        DocumentoElettronico = item.DocumentoElettronico == 1,
                        FETipoDoc = item.FE_TipoDoc,
                        DocEmessoTipoStampa = item.DocEmessoTipo_Stampa,
                        IsSelected = false
                    });
                }

                _currentEntity = null;
                Codice = string.Empty;
                Descrizione = string.Empty;
                Sigla = string.Empty;
                DocumentoElettronico = false;
                FETipoDoc = string.Empty;
                DocEmessoTipoStampa = string.Empty;
                IsDirty = false;
                IsInEditMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore durante la ricerca: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void SelezionaNumerazione(string codice)
        {
            try
            {
                if (short.TryParse(codice, out short id))
                {
                    IsLoading = true;
                    _currentEntity = await _service.GetByIdAsync(id);
                    if (_currentEntity != null)
                    {
                        Codice = _currentEntity.IdDocEmessoNumerazione.ToString();
                        Descrizione = _currentEntity.DocEmessoNumerazioneDescrizione;
                        Sigla = _currentEntity.NumerazioneSigla;
                        DocumentoElettronico = _currentEntity.DocumentoElettronico == 1;
                        FETipoDoc = _currentEntity.FE_TipoDoc;
                        DocEmessoTipoStampa = _currentEntity.DocEmessoTipo_Stampa;
                        IsDirty = false;
                        IsInEditMode = false;
                    }
                    IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nel caricamento della numerazione: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                IsLoading = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DocEmessoNumerazioneItem : INotifyPropertyChanged
    {
        private string _codice = string.Empty;
        private string _descrizione = string.Empty;
        private string _sigla = string.Empty;
        private bool _documentoElettronico;
        private string _feTipoDoc = string.Empty;
        private string _stampa = string.Empty;
        private bool _isSelected;

        public string Codice { get => _codice; set { if (_codice != value) { _codice = value; OnPropertyChanged(); } } }
        public string Descrizione { get => _descrizione; set { if (_descrizione != value) { _descrizione = value; OnPropertyChanged(); } } }
        public string Sigla { get => _sigla; set { if (_sigla != value) { _sigla = value; OnPropertyChanged(); } } }
        public bool DocumentoElettronico { get => _documentoElettronico; set { if (_documentoElettronico != value) { _documentoElettronico = value; OnPropertyChanged(); } } }
        public string FETipoDoc { get => _feTipoDoc; set { if (_feTipoDoc != value) { _feTipoDoc = value; OnPropertyChanged(); } } }
        public string DocEmessoTipoStampa { get => _stampa; set { if (_stampa != value) { _stampa = value; OnPropertyChanged(); } } }
        public bool IsSelected { get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

