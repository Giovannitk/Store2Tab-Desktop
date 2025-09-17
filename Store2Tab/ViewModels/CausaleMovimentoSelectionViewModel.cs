using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Store2Tab.Data.Models;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Views;

namespace Store2Tab.ViewModels
{
    public class CausaleMovimentoSelectionViewModel : INotifyPropertyChanged
    {
        private readonly ICausaleMovimentoService _causaleService;
        private string _filtroCodice = string.Empty;
        private string _filtroDescrizione = string.Empty;
        private CausaleMovimentoItem? _causaleSelezionata;

        public ObservableCollection<CausaleMovimentoItem> CausaliMovimento { get; set; }

        public CausaleMovimentoSelectionViewModel()
        {
            _causaleService = App.ServiceProvider.GetRequiredService<ICausaleMovimentoService>();
            CausaliMovimento = new ObservableCollection<CausaleMovimentoItem>();

            // Carica tutti i dati inizialmente
            _ = Task.Run(async () => await CercaCausali());
        }

        #region Proprietà

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

        public CausaleMovimentoItem? CausaleSelezionata
        {
            get => _causaleSelezionata;
            set => SetProperty(ref _causaleSelezionata, value);
        }

        #endregion

        #region Metodi pubblici

        public async Task CercaCausali()
        {
            try
            {
                var causali = await _causaleService.CercaCausaliPerSelezioneAsync(
                    string.IsNullOrWhiteSpace(FiltroCodice) && string.IsNullOrWhiteSpace(FiltroDescrizione) ?
                    null :
                    $"{FiltroCodice} {FiltroDescrizione}".Trim()
                );

                // Filtra ulteriormente se necessario
                if (!string.IsNullOrWhiteSpace(FiltroCodice))
                {
                    causali = causali.Where(c => c.Codice.Contains(FiltroCodice, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(FiltroDescrizione))
                {
                    causali = causali.Where(c => c.Descrizione.Contains(FiltroDescrizione, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Aggiorna la collection su UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CausaliMovimento.Clear();
                    foreach (var causale in causali)
                    {
                        CausaliMovimento.Add(new CausaleMovimentoItem
                        {
                            Codice = causale.Codice,
                            Descrizione = causale.Descrizione
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Errore durante la ricerca: {ex.Message}", "Errore",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}