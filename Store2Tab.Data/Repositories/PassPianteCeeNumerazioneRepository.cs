using Microsoft.EntityFrameworkCore;
using Store2Tab.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store2Tab.Data.Repositories
{
    public class PassPianteCeeNumerazioneRepository : IPassPianteCeeNumerazioneRepository
    {
        private readonly AppDbContext _context;

        public PassPianteCeeNumerazioneRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PassPianteCeeNumerazione>> GetAllAsync(string? filtroCodice = null, string? filtroDescrizione = null)
        {
            var query = _context.PassPianteCeeNumerazioni.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroCodice) && short.TryParse(filtroCodice, out var code))
                query = query.Where(m => m.IdPassPianteCEE_Numerazione == code);

            if (!string.IsNullOrWhiteSpace(filtroDescrizione))
                query = query.Where(m => m.Descrizione.Contains(filtroDescrizione));

            return await query.OrderBy(m => m.Descrizione).ToListAsync();
        }

        public async Task<PassPianteCeeNumerazione?> GetByIdAsync(short id) =>
            await _context.PassPianteCeeNumerazioni.FirstOrDefaultAsync(m => m.IdPassPianteCEE_Numerazione == id);

        public async Task<PassPianteCeeNumerazione> AddAsync(PassPianteCeeNumerazione entity)
        {
            _context.PassPianteCeeNumerazioni.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<PassPianteCeeNumerazione> UpdateAsync(PassPianteCeeNumerazione entity)
        {
            _context.PassPianteCeeNumerazioni.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var entity = await _context.PassPianteCeeNumerazioni.FindAsync(id);
            if (entity == null) return false;

            _context.PassPianteCeeNumerazioni.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(short id) =>
            await _context.PassPianteCeeNumerazioni.AnyAsync(m => m.IdPassPianteCEE_Numerazione == id);

        public async Task<short> GetNextIdAsync()
        {
            var max = await _context.PassPianteCeeNumerazioni
                .Select(m => (short?)m.IdPassPianteCEE_Numerazione)
                .MaxAsync();
            return (short)((max ?? 0) + 1);
        }
    }
}
