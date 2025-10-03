using Store2Tab.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store2Tab.Core.Interfaces
{
    public interface IDocEmessoNumerazioneService
    {
        Task<List<DocEmessoNumerazione>> GetAllAsync();
        Task<List<DocEmessoNumerazione>> SearchAsync(string codice, string descrizione);
        Task<DocEmessoNumerazione?> GetByIdAsync(short id);
        Task<DocEmessoNumerazione> CreateAsync(DocEmessoNumerazione entity);
        Task<DocEmessoNumerazione> UpdateAsync(DocEmessoNumerazione entity);
        Task<bool> DeleteAsync(short id);
        Task<bool> DeleteMultipleAsync(List<short> ids);
        Task<bool> ExistsAsync(short id);
    }
}

