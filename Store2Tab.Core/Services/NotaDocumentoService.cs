using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Interfaces;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services
{
    public class NotaDocumentoService : INotaDocumentoService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public NotaDocumentoService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<NotaDocumento>> GetAllAsync(string? filtroCodice = null, string? filtroNota = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Set<NotaDocumento>().AsQueryable();
            if (!string.IsNullOrWhiteSpace(filtroCodice))
            {
                if (short.TryParse(filtroCodice, out var code))
                {
                    query = query.Where(n => n.IdNotaDocumento == code);
                }
            }
            if (!string.IsNullOrWhiteSpace(filtroNota))
            {
                query = query.Where(n => n.Nota.Contains(filtroNota));
            }
            return await query
                .OrderBy(n => n.Nota)
                .ToListAsync();
        }

        public async Task<NotaDocumento?> GetByIdAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<NotaDocumento>().FirstOrDefaultAsync(n => n.IdNotaDocumento == id);
        }

        public async Task<NotaDocumento> SaveAsync(NotaDocumento notaDocumento)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                // Normalizzazione dati
                notaDocumento.Nota = notaDocumento.Nota?.Trim() ?? string.Empty;

                // Validazione
                if (string.IsNullOrWhiteSpace(notaDocumento.Nota))
                    throw new ArgumentException("La nota documento è obbligatoria");

                var existing = notaDocumento.IdNotaDocumento > 0
                    ? await context.Set<NotaDocumento>().FirstOrDefaultAsync(n => n.IdNotaDocumento == notaDocumento.IdNotaDocumento)
                    : null;

                if (existing == null)
                {
                    // Insert: creo un nuovo oggetto
                    var nuovaNota = new NotaDocumento
                    {
                        IdNotaDocumento = 0,
                        Nota = notaDocumento.Nota
                    };

                    await context.Set<NotaDocumento>().AddAsync(nuovaNota);
                    await context.SaveChangesAsync();
                    return nuovaNota;
                }
                else
                {
                    // Update: aggiorno l'oggetto esistente
                    existing.Nota = notaDocumento.Nota;
                    context.Set<NotaDocumento>().Update(existing);
                    await context.SaveChangesAsync();
                    return existing;
                }
            }
            catch (DbUpdateException dbEx)
            {
                // Gestione errori di chiave primaria duplicata
                if (dbEx.InnerException != null && dbEx.InnerException.Message.Contains("duplicate key"))
                {
                    throw new InvalidOperationException("Esiste già una nota documento con lo stesso ID.");
                }
                throw;
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
            var existing = await context.Set<NotaDocumento>().FirstOrDefaultAsync(n => n.IdNotaDocumento == id);
            
            if (existing == null)
                return false;
            
            context.Set<NotaDocumento>().Remove(existing);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<NotaDocumento>().AnyAsync(n => n.IdNotaDocumento == id);
        }

        public async Task<short> GetNextIdAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var max = await context.Set<NotaDocumento>()
                .Select(n => (short?)n.IdNotaDocumento)
                .MaxAsync();
            return (short)((max ?? 0) + 1);
        }
    }
}
