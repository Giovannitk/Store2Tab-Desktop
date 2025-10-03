using Store2Tab.Core.Services;
using Store2Tab.Data;
using Store2Tab.Data.Models;
using Store2Tab.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class SchedaTrasportoViewModel : INotifyPropertyChanged
    {
        private readonly ISchedaTrasportoService _schedaTrasportoService;
        private SchedaTrasporto? _schedaCorrente;
        private SchedaTrasporto? _schedaOriginale;
        private bool _isDirty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SchedaTrasportoViewModel(ISchedaTrasportoService schedaTrasportoService)
        {
            _schedaTrasportoService = schedaTrasportoService ?? throw new ArgumentNullException(nameof(schedaTrasportoService));
            _ = CaricaDatiDefaultAsync();
        }

        public SchedaTrasportoViewModel()
        {
            var factory = new DefaultAppDbContextFactory();
            _schedaTrasportoService = new SchedaTrasportoService(factory);
            _ = CaricaDatiDefaultAsync();
        }

        public SchedaTrasporto? SchedaCorrente
        {
            get => _schedaCorrente;
            set
            {
                _schedaCorrente = value;
                OnPropertyChanged();
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanSave));
                    OnPropertyChanged(nameof(CanCancel));
                }
            }
        }

        public bool CanSave => IsDirty;
        public bool CanCancel => IsDirty;

        public async Task<bool> SalvaSchedaTrasportoAsync(SchedaTrasporto scheda)
        {
            if (scheda == null)
            {
                MessageBox.Show("Dati della scheda non validi.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                bool successo = await _schedaTrasportoService.SalvaSchedaTrasportoAsync(scheda);

                if (successo)
                {
                    MessageBox.Show("Scheda trasporto salvata con successo.", "Salvataggio", MessageBoxButton.OK, MessageBoxImage.Information);
                    SchedaCorrente = scheda;

                    // Aggiorna la copia originale e resetta il dirty flag
                    _schedaOriginale = ClonaScheda(scheda);
                    IsDirty = false;

                    // Ricarica per aggiornare l'ID se era una nuova scheda
                    await CaricaDatiDefaultAsync();
                    return true;
                }
                else
                {
                    MessageBox.Show("Errore nel salvataggio della scheda trasporto.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            catch (DbException dbe) 
            {
                MessageBox.Show($"Errore di database: {dbe.Message} - {dbe.InnerException}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore imprevisto: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> CancellaSchedaTrasportoAsync(int idScheda)
        {
            if (idScheda <= 0)
            {
                MessageBox.Show("Nessuna scheda selezionata da cancellare.", "Attenzione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            try
            {
                var result = MessageBox.Show(
                    "Sei sicuro di voler cancellare la scheda trasporto corrente?",
                    "Conferma Cancellazione",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool successo = await _schedaTrasportoService.CancellaSchedaTrasportoAsync(idScheda);

                    if (successo)
                    {
                        MessageBox.Show("Scheda trasporto cancellata con successo.", "Cancellazione", MessageBoxButton.OK, MessageBoxImage.Information);
                        SchedaCorrente = null;
                        _schedaOriginale = null;
                        IsDirty = false;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Errore nella cancellazione della scheda trasporto.", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore imprevisto: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task AnnullaModificheAsync()
        {
            if (!IsDirty)
                return;

            var result = MessageBox.Show(
                "Sei sicuro di voler annullare le modifiche?",
                "Conferma Annulla",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await CaricaDatiDefaultAsync();
                IsDirty = false;
            }
        }

        public async Task CaricaDatiDefaultAsync()
        {
            try
            {
                var scheda = await _schedaTrasportoService.CaricaSchedaTrasportoAsync();

                if (scheda != null)
                {
                    SchedaCorrente = scheda;
                    _schedaOriginale = ClonaScheda(scheda);
                }
                else
                {
                    // Crea una nuova scheda vuota
                    SchedaCorrente = new SchedaTrasporto();
                    _schedaOriginale = ClonaScheda(SchedaCorrente);
                }

                IsDirty = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore nel caricamento dei dati: {ex.Message}", "Errore", MessageBoxButton.OK, MessageBoxImage.Error);
                SchedaCorrente = new SchedaTrasporto();
                _schedaOriginale = ClonaScheda(SchedaCorrente);
            }
        }

        public bool ValidaDatiScheda(SchedaTrasporto scheda)
        {
            if (scheda == null)
                return false;

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(scheda.VettoreDescrizione))
            {
                MessageBox.Show("Il campo 'Dati Vettore' è obbligatorio.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(scheda.CommittenteDescrizione))
            {
                MessageBox.Show("Il campo 'Dati Committente' è obbligatorio.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(scheda.MerceTipologia))
            {
                MessageBox.Show("Il campo 'Tipologia Merce' è obbligatorio.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(scheda.MerceLuogoCarico))
            {
                MessageBox.Show("Il campo 'Luogo di Carico' è obbligatorio.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(scheda.MerceLuogoScarico))
            {
                MessageBox.Show("Il campo 'Luogo di Scarico' è obbligatorio.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                isValid = false;
            }

            return isValid;
        }

        public void SegnaModificato()
        {
            IsDirty = true;
        }

        // Metodo per clonare la scheda (shallow copy)
        private SchedaTrasporto ClonaScheda(SchedaTrasporto scheda)
        {
            if (scheda == null)
                return new SchedaTrasporto();

            return new SchedaTrasporto
            {
                IdSchedaTrasporto = scheda.IdSchedaTrasporto,
                VettoreDescrizione = scheda.VettoreDescrizione,
                VettorePartitaIva = scheda.VettorePartitaIva,
                VettoreAlboAutotrasportatori = scheda.VettoreAlboAutotrasportatori,
                CommittenteDescrizione = scheda.CommittenteDescrizione,
                CommittentePartitaIva = scheda.CommittentePartitaIva,
                CaricatoreDescrizione = scheda.CaricatoreDescrizione,
                CaricatorePartitaIva = scheda.CaricatorePartitaIva,
                ProprietarioDescrizione = scheda.ProprietarioDescrizione,
                ProprietarioPartitaIva = scheda.ProprietarioPartitaIva,
                Dichiarazioni = scheda.Dichiarazioni,
                MerceTipologia = scheda.MerceTipologia,
                MerceQuantitaPeso = scheda.MerceQuantitaPeso,
                MerceLuogoCarico = scheda.MerceLuogoCarico,
                MerceLuogoScarico = scheda.MerceLuogoScarico,
                Luogo = scheda.Luogo,
                Compilatore = scheda.Compilatore
            };
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}