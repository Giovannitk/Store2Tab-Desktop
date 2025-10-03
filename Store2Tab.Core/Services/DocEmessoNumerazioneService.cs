using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store2Tab.Core.Services
{
    public class DocEmessoNumerazioneService : IDocEmessoNumerazioneService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public DocEmessoNumerazioneService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<DocEmessoNumerazione>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<DocEmessoNumerazione>()
                .Where(n => n.IdDocEmessoNumerazione > 0)
                .OrderBy(n => n.DocEmessoNumerazioneDescrizione)
                .ToListAsync();
        }

        public async Task<List<DocEmessoNumerazione>> SearchAsync(string codice, string descrizione)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Set<DocEmessoNumerazione>()
                .Where(n => n.IdDocEmessoNumerazione > 0);

            if (!string.IsNullOrWhiteSpace(codice) && short.TryParse(codice, out short id))
            {
                query = query.Where(n => n.IdDocEmessoNumerazione == id);
            }

            if (!string.IsNullOrWhiteSpace(descrizione))
            {
                query = query.Where(n => n.DocEmessoNumerazioneDescrizione.Contains(descrizione));
            }

            return await query.OrderBy(n => n.DocEmessoNumerazioneDescrizione).ToListAsync();
        }

        public async Task<DocEmessoNumerazione?> GetByIdAsync(short id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<DocEmessoNumerazione>()
                .FirstOrDefaultAsync(n => n.IdDocEmessoNumerazione == id);
        }

        public async Task<DocEmessoNumerazione> CreateAsync(DocEmessoNumerazione entity)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Set<DocEmessoNumerazione>().Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<DocEmessoNumerazione> UpdateAsync(DocEmessoNumerazione entity)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var existing = await context.Set<DocEmessoNumerazione>()
                .FirstOrDefaultAsync(n => n.IdDocEmessoNumerazione == entity.IdDocEmessoNumerazione);

            if (existing == null)
            {
                throw new InvalidOperationException($"Numerazione documenti emessi con ID {entity.IdDocEmessoNumerazione} non trovata.");
            }

            existing.DocEmessoNumerazioneDescrizione = entity.DocEmessoNumerazioneDescrizione;
            existing.NumerazioneSigla = entity.NumerazioneSigla;
            existing.DocumentoElettronico = entity.DocumentoElettronico;
            existing.FE_TipoDoc = entity.FE_TipoDoc;
            existing.DocEmessoTipo_Stampa = entity.DocEmessoTipo_Stampa;

            await context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteAsync(short id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var entity = await context.Set<DocEmessoNumerazione>()
                .FirstOrDefaultAsync(n => n.IdDocEmessoNumerazione == id);

            if (entity == null)
                return false;

            context.Set<DocEmessoNumerazione>().Remove(entity);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteMultipleAsync(List<short> ids)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var entities = await context.Set<DocEmessoNumerazione>()
                .Where(n => ids.Contains(n.IdDocEmessoNumerazione))
                .ToListAsync();

            if (!entities.Any())
                return false;

            context.Set<DocEmessoNumerazione>().RemoveRange(entities);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(short id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<DocEmessoNumerazione>()
                .AnyAsync(n => n.IdDocEmessoNumerazione == id);
        }
    }
}

