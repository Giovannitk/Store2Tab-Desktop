using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class DepositiViewModel
    {
        public void CancellaDeposito()
        {
            MessageBox.Show("Funzionalità di cancellazione deposito non ancora implementata.");
        }

        public void NuovoDeposito()
        {
            MessageBox.Show("Funzionalità di creazione nuovo deposito non ancora implementata.");
        }

        public void SalvaDeposito()
        {
            MessageBox.Show("Funzionalità di salvataggio deposito non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaDepositi()
        {
            MessageBox.Show("Funzionalità di ricerca depositi non ancora implementata.");
        }

        public void SelezionaDeposito(string codice, string descrizione, bool depositoPrincipale,
                                     bool visualizzaNelleRicerche, string sigla, string ordineVisualizzazione)
        {
            MessageBox.Show($"Selezionato deposito: {codice} - {descrizione}");
        }
    }
}