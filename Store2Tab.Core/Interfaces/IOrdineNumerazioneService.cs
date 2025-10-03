using Store2Tab.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store2Tab.Core.Interfaces
{
    public interface IOrdineNumerazioneService
    {
        Task<List<NumerazioneOrdini>> GetAllAsync();
        Task<List<NumerazioneOrdini>> SearchAsync(string codice, string descrizione);
        Task<NumerazioneOrdini?> GetByIdAsync(short id);
        Task<NumerazioneOrdini> CreateAsync(NumerazioneOrdini numerazione);
        Task<NumerazioneOrdini> UpdateAsync(NumerazioneOrdini numerazione);
        Task<bool> DeleteAsync(short id);
        Task<bool> DeleteMultipleAsync(List<short> ids);
        Task<bool> ExistsAsync(short id);
    }
}