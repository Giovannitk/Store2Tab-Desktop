using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Interfaces;
using Store2Tab.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store2Tab.Core.Services
{
    public class ProtocolloContatoreService : IProtocolloContatoreService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ProtocolloContatoreService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ProtocolloContatoreDto>> GetAllAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Set<Data.Models.ProtocolloContatore>()
                .Include(pc => pc.Protocollo)
                .OrderByDescending(pc => pc.Esercizio)
                .ThenBy(pc => pc.Protocollo!.NomeProtocollo)
                .Select(pc => new ProtocolloContatoreDto
                {
                    IdProtocollo = pc.IdProtocollo,
                    Esercizio = pc.Esercizio,
                    NomeProtocollo = pc.Protocollo!.NomeProtocollo,
                    Contatore = pc.Contatore
                })
                .ToListAsync();
        }

        public async Task<Data.Models.ProtocolloContatore?> GetByIdAsync(short idProtocollo, short esercizio)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Set<Data.Models.ProtocolloContatore>()
                .Include(pc => pc.Protocollo)
                .FirstOrDefaultAsync(pc => pc.IdProtocollo == idProtocollo && pc.Esercizio == esercizio);
        }

        public async Task<Data.Models.ProtocolloContatore> UpdateContatoreAsync(short idProtocollo, short esercizio, int nuovoContatore)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                if (nuovoContatore < 0)
                    throw new ArgumentException("Il contatore non può essere negativo");

                var existing = await context.Set<Data.Models.ProtocolloContatore>()
                    .FirstOrDefaultAsync(pc => pc.IdProtocollo == idProtocollo && pc.Esercizio == esercizio);

                if (existing == null)
                    throw new InvalidOperationException($"Protocollo contatore non trovato (IdProtocollo: {idProtocollo}, Esercizio: {esercizio})");

                existing.Contatore = nuovoContatore;
                context.Set<Data.Models.ProtocolloContatore>().Update(existing);
                await context.SaveChangesAsync();

                // Ricarica con navigation property
                await context.Entry(existing).Reference(e => e.Protocollo).LoadAsync();

                return existing;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Errore durante l'aggiornamento del contatore.", ex);
            }
        }

        public async Task<bool> DeleteAsync(short idProtocollo, short esercizio)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var existing = await context.Set<Data.Models.ProtocolloContatore>()
                .FirstOrDefaultAsync(pc => pc.IdProtocollo == idProtocollo && pc.Esercizio == esercizio);

            if (existing == null)
                return false;

            context.Set<Data.Models.ProtocolloContatore>().Remove(existing);
            await context.SaveChangesAsync();
            return true;
        }
    }
}