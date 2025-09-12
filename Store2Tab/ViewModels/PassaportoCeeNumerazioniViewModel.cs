using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class PassaportoCeeNumerazioniViewModel
    {
        public void CancellaNumerazione()
        {
            MessageBox.Show("Funzionalità di cancellazione numerazione non ancora implementata.");
        }

        public void NuovaNumerazione()
        {
            MessageBox.Show("Funzionalità di creazione nuova numerazione non ancora implementata.");
        }

        public void SalvaNumerazione()
        {
            MessageBox.Show("Funzionalità di salvataggio numerazione non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaNumerazioni()
        {
            MessageBox.Show("Funzionalità di ricerca numerazioni non ancora implementata.");
        }

        public void SelezionaNumerazione(string codice, string descrizione, string sigla, string prefisso)
        {
            MessageBox.Show($"Selezionata numerazione: {codice} - {descrizione}");
        }
    }
}