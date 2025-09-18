using Store2Tab.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Data.Repositories.Interfaces
{
    public interface IPagamentoMezzoRepository
    {
        Task<IEnumerable<PagamentoMezzo>> GetAllAsync();
        Task<PagamentoMezzo?> GetByIdAsync(short id);
        Task<PagamentoMezzo> AddAsync(PagamentoMezzo pagamentoMezzo);
        Task<PagamentoMezzo> UpdateAsync(PagamentoMezzo pagamentoMezzo);
        Task DeleteAsync(short id);
        Task<bool> ExistsAsync(short id);
    }
}
