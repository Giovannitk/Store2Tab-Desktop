using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class SpecieBotanicheViewModel
    {
        public void CancellaSpecie()
        {
            MessageBox.Show("Funzionalità di cancellazione specie botanica non ancora implementata.");
        }

        public void NuovaSpecie()
        {
            MessageBox.Show("Funzionalità di creazione nuova specie botanica non ancora implementata.");
        }

        public void SalvaSpecie()
        {
            MessageBox.Show("Funzionalità di salvataggio specie botanica non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaSpecie()
        {
            MessageBox.Show("Funzionalità di ricerca specie botaniche non ancora implementata.");
        }

        public void SelezionaSpecie(string codice, string specieBotanica)
        {
            MessageBox.Show($"Selezionata specie botanica: {codice} - {specieBotanica}");
        }
    }
}