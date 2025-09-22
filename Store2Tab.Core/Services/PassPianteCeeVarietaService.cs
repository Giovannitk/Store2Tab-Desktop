using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services
{
    public class PassPianteCeeVarietaService : IPassPianteCeeVarietaService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public PassPianteCeeVarietaService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<PassPianteCeeVarieta>> GetAllAsync(string? filtroCodice = null, string? filtroVarieta = null, short? filtroSpecieId = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.PassPianteCeeVarieta
                .Include(v => v.SpecieBotanica)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroCodice))
            {
                if (short.TryParse(filtroCodice, out var code))
                {
                    query = query.Where(v => v.IdPassPianteCEE_Varieta == code);
                }
            }

            if (!string.IsNullOrWhiteSpace(filtroVarieta))
            {
                query = query.Where(v => v.Varieta.Contains(filtroVarieta));
            }

            if (filtroSpecieId.HasValue && filtroSpecieId.Value > 0)
            {
                query = query.Where(v => v.IdPassPianteCEE_Specie == filtroSpecieId.Value);
            }

            return await query
                .OrderBy(v => v.SpecieBotanica!.Specie)
                .ThenBy(v => v.Varieta)
                .ToListAsync();
        }

        public async Task<PassPianteCeeVarieta?> GetByIdAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PassPianteCeeVarieta
                .Include(v => v.SpecieBotanica)
                .FirstOrDefaultAsync(v => v.IdPassPianteCEE_Varieta == id);
        }

        public async Task<PassPianteCeeVarieta> SaveAsync(PassPianteCeeVarieta varieta)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                // Normalizzazione dati
                varieta.Varieta = varieta.Varieta?.Trim() ?? string.Empty;

                // Validazione
                if (varieta.IdPassPianteCEE_Specie <= 0)
                    throw new ArgumentException("La specie botanica è obbligatoria");

                if (string.IsNullOrWhiteSpace(varieta.Varieta))
                    throw new ArgumentException("La varietà è obbligatoria");

                // Verifica che la specie botanica esista
                var specieExists = await context.PassPianteCeeSpecie
                    .AnyAsync(s => s.IdPassPianteCEE_Specie == varieta.IdPassPianteCEE_Specie);

                if (!specieExists)
                    throw new ArgumentException("La specie botanica selezionata non esiste");

                var existing = varieta.IdPassPianteCEE_Varieta > 0
                    ? await context.PassPianteCeeVarieta.FirstOrDefaultAsync(v => v.IdPassPianteCEE_Varieta == varieta.IdPassPianteCEE_Varieta)
                    : null;

                if (existing == null)
                {
                    // Insert: crea un nuovo oggetto
                    var nuovaVarieta = new PassPianteCeeVarieta
                    {
                        IdPassPianteCEE_Varieta = 0, // IDENTITY
                        IdPassPianteCEE_Specie = varieta.IdPassPianteCEE_Specie,
                        Varieta = varieta.Varieta
                    };

                    await context.PassPianteCeeVarieta.AddAsync(nuovaVarieta);
                    await context.SaveChangesAsync();

                    // Ricarica con la navigazione property
                    var savedVarieta = await context.PassPianteCeeVarieta
                        .Include(v => v.SpecieBotanica)
                        .FirstAsync(v => v.IdPassPianteCEE_Varieta == nuovaVarieta.IdPassPianteCEE_Varieta);

                    return savedVarieta;
                }
                else
                {
                    // Update
                    existing.IdPassPianteCEE_Specie = varieta.IdPassPianteCEE_Specie;
                    existing.Varieta = varieta.Varieta;

                    context.PassPianteCeeVarieta.Update(existing);
                    await context.SaveChangesAsync();

                    // Ricarica con la navigazione property
                    return await context.PassPianteCeeVarieta
                        .Include(v => v.SpecieBotanica)
                        .FirstAsync(v => v.IdPassPianteCEE_Varieta == existing.IdPassPianteCEE_Varieta);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRORE in SaveAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var entity = await context.PassPianteCeeVarieta.FirstOrDefaultAsync(v => v.IdPassPianteCEE_Varieta == id);
            if (entity == null) return false;

            context.PassPianteCeeVarieta.Remove(entity);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PassPianteCeeVarieta.AnyAsync(v => v.IdPassPianteCEE_Varieta == id);
        }

        public async Task<short> GetNextIdAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var max = await context.PassPianteCeeVarieta
                .Select(v => (short?)v.IdPassPianteCEE_Varieta)
                .MaxAsync();
            return (short)((max ?? 0) + 1);
        }

        public async Task<List<PassPianteCeeVarieta>> GetBySpecieAsync(short specieId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PassPianteCeeVarieta
                .Include(v => v.SpecieBotanica)
                .Where(v => v.IdPassPianteCEE_Specie == specieId)
                .OrderBy(v => v.Varieta)
                .ToListAsync();
        }
    }
}