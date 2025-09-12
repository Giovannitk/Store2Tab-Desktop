using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class PassaportoPianteCeeViewModel
    {
        public void CancellaPassaporto()
        {
            MessageBox.Show("Funzionalità di cancellazione passaporto non ancora implementata.");
        }

        public void NuovoPassaporto()
        {
            MessageBox.Show("Funzionalità di creazione nuovo passaporto non ancora implementata.");
        }

        public void SalvaPassaporto()
        {
            MessageBox.Show("Funzionalità di salvataggio passaporto non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaPassaporti()
        {
            MessageBox.Show("Funzionalità di ricerca passaporti non ancora implementata.");
        }

        public void SelezionaPassaporto(string codice, string validoDal, string validoAl, string numerazione,
                                       string descrizione, string descrizioneStampare, string servizioFitosanitario,
                                       string codiceProduttore, string codiceProduttoreOriginario, bool passaportoPianteCee,
                                       bool categoriaC, bool documentoCommercializzazione, bool stampaTestessino,
                                       string paeseDiOrigine)
        {
            MessageBox.Show($"Selezionato passaporto: {codice} - {descrizione}");
        }
    }
}