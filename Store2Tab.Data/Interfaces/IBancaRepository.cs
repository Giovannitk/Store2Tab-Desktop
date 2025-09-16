// Store2Tab.Data/Repositories/Interfaces/IBancaRepository.cs
using Store2Tab.Data.Models;

namespace Store2Tab.Data.Repositories.Interfaces
{
    public interface IBancaRepository
    {
        Task<IEnumerable<Banca>> GetAllAsync();
        Task<Banca?> GetByIdAsync(short id);
        Task<IEnumerable<Banca>> SearchAsync(string searchTerm);
        Task<Banca> AddAsync(Banca banca);
        Task<Banca> UpdateAsync(Banca banca);
        Task DeleteAsync(short id);
        Task<bool> ExistsAsync(short id);
        Task<Banca?> GetPredefinitaAsync();
        Task SetPredefinitaAsync(short idBanca);
        
        // Controllo di unicità 
        Task<bool> ExistsCodiceAsync(string codice, short excludeId = 0);

    }
}