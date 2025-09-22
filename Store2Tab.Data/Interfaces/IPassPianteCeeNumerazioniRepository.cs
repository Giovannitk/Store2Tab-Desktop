using Store2Tab.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store2Tab.Data.Repositories
{
    public interface IPassPianteCeeNumerazioneRepository
    {
        Task<List<PassPianteCeeNumerazione>> GetAllAsync(string? filtroCodice = null, string? filtroDescrizione = null);
        Task<PassPianteCeeNumerazione?> GetByIdAsync(short id);
        Task<PassPianteCeeNumerazione> AddAsync(PassPianteCeeNumerazione entity);
        Task<PassPianteCeeNumerazione> UpdateAsync(PassPianteCeeNumerazione entity);
        Task<bool> DeleteAsync(short id);
        Task<bool> ExistsAsync(short id);
        Task<short> GetNextIdAsync();
    }
}
