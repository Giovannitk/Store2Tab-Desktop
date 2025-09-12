using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Store2Tab.ViewModels
{
    public class MezziPagamentoViewModel
    {
        public void CancellaMezzoPagamento()
        {
            MessageBox.Show("Funzionalità di cancellazione mezzo pagamento non ancora implementata.");
        }

        public void NuovoMezzoPagamento()
        {
            MessageBox.Show("Funzionalità di creazione nuovo mezzo pagamento non ancora implementata.");
        }

        public void SalvaMezzoPagamento()
        {
            MessageBox.Show("Funzionalità di salvataggio mezzo pagamento non ancora implementata.");
        }

        public void AnnullaModifiche()
        {
            MessageBox.Show("Funzionalità di annullamento modifiche non ancora implementata.");
        }

        public void CercaMezziPagamento()
        {
            MessageBox.Show("Funzionalità di ricerca mezzi pagamento non ancora implementata.");
        }

        public void SelezionaMezzoPagamento(string descrizione)
        {
            MessageBox.Show($"Selezionato mezzo pagamento: {descrizione}");
        }
    }
}