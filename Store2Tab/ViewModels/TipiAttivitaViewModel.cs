using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class TipiAttivitaViewModel
    {
        public void CancellaTipoAttivita()
        {
            MessageBox.Show("Funzionalità di cancellazione tipo attività non ancora implementata.");
        }

        public void NuovoTipoAttivita()
        {
            MessageBox.Show("Funzionalità di creazione nuovo tipo attività non ancora implementata.");
        }

        public void SalvaTipoAttivita()
        {
            MessageBox.Show("Funzionalità di salvataggio tipo attività non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaTipiAttivita()
        {
            MessageBox.Show("Funzionalità di ricerca tipi attività non ancora implementata.");
        }

        public void SelezionaTipoAttivita(string codice, string descrizione)
        {
            MessageBox.Show($"Selezionato tipo attività: {codice} - {descrizione}");
        }
    }
}