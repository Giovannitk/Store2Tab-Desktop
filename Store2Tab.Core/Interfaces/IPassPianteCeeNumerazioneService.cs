using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services.Interfaces
{
    public interface IPassPianteCeeNumerazioneService
    {
        Task<List<PassPianteCeeNumerazione>> GetAllAsync(string? filtroCodice = null, string? filtroDescrizione = null);
        Task<PassPianteCeeNumerazione?> GetByIdAsync(int id);
        Task<PassPianteCeeNumerazione> SaveAsync(PassPianteCeeNumerazione numerazione);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetNextIdAsync();
    }
}