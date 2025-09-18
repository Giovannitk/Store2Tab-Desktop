using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services
{
    public class PagamentoMezzoService : IPagamentoMezzoService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public PagamentoMezzoService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<PagamentoMezzo>> GetAllAsync(string? filtroCodice = null, string? filtroDescrizione = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.PagamentiMezzo.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroCodice))
            {
                if (short.TryParse(filtroCodice, out var code))
                {
                    query = query.Where(m => m.IdPagamentoMezzo == code);
                }
            }

            if (!string.IsNullOrWhiteSpace(filtroDescrizione))
            {
                query = query.Where(m => m.pagamentoMezzo.Contains(filtroDescrizione));
            }

            return await query
                .OrderBy(m => m.pagamentoMezzo)
                .ToListAsync();
        }

        public async Task<PagamentoMezzo?> GetByIdAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PagamentiMezzo.FirstOrDefaultAsync(m => m.IdPagamentoMezzo == id);
        }

        public async Task<PagamentoMezzo> SaveAsync(PagamentoMezzo mezzo)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                // DEBUG: Stampo i valori PRIMA della normalizzazione
                //System.Diagnostics.Debug.WriteLine($"PRIMA normalizzazione:");
                //System.Diagnostics.Debug.WriteLine($"IdPagamentoMezzo: {mezzo.IdPagamentoMezzo}");
                //System.Diagnostics.Debug.WriteLine($"pagamentoMezzo: '{mezzo.pagamentoMezzo}' (is null: {mezzo.pagamentoMezzo == null})");
                //System.Diagnostics.Debug.WriteLine($"Gruppo: '{mezzo.Gruppo}' (is null: {mezzo.Gruppo == null})");
                //System.Diagnostics.Debug.WriteLine($"PagamentoInterbancario: {mezzo.PagamentoInterbancario} (type: {mezzo.PagamentoInterbancario.GetType()})");
                //System.Diagnostics.Debug.WriteLine($"IdBanca_Emesso: {mezzo.IdBanca_Emesso}");
                //System.Diagnostics.Debug.WriteLine($"IdBanca_Ricevuto: {mezzo.IdBanca_Ricevuto}");
                //System.Diagnostics.Debug.WriteLine($"FE_ModPag: '{mezzo.FE_ModPag}' (is null: {mezzo.FE_ModPag == null})");

                // Normalizzazione FORZATA - mi assicuro che non ci siano mai valori null
                mezzo.pagamentoMezzo = mezzo.pagamentoMezzo?.Trim() ?? string.Empty;
                mezzo.Gruppo = mezzo.Gruppo?.Trim() ?? string.Empty;
                mezzo.FE_ModPag = mezzo.FE_ModPag?.Trim() ?? string.Empty;

                // FORZA i valori numerici - mi assicuro che siano sempre validi
                // Se PagamentoInterbancario è null o ha problemi, forzo a 0
                mezzo.PagamentoInterbancario = mezzo.PagamentoInterbancario; // dovrebbe essere già byte
                mezzo.IdBanca_Emesso = Math.Max((short)0, mezzo.IdBanca_Emesso);
                mezzo.IdBanca_Ricevuto = Math.Max((short)0, mezzo.IdBanca_Ricevuto);

                // DEBUG: Stampo i valori DOPO la normalizzazione
                //System.Diagnostics.Debug.WriteLine($"DOPO normalizzazione:");
                //System.Diagnostics.Debug.WriteLine($"IdPagamentoMezzo: {mezzo.IdPagamentoMezzo}");
                //System.Diagnostics.Debug.WriteLine($"pagamentoMezzo: '{mezzo.pagamentoMezzo}'");
                //System.Diagnostics.Debug.WriteLine($"Gruppo: '{mezzo.Gruppo}'");
                //System.Diagnostics.Debug.WriteLine($"PagamentoInterbancario: {mezzo.PagamentoInterbancario}");
                //System.Diagnostics.Debug.WriteLine($"IdBanca_Emesso: {mezzo.IdBanca_Emesso}");
                //System.Diagnostics.Debug.WriteLine($"IdBanca_Ricevuto: {mezzo.IdBanca_Ricevuto}");
                //System.Diagnostics.Debug.WriteLine($"FE_ModPag: '{mezzo.FE_ModPag}'");

                // Validazione
                if (string.IsNullOrWhiteSpace(mezzo.pagamentoMezzo))
                    throw new ArgumentException("La descrizione del mezzo di pagamento è obbligatoria");

                var existing = mezzo.IdPagamentoMezzo > 0
                    ? await context.PagamentiMezzo.FirstOrDefaultAsync(m => m.IdPagamentoMezzo == mezzo.IdPagamentoMezzo)
                    : null;

                if (existing == null)
                {
                    // Insert: viene creato un nuovo oggetto completamente pulito
                    var nuovoMezzo = new PagamentoMezzo
                    {
                        IdPagamentoMezzo = 0, // IDENTITY
                        pagamentoMezzo = mezzo.pagamentoMezzo,
                        Gruppo = mezzo.Gruppo,
                        PagamentoInterbancario = mezzo.PagamentoInterbancario,
                        IdBanca_Emesso = mezzo.IdBanca_Emesso,
                        IdBanca_Ricevuto = mezzo.IdBanca_Ricevuto,
                        FE_ModPag = mezzo.FE_ModPag
                    };

                    // DEBUG: Stampo l'oggetto che sto per inserire
                    //System.Diagnostics.Debug.WriteLine($"OGGETTO PER INSERT:");
                    //System.Diagnostics.Debug.WriteLine($"pagamentoMezzo: '{nuovoMezzo.pagamentoMezzo}'");
                    //System.Diagnostics.Debug.WriteLine($"Gruppo: '{nuovoMezzo.Gruppo}'");
                    //System.Diagnostics.Debug.WriteLine($"PagamentoInterbancario: {nuovoMezzo.PagamentoInterbancario}");
                    //System.Diagnostics.Debug.WriteLine($"IdBanca_Emesso: {nuovoMezzo.IdBanca_Emesso}");
                    //System.Diagnostics.Debug.WriteLine($"IdBanca_Ricevuto: {nuovoMezzo.IdBanca_Ricevuto}");
                    //System.Diagnostics.Debug.WriteLine($"FE_ModPag: '{nuovoMezzo.FE_ModPag}'");

                    await context.PagamentiMezzo.AddAsync(nuovoMezzo);
                    await context.SaveChangesAsync();
                    return nuovoMezzo;
                }
                else
                {
                    // Update
                    existing.pagamentoMezzo = mezzo.pagamentoMezzo;
                    existing.Gruppo = mezzo.Gruppo;
                    existing.PagamentoInterbancario = mezzo.PagamentoInterbancario;
                    existing.IdBanca_Emesso = mezzo.IdBanca_Emesso;
                    existing.IdBanca_Ricevuto = mezzo.IdBanca_Ricevuto;
                    existing.FE_ModPag = mezzo.FE_ModPag;

                    context.PagamentiMezzo.Update(existing);
                    await context.SaveChangesAsync();
                    return existing;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRORE in SaveAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var entity = await context.PagamentiMezzo.FirstOrDefaultAsync(m => m.IdPagamentoMezzo == id);
            if (entity == null) return false;

            context.PagamentiMezzo.Remove(entity);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PagamentiMezzo.AnyAsync(m => m.IdPagamentoMezzo == id);
        }

        public async Task<short> GetNextIdAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var max = await context.PagamentiMezzo
                .Select(m => (short?)m.IdPagamentoMezzo)
                .MaxAsync();
            return (short)((max ?? 0) + 1);
        }
    }
}