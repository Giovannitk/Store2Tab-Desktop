using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class ProtocolliViewModel
    {
        public void CancellaProtocollo()
        {
            MessageBox.Show("Funzionalità di cancellazione protocollo non ancora implementata.");
        }

        public void NuovoProtocollo()
        {
            MessageBox.Show("Funzionalità di creazione nuovo protocollo non ancora implementata.");
        }

        public void SalvaProtocollo()
        {
            MessageBox.Show("Funzionalità di salvataggio protocollo non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaProtocolli()
        {
            MessageBox.Show("Funzionalità di ricerca protocolli non ancora implementata.");
        }

        public void SelezionaProtocollo(string codice, string descrizione)
        {
            MessageBox.Show($"Selezionato protocollo: {codice} - {descrizione}");
        }
    }
}