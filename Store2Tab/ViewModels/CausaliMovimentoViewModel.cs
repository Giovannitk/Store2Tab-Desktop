using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class CausaliMovimentoViewModel
    {
        public void CancellaCausaleMovimento()
        {
            MessageBox.Show("Funzionalità di cancellazione causale movimento non ancora implementata.");
        }

        public void NuovaCausaleMovimento()
        {
            MessageBox.Show("Funzionalità di creazione nuova causale movimento non ancora implementata.");
        }

        public void SalvaCausaleMovimento()
        {
            MessageBox.Show("Funzionalità di salvataggio causale movimento non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaCausaliMovimento()
        {
            MessageBox.Show("Funzionalità di ricerca causali movimento non ancora implementata.");
        }

        public void SelezionaCausaleMovimento(string codice, string descrizione)
        {
            MessageBox.Show($"Selezionata causale movimento: {codice} - {descrizione}");
        }

        public void ApriSelezioneControMovimento()
        {
            MessageBox.Show("Funzionalità di selezione contro movimento non ancora implementata.");
        }
    }
}