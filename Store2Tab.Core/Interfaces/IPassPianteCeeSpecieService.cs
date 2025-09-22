using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services.Interfaces
{
    public interface IPassPianteCeeSpecieService
    {
        Task<List<PassPianteCeeSpecie>> GetAllAsync(string? filtroCodice = null, string? filtroSpecie = null);
        Task<PassPianteCeeSpecie?> GetByIdAsync(short id);
        Task<PassPianteCeeSpecie> SaveAsync(PassPianteCeeSpecie specie);
        Task<bool> DeleteAsync(short id);
        Task<bool> ExistsAsync(short id);
        Task<short> GetNextIdAsync();
    }
}