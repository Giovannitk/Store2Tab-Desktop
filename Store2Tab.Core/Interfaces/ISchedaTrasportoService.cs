using Store2Tab.Data.Models;
using System.Threading.Tasks;

namespace Store2Tab.Services
{
    public interface ISchedaTrasportoService
    {
        /// <summary>
        /// Carica la scheda trasporto esistente (ce ne dovrebbe essere solo una)
        /// </summary>
        Task<SchedaTrasporto?> CaricaSchedaTrasportoAsync();

        /// <summary>
        /// Salva o aggiorna la scheda trasporto
        /// </summary>
        /// <param name="scheda">Scheda da salvare</param>
        /// <returns>True se l'operazione ha successo</returns>
        Task<bool> SalvaSchedaTrasportoAsync(SchedaTrasporto scheda);

        /// <summary>
        /// Cancella la scheda trasporto corrente
        /// </summary>
        /// <param name="idScheda">ID della scheda da cancellare</param>
        /// <returns>True se l'operazione ha successo</returns>
        Task<bool> CancellaSchedaTrasportoAsync(int idScheda);

        /// <summary>
        /// Verifica se esiste già una scheda trasporto
        /// </summary>
        Task<bool> EsisteSchedaTrasportoAsync();
    }
}