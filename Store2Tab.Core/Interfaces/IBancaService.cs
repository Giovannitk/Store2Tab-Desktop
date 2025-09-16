// Store2Tab.Core/Services/Interfaces/IBancaService.cs
using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services.Interfaces
{
    public interface IBancaService
    {
        Task<IEnumerable<Banca>> GetAllBancheAsync();
        Task<Banca?> GetBancaByIdAsync(short id);
        Task<IEnumerable<Banca>> SearchBancheAsync(string searchTerm);
        Task<Banca> CreateBancaAsync(Banca banca, bool licenzaScaduta = false, Func<string, bool>? confermaUtente = null);
        Task<Banca> UpdateBancaAsync(Banca banca, Func<string, bool>? confermaUtente = null);
        Task DeleteBancaAsync(short id);
        Task<bool> BancaExistsAsync(short id);
        Task<Banca?> GetBancaPredefinitaAsync();
        Task SetBancaPredefinitaAsync(short idBanca);
        Task<bool> ValidateBancaAsync(Banca banca);
    }
}