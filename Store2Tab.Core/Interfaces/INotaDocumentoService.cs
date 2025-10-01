using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Core.Interfaces
{
    public interface INotaDocumentoService
    {
        /// <summary>
        /// Ottiene tutte le note documento con filtro opzionale
        /// </summary>
        /// <param name="filtroCodice"></param>
        /// <param name="filtroNota"></param>
        /// <returns></returns>
        Task<List<Data.Models.NotaDocumento>> GetAllAsync(string? filtroCodice = null, string? filtroNota = null);

        /// <summary>
        /// Ottiene una nota documento per ID
        /// </summary>
        /// <param name="id">ID della nota</param>
        /// <returns>La nota oppure null se non trovata</returns>
        Task<Data.Models.NotaDocumento?> GetByIdAsync(short id);

        /// <summary>
        /// Salva una nota documento (inserimento o aggiornamento)
        /// </summary>
        /// <param name="notaDocumento"></param>
        /// <returns></returns>
        Task<Data.Models.NotaDocumento> SaveAsync(Data.Models.NotaDocumento notaDocumento);

        /// <summary>
        /// Cancella una nota documento per ID
        /// </summary>
        /// <param name="id">ID del tipo nota documento da cancellare</param>
        /// <returns>True se cancellata, false se non trovata</returns>
        Task<bool> DeleteAsync(short id);

        /// <summary>
        /// Verifica se un tipo nota documento esiste
        /// </summary>
        /// <param name="id">ID del tipo nota documento</param>
        /// <returns>True se esiste</returns>
        Task<bool> ExistsAsync(short id);

        /// <summary>
        /// Ottiene il prossimo ID disponibile
        /// </summary>
        /// <returns>Il prossimo ID disponibile</returns>
        Task<short> GetNextIdAsync();
    }
}
