using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class ComponentiViewModel
    {
        public void CancellaComponente()
        {
            MessageBox.Show("Funzionalità di cancellazione componente non ancora implementata.");
        }

        public void NuovoComponente()
        {
            MessageBox.Show("Funzionalità di creazione nuovo componente non ancora implementata.");
        }

        public void SalvaComponente()
        {
            MessageBox.Show("Funzionalità di salvataggio componente non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaComponenti()
        {
            MessageBox.Show("Funzionalità di ricerca componenti non ancora implementata.");
        }

        public void SelezionaComponente(string codice, string descrizione)
        {
            MessageBox.Show($"Selezionato componente: {codice} - {descrizione}");
        }
    }
}