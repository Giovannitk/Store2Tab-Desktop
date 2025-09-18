using Microsoft.EntityFrameworkCore;
using Store2Tab.Data.Models;
using Store2Tab.Data.Repositories.Interfaces;

namespace Store2Tab.Data.Repositories
{
    public class PagamentoMezzoRepository : IPagamentoMezzoRepository
    {
        private readonly AppDbContext _context;
        public PagamentoMezzoRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<PagamentoMezzo>> GetAllAsync()
        {
            return await _context.PagamentiMezzo
                .OrderBy(p => p.pagamentoMezzo)
                .ToListAsync();
        }
        public async Task<PagamentoMezzo?> GetByIdAsync(short id)
        {
            return await _context.PagamentiMezzo
                .FirstOrDefaultAsync(e => e.IdPagamentoMezzo == id);
        }
        public async Task<PagamentoMezzo> AddAsync(PagamentoMezzo pagamentoMezzo)
        {
            await _context.PagamentiMezzo.AddAsync(pagamentoMezzo);
            await _context.SaveChangesAsync();
            return pagamentoMezzo;
        }
        public async Task<PagamentoMezzo> UpdateAsync(PagamentoMezzo pagamentoMezzo)
        {
            _context.PagamentiMezzo.Update(pagamentoMezzo);
            await _context.SaveChangesAsync();
            return pagamentoMezzo;
        }
        public async Task DeleteAsync(short id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.PagamentiMezzo.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(short id)
        {
            return await _context.PagamentiMezzo.AnyAsync(e => e.IdPagamentoMezzo == id);
        }
    }
}
