using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services
{
    public class PassPianteCeeTipoService : IPassPianteCeeTipoService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public PassPianteCeeTipoService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<PassPianteCeeTipo>> GetAllAsync(string? filtroCodice = null, string? filtroDescrizione = null)
        {
            using var context = _contextFactory.CreateDbContext();

            var query = context.PassPianteCeeTipo
                .Include(p => p.Numerazione)
                .AsQueryable();

            // Applica filtri se specificati
            if (!string.IsNullOrWhiteSpace(filtroCodice) && short.TryParse(filtroCodice, out short codiceId))
            {
                query = query.Where(p => p.IdPassPianteCEE_Tipo == codiceId);
            }

            if (!string.IsNullOrWhiteSpace(filtroDescrizione))
            {
                query = query.Where(p => p.Descrizione.Contains(filtroDescrizione));
            }

            return await query
                .OrderBy(p => p.Descrizione)
                .ToListAsync();
        }

        public async Task<PassPianteCeeTipo?> GetByIdAsync(short id)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.PassPianteCeeTipo
                .Include(p => p.Numerazione)
                .FirstOrDefaultAsync(p => p.IdPassPianteCEE_Tipo == id);
        }

        public async Task<PassPianteCeeTipo> SaveAsync(PassPianteCeeTipo tipo)
        {
            using var context = _contextFactory.CreateDbContext();

            bool isNew = tipo.IdPassPianteCEE_Tipo == 0;

            if (isNew)
            {
                context.PassPianteCeeTipo.Add(tipo);
            }
            else
            {
                var existing = await context.PassPianteCeeTipo
                    .FirstOrDefaultAsync(p => p.IdPassPianteCEE_Tipo == tipo.IdPassPianteCEE_Tipo);

                if (existing == null)
                {
                    throw new InvalidOperationException($"PassPianteCeeTipo con ID {tipo.IdPassPianteCEE_Tipo} non trovato.");
                }

                // Aggiorna tutte le proprietà
                existing.IdPassPianteCEE_Numerazione = tipo.IdPassPianteCEE_Numerazione;
                existing.Descrizione = tipo.Descrizione;
                existing.ServizioFitosanitario = tipo.ServizioFitosanitario;
                existing.CodiceProduttore = tipo.CodiceProduttore;
                existing.CodiceProduttoreOrig = tipo.CodiceProduttoreOrig;
                existing.PaeseOrigine = tipo.PaeseOrigine;
                existing.StampaTesserino = tipo.StampaTesserino;
                existing.PassaportoCEE = tipo.PassaportoCEE;
                existing.DocumentoCommerc = tipo.DocumentoCommerc;
                existing.CatCertCAC = tipo.CatCertCAC;
                existing.Dal = tipo.Dal;
                existing.Al = tipo.Al;
                existing.DescrizioneStamp = tipo.DescrizioneStamp;
                existing.Raggruppamento = tipo.Raggruppamento;

                context.Entry(existing).State = EntityState.Modified;
            }

            await context.SaveChangesAsync();

            // Ricarica l'entità con le navigazione properties
            if (isNew)
            {
                await context.Entry(tipo)
                    .Reference(p => p.Numerazione)
                    .LoadAsync();
            }
            else
            {
                var saved = await GetByIdAsync(tipo.IdPassPianteCEE_Tipo);
                return saved!;
            }

            return tipo;
        }

        public async Task<bool> DeleteAsync(short id)
        {
            using var context = _contextFactory.CreateDbContext();

            var tipo = await context.PassPianteCeeTipo
                .FirstOrDefaultAsync(p => p.IdPassPianteCEE_Tipo == id);

            if (tipo == null)
                return false;

            context.PassPianteCeeTipo.Remove(tipo);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(short id)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.PassPianteCeeTipo
                .AnyAsync(p => p.IdPassPianteCEE_Tipo == id);
        }

        public async Task<short> GetNextIdAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            var maxId = await context.PassPianteCeeTipo
                .MaxAsync(p => (short?)p.IdPassPianteCEE_Tipo) ?? 0;

            return (short)(maxId + 1);
        }

        public async Task<List<PassPianteCeeNumerazione>> GetNumerazioniAsync()
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.PassPianteCeeNumerazioni
                .OrderBy(n => n.Descrizione)
                .ToListAsync();
        }
    }
}