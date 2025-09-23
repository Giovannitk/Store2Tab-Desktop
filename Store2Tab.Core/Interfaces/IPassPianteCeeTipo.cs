using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services.Interfaces
{
    public interface IPassPianteCeeTipoService
    {
        Task<List<PassPianteCeeTipo>> GetAllAsync(string? filtroCodice = null, string? filtroDescrizione = null);
        Task<PassPianteCeeTipo?> GetByIdAsync(short id);
        Task<PassPianteCeeTipo> SaveAsync(PassPianteCeeTipo tipo);
        Task<bool> DeleteAsync(short id);
        Task<bool> ExistsAsync(short id);
        Task<short> GetNextIdAsync();
        Task<List<PassPianteCeeNumerazione>> GetNumerazioniAsync();
    }
}