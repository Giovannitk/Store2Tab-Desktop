using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services.Interfaces
{
    public interface ITipiAttivitaService
    {
        /// <summary>
        /// Ottiene tutti i tipi attività con filtri opzionali
        /// </summary>
        /// <param name="filtroCodice">Filtro per codice (IdAnagraficaAttivita)</param>
        /// <param name="filtroAttivita">Filtro per descrizione attività</param>
        /// <returns>Lista dei tipi attività</returns>
        Task<List<TipiAttivita>> GetAllAsync(string? filtroCodice = null, string? filtroAttivita = null);

        /// <summary>
        /// Ottiene un tipo attività per ID
        /// </summary>
        /// <param name="id">ID del tipo attività</param>
        /// <returns>Il tipo attività o null se non trovato</returns>
        Task<TipiAttivita?> GetByIdAsync(short id);

        /// <summary>
        /// Salva un tipo attività (inserimento o aggiornamento)
        /// </summary>
        /// <param name="tipoAttivita">Il tipo attività da salvare</param>
        /// <returns>Il tipo attività salvato con ID aggiornato</returns>
        Task<TipiAttivita> SaveAsync(TipiAttivita tipoAttivita);

        /// <summary>
        /// Cancella un tipo attività per ID
        /// </summary>
        /// <param name="id">ID del tipo attività da cancellare</param>
        /// <returns>True se cancellato, false se non trovato</returns>
        Task<bool> DeleteAsync(short id);

        /// <summary>
        /// Verifica se un tipo attività esiste
        /// </summary>
        /// <param name="id">ID del tipo attività</param>
        /// <returns>True se esiste</returns>
        Task<bool> ExistsAsync(short id);

        /// <summary>
        /// Ottiene il prossimo ID disponibile
        /// </summary>
        /// <returns>Il prossimo ID disponibile</returns>
        Task<short> GetNextIdAsync();
    }
}