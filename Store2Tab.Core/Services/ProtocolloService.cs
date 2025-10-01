using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Interfaces;
using Store2Tab.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Core.Services
{
    public class ProtocolloService : IProtocolloService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ProtocolloService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Data.Models.Protocollo>> GetAllAsync(string? filtroCodice = null, string? filtroProtocollo = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Set<Data.Models.Protocollo>().AsQueryable();
            if (!string.IsNullOrWhiteSpace(filtroCodice))
            {
                if (short.TryParse(filtroCodice, out var code))
                {
                    query = query.Where(p => p.IdProtocollo == code);
                }
            }
            if (!string.IsNullOrWhiteSpace(filtroProtocollo))
            {
                query = query.Where(p => p.NomeProtocollo.Contains(filtroProtocollo));
            }
            return await query
                .OrderBy(p => p.NomeProtocollo)
                .ToListAsync();
        }

        public async Task<Data.Models.Protocollo?> GetByIdAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<Data.Models.Protocollo>().FirstOrDefaultAsync(p => p.IdProtocollo == id);
        }

        public async Task<Data.Models.Protocollo> SaveAsync(Data.Models.Protocollo protocollo)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                protocollo.NomeProtocollo = protocollo.NomeProtocollo?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(protocollo.NomeProtocollo))
                    throw new ArgumentException("Il nome del protocollo è obbligatorio");

                if (protocollo.IdProtocollo == 0)
                {
                    // Nuovo inserimento
                    context.Set<Data.Models.Protocollo>().Add(protocollo);
                }
                else
                {
                    // Aggiornamento esistente - usa Attach e modifica stato
                    context.Set<Data.Models.Protocollo>().Attach(protocollo);
                    context.Entry(protocollo).State = EntityState.Modified;
                }

                await context.SaveChangesAsync();

                // Refresh per ottenere valori generati dal database
                await context.Entry(protocollo).ReloadAsync();

                return protocollo;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate"))
                {
                    throw new InvalidOperationException("Esiste già un protocollo con lo stesso nome.", ex);
                }
                throw;
            }
        }

        public async Task<bool> DeleteAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var existing = await context.Set<Data.Models.Protocollo>().FirstOrDefaultAsync(p => p.IdProtocollo == id);
            if (existing == null)
                return false;
            context.Set<Data.Models.Protocollo>().Remove(existing);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<Data.Models.Protocollo>().AnyAsync(p => p.IdProtocollo == id);
        }

        public async Task<short> GetNextIdAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var max = await context.Set<Data.Models.Protocollo>()
                .Select(p => (short?)p.IdProtocollo)
                .MaxAsync();
            return (short)((max ?? 0) + 1);
        }
    }
}
