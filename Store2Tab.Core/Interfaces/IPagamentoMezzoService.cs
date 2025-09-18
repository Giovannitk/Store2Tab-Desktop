using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services.Interfaces
{
    public interface IPagamentoMezzoService
    {
        Task<List<PagamentoMezzo>> GetAllAsync(string? filtroCodice = null, string? filtroDescrizione = null);
        Task<PagamentoMezzo?> GetByIdAsync(short id);
        Task<PagamentoMezzo> SaveAsync(PagamentoMezzo mezzo);
        Task<bool> DeleteAsync(short id);
        Task<bool> ExistsAsync(short id);
        Task<short> GetNextIdAsync();
    }
}


