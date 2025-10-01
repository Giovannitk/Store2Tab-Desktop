using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Core.Interfaces
{
    public interface IProtocolloService
    {
        /// <summary>
        /// Ottiene tutti i protocolli con filtro opzionale
        /// </summary>
        /// <param name="filtroCodice"></param>
        /// <param name="filtroProtocollo"></param>
        /// <returns></returns>
        Task<List<Data.Models.Protocollo>> GetAllAsync(string? filtroCodice = null, string? filtroProtocollo = null);
        
        /// <summary>
        /// Ottiene un protocollo per ID
        /// </summary>
        /// <param name="id">ID del protocollo</param>
        /// <returns>Il protocollo oppure null se non trovato</returns>
        Task<Data.Models.Protocollo?> GetByIdAsync(short id);
        
        /// <summary>
        /// Salva un protocollo (inserimento o aggiornamento)
        /// </summary>
        /// <param name="protocollo"></param>
        /// <returns></returns>
        Task<Data.Models.Protocollo> SaveAsync(Data.Models.Protocollo protocollo);
        
        /// <summary>
        /// Cancella un protocollo per ID
        /// </summary>
        /// <param name="id">ID del protocollo da cancellare</param>
        /// <returns>True se cancellato, false se non trovato</returns>
        Task<bool> DeleteAsync(short id);
       
        /// <summary>
        /// Verifica se un protocollo esiste
        /// </summary>
        /// <param name="id">ID del protocollo</param>
        /// <returns>True se esiste</returns>
        Task<bool> ExistsAsync(short id);
        
        /// <summary>
        /// Ottiene il prossimo ID disponibile
        /// </summary>
        /// <returns>Il prossimo ID disponibile</returns>
        Task<short> GetNextIdAsync();
    }
}
