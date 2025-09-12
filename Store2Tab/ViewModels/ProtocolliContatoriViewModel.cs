using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class ProtocolliContatoriViewModel
    {
        public void CancellaProtocolloContatore()
        {
            MessageBox.Show("Funzionalità di cancellazione protocollo contatore non ancora implementata.");
        }

        public void NuovoProtocolloContatore()
        {
            MessageBox.Show("Funzionalità di creazione nuovo protocollo contatore non ancora implementata.");
        }

        public void SalvaProtocolloContatore()
        {
            MessageBox.Show("Funzionalità di salvataggio protocollo contatore non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaProtocolliContatori()
        {
            MessageBox.Show("Funzionalità di ricerca protocolli contatori non ancora implementata.");
        }

        public void SelezionaProtocolloContatore(string anno, string protocollo, string contatore)
        {
            MessageBox.Show($"Selezionato: Anno {anno} - Protocollo: {protocollo} - Contatore: {contatore}");
        }
    }
}