using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services
{
    public class PassPianteCeeNumerazioneService : IPassPianteCeeNumerazioneService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public PassPianteCeeNumerazioneService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<PassPianteCeeNumerazione>> GetAllAsync(string? filtroCodice = null, string? filtroDescrizione = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.PassPianteCeeNumerazioni.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroCodice))
            {
                if (int.TryParse(filtroCodice, out var code))
                {
                    query = query.Where(n => n.IdPassPianteCEE_Numerazione == code);
                }
            }

            if (!string.IsNullOrWhiteSpace(filtroDescrizione))
            {
                query = query.Where(n => n.Descrizione.Contains(filtroDescrizione));
            }

            return await query
                .OrderBy(n => n.IdPassPianteCEE_Numerazione)
                .ToListAsync();
        }

        public async Task<PassPianteCeeNumerazione?> GetByIdAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PassPianteCeeNumerazioni.FirstOrDefaultAsync(n => n.IdPassPianteCEE_Numerazione == id);
        }

        public async Task<PassPianteCeeNumerazione> SaveAsync(PassPianteCeeNumerazione numerazione)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                // Normalizzazione dati
                numerazione.Descrizione = numerazione.Descrizione?.Trim() ?? string.Empty;
                numerazione.Sigla = numerazione.Sigla?.Trim() ?? string.Empty;
                numerazione.Prefisso = numerazione.Prefisso?.Trim() ?? string.Empty;

                // Validazione
                if (string.IsNullOrWhiteSpace(numerazione.Descrizione))
                    throw new ArgumentException("La descrizione è obbligatoria");

                var existing = numerazione.IdPassPianteCEE_Numerazione > 0
                    ? await context.PassPianteCeeNumerazioni.FirstOrDefaultAsync(n => n.IdPassPianteCEE_Numerazione == numerazione.IdPassPianteCEE_Numerazione)
                    : null;

                if (existing == null)
                {
                    // Insert: crea un nuovo oggetto
                    var nuovaNumerazione = new PassPianteCeeNumerazione
                    {
                        IdPassPianteCEE_Numerazione = 0, // IDENTITY
                        Descrizione = numerazione.Descrizione,
                        Sigla = numerazione.Sigla,
                        Prefisso = numerazione.Prefisso
                    };

                    await context.PassPianteCeeNumerazioni.AddAsync(nuovaNumerazione);
                    await context.SaveChangesAsync();
                    return nuovaNumerazione;
                }
                else
                {
                    // Update
                    existing.Descrizione = numerazione.Descrizione;
                    existing.Sigla = numerazione.Sigla;
                    existing.Prefisso = numerazione.Prefisso;

                    context.PassPianteCeeNumerazioni.Update(existing);
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

        public async Task<bool> DeleteAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var entity = await context.PassPianteCeeNumerazioni.FirstOrDefaultAsync(n => n.IdPassPianteCEE_Numerazione == id);
            if (entity == null) return false;

            context.PassPianteCeeNumerazioni.Remove(entity);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PassPianteCeeNumerazioni.AnyAsync(n => n.IdPassPianteCEE_Numerazione == id);
        }

        public async Task<int> GetNextIdAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var max = await context.PassPianteCeeNumerazioni
                .Select(n => (int?)n.IdPassPianteCEE_Numerazione)
                .MaxAsync();
            return (max ?? 0) + 1;
        }
    }
}