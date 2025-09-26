using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services
{
    public class TipiAttivitaService : ITipiAttivitaService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public TipiAttivitaService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<TipiAttivita>> GetAllAsync(string? filtroCodice = null, string? filtroAttivita = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Set<TipiAttivita>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroCodice))
            {
                if (short.TryParse(filtroCodice, out var code))
                {
                    query = query.Where(t => t.IdAnagraficaAttivita == code);
                }
            }

            if (!string.IsNullOrWhiteSpace(filtroAttivita))
            {
                query = query.Where(t => t.Attivita.Contains(filtroAttivita));
            }

            return await query
                .OrderBy(t => t.Attivita)
                .ToListAsync();
        }

        public async Task<TipiAttivita?> GetByIdAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<TipiAttivita>().FirstOrDefaultAsync(t => t.IdAnagraficaAttivita == id);
        }

        public async Task<TipiAttivita> SaveAsync(TipiAttivita tipoAttivita)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                // Normalizzazione dati
                tipoAttivita.Attivita = tipoAttivita.Attivita?.Trim() ?? string.Empty;

                // Validazione
                if (string.IsNullOrWhiteSpace(tipoAttivita.Attivita))
                    throw new ArgumentException("La descrizione dell'attività è obbligatoria");

                var existing = tipoAttivita.IdAnagraficaAttivita > 0
                    ? await context.Set<TipiAttivita>().FirstOrDefaultAsync(t => t.IdAnagraficaAttivita == tipoAttivita.IdAnagraficaAttivita)
                    : null;

                if (existing == null)
                {
                    // Insert: crea un nuovo oggetto
                    var nuovoTipo = new TipiAttivita
                    {
                        IdAnagraficaAttivita = 0, // IDENTITY
                        Attivita = tipoAttivita.Attivita
                    };

                    await context.Set<TipiAttivita>().AddAsync(nuovoTipo);
                    await context.SaveChangesAsync();
                    return nuovoTipo;
                }
                else
                {
                    // Update
                    existing.Attivita = tipoAttivita.Attivita;

                    context.Set<TipiAttivita>().Update(existing);
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
            var entity = await context.Set<TipiAttivita>().FirstOrDefaultAsync(t => t.IdAnagraficaAttivita == id);
            if (entity == null) return false;

            context.Set<TipiAttivita>().Remove(entity);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<TipiAttivita>().AnyAsync(t => t.IdAnagraficaAttivita == id);
        }

        public async Task<short> GetNextIdAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var max = await context.Set<TipiAttivita>()
                .Select(t => (short?)t.IdAnagraficaAttivita)
                .MaxAsync();
            return (short)((max ?? 0) + 1);
        }
    }
}