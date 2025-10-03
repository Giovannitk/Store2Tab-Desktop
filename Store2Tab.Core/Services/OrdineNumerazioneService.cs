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
    public class OrdineNumerazioneService : IOrdineNumerazioneService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public OrdineNumerazioneService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<NumerazioneOrdini>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<NumerazioneOrdini>()
                .Where(n => n.IdOrdineNumerazione > 0)
                .OrderBy(n => n.OrdineNumerazione)
                .ToListAsync();
        }

        public async Task<List<NumerazioneOrdini>> SearchAsync(string codice, string descrizione)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Set<NumerazioneOrdini>()
                .Where(n => n.IdOrdineNumerazione > 0);

            if (!string.IsNullOrWhiteSpace(codice) && short.TryParse(codice, out short id))
            {
                query = query.Where(n => n.IdOrdineNumerazione == id);
            }

            if (!string.IsNullOrWhiteSpace(descrizione))
            {
                query = query.Where(n => n.OrdineNumerazione.Contains(descrizione));
            }

            return await query.OrderBy(n => n.OrdineNumerazione).ToListAsync();
        }

        public async Task<NumerazioneOrdini?> GetByIdAsync(short id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<NumerazioneOrdini>()
                .FirstOrDefaultAsync(n => n.IdOrdineNumerazione == id);
        }

        public async Task<NumerazioneOrdini> CreateAsync(NumerazioneOrdini numerazione)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // Inizializza i campi di default
            numerazione.StampaLogo = string.Empty;
            numerazione.StampaCodiciArticolo = 0;

            context.Set<NumerazioneOrdini>().Add(numerazione);
            await context.SaveChangesAsync();

            return numerazione;
        }

        public async Task<NumerazioneOrdini> UpdateAsync(NumerazioneOrdini numerazione)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var existing = await context.Set<NumerazioneOrdini>()
                .FirstOrDefaultAsync(n => n.IdOrdineNumerazione == numerazione.IdOrdineNumerazione);

            if (existing == null)
            {
                throw new InvalidOperationException($"Numerazione con ID {numerazione.IdOrdineNumerazione} non trovata.");
            }

            existing.OrdineNumerazione = numerazione.OrdineNumerazione;
            existing.NumerazioneSigla = numerazione.NumerazioneSigla;
            existing.DefaultCliente = numerazione.DefaultCliente;
            existing.DefaultFornitore = numerazione.DefaultFornitore;
            existing.StampaDestinatarioTel = numerazione.StampaDestinatarioTel;
            existing.StampaLogo = string.Empty;
            existing.StampaCodiciArticolo = 0;

            await context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteAsync(short id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var numerazione = await context.Set<NumerazioneOrdini>()
                .FirstOrDefaultAsync(n => n.IdOrdineNumerazione == id);

            if (numerazione == null)
                return false;

            context.Set<NumerazioneOrdini>().Remove(numerazione);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteMultipleAsync(List<short> ids)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var numerazioni = await context.Set<NumerazioneOrdini>()
                .Where(n => ids.Contains(n.IdOrdineNumerazione))
                .ToListAsync();

            if (!numerazioni.Any())
                return false;

            context.Set<NumerazioneOrdini>().RemoveRange(numerazioni);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(short id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<NumerazioneOrdini>()
                .AnyAsync(n => n.IdOrdineNumerazione == id);
        }
    }
}