using Store2Tab.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Core.Interfaces
{
    public interface IPassPianteCeePortinnestoService
    {
        /// <summary>
        /// Ottiene tutte le portinneto con possibilità di filtri
        /// </summary>
        /// <param name="filtroCodice">Filtro per codice portinnesto</param>
        /// <param name="filtroPortinnesto">Filtro per nome portinnesto</param>
        /// <param name="filtroSpecieId">Filtro per ID specie botanica</param>
        /// <returns>Lista delle portinnesto</returns>
        Task<List<PassPianteCEE_Portinnesto>> GetAllAsync(string? filtroCodice = null, string? filtroPortinnesto = null, short? filtroSpecieId = null);

        /// <summary>
        /// Ottiene una varietà per ID
        /// </summary>
        /// <param name="id">ID del portinnesto</param>
        /// <returns>Portinnesto trovata o null</returns>
        Task<PassPianteCEE_Portinnesto?> GetByIdAsync(short id);


        /// <summary>
        /// Salva o aggiorna un portinnesto
        /// </summary>
        /// <param name="portinnesto">Portinnesto da salvare</param>
        /// <returns>Portinnesto salvato</returns>
        Task<PassPianteCEE_Portinnesto> SaveAsync(PassPianteCEE_Portinnesto portinnesto);

        /// <summary>
        /// Elimina un portinnesto
        /// </summary>
        /// <param name="id">ID del portinnesto da eliminare</param>
        /// <returns>True se eliminato con successo</returns>
        Task<bool> DeleteAsync(short id);

        /// <summary>
        /// Verifica se esiste un portinnesto con l'ID specificato
        /// </summary>
        /// <param name="id">ID da verificare</param>
        /// <returns>True se esiste</returns>
        Task<bool> ExistsAsync(short id);

        /// <summary>
        /// Ottiene il prossimo ID disponibile per un nuovo portinnesto
        /// </summary>
        /// <returns>Prossimo ID</returns>
        Task<short> GetNextIdAsync();

        /// <summary>
        /// Ottiene i portinnesti per una specifica specie botanica
        /// </summary>
        /// <param name="specieId">ID della specie botanica</param>
        /// <returns>Lista dei portinnesti della specie</returns>
        Task<List<PassPianteCEE_Portinnesto>> GetBySpecieAsync(short specieId);
    }
}
