using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services.Interfaces
{
    /// <summary>
    /// Interfaccia per il servizio di gestione delle varietà del passaporto piante CEE
    /// </summary>
    public interface IPassPianteCeeVarietaService
    {
        /// <summary>
        /// Ottiene tutte le varietà con possibilità di filtri
        /// </summary>
        /// <param name="filtroCodice">Filtro per codice varietà</param>
        /// <param name="filtroVarieta">Filtro per nome varietà</param>
        /// <param name="filtroSpecieId">Filtro per ID specie botanica</param>
        /// <returns>Lista delle varietà</returns>
        Task<List<PassPianteCeeVarieta>> GetAllAsync(string? filtroCodice = null, string? filtroVarieta = null, short? filtroSpecieId = null);

        /// <summary>
        /// Ottiene una varietà per ID
        /// </summary>
        /// <param name="id">ID della varietà</param>
        /// <returns>Varietà trovata o null</returns>
        Task<PassPianteCeeVarieta?> GetByIdAsync(short id);

        /// <summary>
        /// Salva o aggiorna una varietà
        /// </summary>
        /// <param name="varieta">Varietà da salvare</param>
        /// <returns>Varietà salvata</returns>
        Task<PassPianteCeeVarieta> SaveAsync(PassPianteCeeVarieta varieta);

        /// <summary>
        /// Elimina una varietà
        /// </summary>
        /// <param name="id">ID della varietà da eliminare</param>
        /// <returns>True se eliminata con successo</returns>
        Task<bool> DeleteAsync(short id);

        /// <summary>
        /// Verifica se esiste una varietà con l'ID specificato
        /// </summary>
        /// <param name="id">ID da verificare</param>
        /// <returns>True se esiste</returns>
        Task<bool> ExistsAsync(short id);

        /// <summary>
        /// Ottiene il prossimo ID disponibile per una nuova varietà
        /// </summary>
        /// <returns>Prossimo ID</returns>
        Task<short> GetNextIdAsync();

        /// <summary>
        /// Ottiene le varietà per una specifica specie botanica
        /// </summary>
        /// <param name="specieId">ID della specie botanica</param>
        /// <returns>Lista delle varietà della specie</returns>
        Task<List<PassPianteCeeVarieta>> GetBySpecieAsync(short specieId);
    }
}