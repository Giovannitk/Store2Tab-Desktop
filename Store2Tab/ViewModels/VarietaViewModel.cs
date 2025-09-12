using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class VarietaViewModel
    {
        public void CancellaVarieta()
        {
            MessageBox.Show("Funzionalità di cancellazione varietà non ancora implementata.");
        }

        public void NuovaVarieta()
        {
            MessageBox.Show("Funzionalità di creazione nuova varietà non ancora implementata.");
        }

        public void SalvaVarieta()
        {
            MessageBox.Show("Funzionalità di salvataggio varietà non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaVarieta()
        {
            MessageBox.Show("Funzionalità di ricerca varietà non ancora implementata.");
        }

        public void SelezionaVarieta(string codice, string specieBotanica, string varieta)
        {
            MessageBox.Show($"Selezionata varietà: {codice} - {specieBotanica} - {varieta}");
        }
    }
}