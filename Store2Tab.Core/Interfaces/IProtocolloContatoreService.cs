using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store2Tab.Core.Interfaces
{
    public interface IProtocolloContatoreService
    {
        /// <summary>
        /// Ottiene tutti i protocolli contatori con JOIN su TProtocollo
        /// </summary>
        Task<List<ProtocolloContatoreDto>> GetAllAsync();

        /// <summary>
        /// Ottiene un protocollo contatore specifico
        /// </summary>
        Task<Data.Models.ProtocolloContatore?> GetByIdAsync(short idProtocollo, short esercizio);

        /// <summary>
        /// Aggiorna il contatore (non inserisce, solo update)
        /// </summary>
        Task<Data.Models.ProtocolloContatore> UpdateContatoreAsync(short idProtocollo, short esercizio, int nuovoContatore);

        /// <summary>
        /// Elimina un protocollo contatore
        /// </summary>
        Task<bool> DeleteAsync(short idProtocollo, short esercizio);
    }

    /// <summary>
    /// DTO per il join tra ProtocolloContatore e Protocollo
    /// </summary>
    public class ProtocolloContatoreDto
    {
        public short IdProtocollo { get; set; }
        public short Esercizio { get; set; }
        public string NomeProtocollo { get; set; } = string.Empty;
        public int Contatore { get; set; }
    }
}