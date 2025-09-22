using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services
{
    public class PassPianteCeeSpecieService : IPassPianteCeeSpecieService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public PassPianteCeeSpecieService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<PassPianteCeeSpecie>> GetAllAsync(string? filtroCodice = null, string? filtroSpecie = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.PassPianteCeeSpecie.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroCodice))
            {
                if (short.TryParse(filtroCodice, out var code))
                {
                    query = query.Where(s => s.IdPassPianteCEE_Specie == code);
                }
            }

            if (!string.IsNullOrWhiteSpace(filtroSpecie))
            {
                query = query.Where(s => s.Specie.Contains(filtroSpecie));
            }

            return await query
                .OrderBy(s => s.Specie)
                .ToListAsync();
        }

        public async Task<PassPianteCeeSpecie?> GetByIdAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PassPianteCeeSpecie.FirstOrDefaultAsync(s => s.IdPassPianteCEE_Specie == id);
        }

        public async Task<PassPianteCeeSpecie> SaveAsync(PassPianteCeeSpecie specie)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                // Normalizzazione dati
                specie.Specie = specie.Specie?.Trim() ?? string.Empty;

                // Validazione
                if (string.IsNullOrWhiteSpace(specie.Specie))
                    throw new ArgumentException("La specie botanica è obbligatoria");

                var existing = specie.IdPassPianteCEE_Specie > 0
                    ? await context.PassPianteCeeSpecie.FirstOrDefaultAsync(s => s.IdPassPianteCEE_Specie == specie.IdPassPianteCEE_Specie)
                    : null;

                if (existing == null)
                {
                    // Insert: crea un nuovo oggetto
                    var nuovaSpecie = new PassPianteCeeSpecie
                    {
                        IdPassPianteCEE_Specie = 0, // IDENTITY
                        Specie = specie.Specie
                    };

                    await context.PassPianteCeeSpecie.AddAsync(nuovaSpecie);
                    await context.SaveChangesAsync();
                    return nuovaSpecie;
                }
                else
                {
                    // Update
                    existing.Specie = specie.Specie;

                    context.PassPianteCeeSpecie.Update(existing);
                    await context.SaveChangesAsync();
                    return existing;
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
            var entity = await context.PassPianteCeeSpecie.FirstOrDefaultAsync(s => s.IdPassPianteCEE_Specie == id);
            if (entity == null) return false;

            context.PassPianteCeeSpecie.Remove(entity);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PassPianteCeeSpecie.AnyAsync(s => s.IdPassPianteCEE_Specie == id);
        }

        public async Task<short> GetNextIdAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var max = await context.PassPianteCeeSpecie
                .Select(s => (short?)s.IdPassPianteCEE_Specie)
                .MaxAsync();
            return (short)((max ?? 0) + 1);
        }
    }
}