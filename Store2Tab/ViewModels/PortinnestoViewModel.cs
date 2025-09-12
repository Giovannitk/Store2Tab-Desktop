using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class PortinnestoViewModel
    {
        public void CancellaPortinnesto()
        {
            MessageBox.Show("Funzionalità di cancellazione portinnesto non ancora implementata.");
        }

        public void NuovoPortinnesto()
        {
            MessageBox.Show("Funzionalità di creazione nuovo portinnesto non ancora implementata.");
        }

        public void SalvaPortinnesto()
        {
            MessageBox.Show("Funzionalità di salvataggio portinnesto non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaPortinnesti()
        {
            MessageBox.Show("Funzionalità di ricerca portinnesti non ancora implementata.");
        }

        public void SelezionaPortinnesto(string codice, string specieBotanica, string portinnesto)
        {
            MessageBox.Show($"Selezionato portinnesto: {codice} - {specieBotanica} - {portinnesto}");
        }
    }
}