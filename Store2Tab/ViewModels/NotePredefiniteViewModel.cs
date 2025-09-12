using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class NotePredefiniteViewModel
    {
        public void CancellaNota()
        {
            MessageBox.Show("Funzionalità di cancellazione nota predefinita non ancora implementata.");
        }

        public void NuovaNota()
        {
            MessageBox.Show("Funzionalità di creazione nuova nota predefinita non ancora implementata.");
        }

        public void SalvaNota()
        {
            MessageBox.Show("Funzionalità di salvataggio nota predefinita non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaNote()
        {
            MessageBox.Show("Funzionalità di ricerca note predefinite non ancora implementata.");
        }

        public void SelezionaNota(string codice, string descrizione)
        {
            MessageBox.Show($"Selezionata nota predefinita: {codice} - {descrizione}");
        }

        public void ValidaNota(string codice, string descrizione)
        {
            if (string.IsNullOrWhiteSpace(codice))
            {
                MessageBox.Show("Il codice è obbligatorio.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(descrizione))
            {
                MessageBox.Show("La descrizione della nota è obbligatoria.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Validazione completata con successo.", "Validazione", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}