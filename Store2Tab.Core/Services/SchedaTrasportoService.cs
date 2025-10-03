using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;
using Store2Tab.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store2Tab.Core.Services
{
    public class SchedaTrasportoService : ISchedaTrasportoService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public SchedaTrasportoService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<SchedaTrasporto?> CaricaSchedaTrasportoAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            // Carica la prima scheda disponibile (come nel VB6 che fa SELECT senza WHERE)
            return await context.Set<SchedaTrasporto>()
                .FirstOrDefaultAsync();
        }

        public async Task<bool> SalvaSchedaTrasportoAsync(SchedaTrasporto scheda)
        {
            if (scheda == null)
                throw new ArgumentNullException(nameof(scheda));

            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                // Normalizza i dati (trim delle stringhe)
                scheda.VettoreDescrizione = scheda.VettoreDescrizione?.Trim() ?? string.Empty;
                scheda.VettorePartitaIva = scheda.VettorePartitaIva?.Trim() ?? string.Empty;
                scheda.VettoreAlboAutotrasportatori = scheda.VettoreAlboAutotrasportatori?.Trim() ?? string.Empty;
                scheda.CommittenteDescrizione = scheda.CommittenteDescrizione?.Trim() ?? string.Empty;
                scheda.CommittentePartitaIva = scheda.CommittentePartitaIva?.Trim() ?? string.Empty;
                scheda.CaricatoreDescrizione = scheda.CaricatoreDescrizione?.Trim() ?? string.Empty;
                scheda.CaricatorePartitaIva = scheda.CaricatorePartitaIva?.Trim() ?? string.Empty;
                scheda.ProprietarioDescrizione = scheda.ProprietarioDescrizione?.Trim() ?? string.Empty;
                scheda.ProprietarioPartitaIva = scheda.ProprietarioPartitaIva?.Trim() ?? string.Empty;
                scheda.Dichiarazioni = scheda.Dichiarazioni?.Trim() ?? string.Empty;
                scheda.MerceTipologia = scheda.MerceTipologia?.Trim() ?? string.Empty;
                scheda.MerceQuantitaPeso = scheda.MerceQuantitaPeso?.Trim() ?? string.Empty;
                scheda.MerceLuogoCarico = scheda.MerceLuogoCarico?.Trim() ?? string.Empty;
                scheda.MerceLuogoScarico = scheda.MerceLuogoScarico?.Trim() ?? string.Empty;
                scheda.Luogo = scheda.Luogo?.Trim() ?? string.Empty;
                scheda.Compilatore = scheda.Compilatore?.Trim() ?? string.Empty;

                if (scheda.IdSchedaTrasporto == 0)
                {
                    // Calcola nuovo ID
                    var maxId = await context.Set<SchedaTrasporto>()
                                             .MaxAsync(s => (byte?)s.IdSchedaTrasporto) ?? 0;

                    if (maxId >= 255)
                        throw new InvalidOperationException("Limite massimo raggiunto per IdSchedaTrasporto (255).");

                    scheda.IdSchedaTrasporto = (byte)(maxId + 1);

                    context.Set<SchedaTrasporto>().Add(scheda);
                }
                else
                {
                    // Aggiornamento esistente
                    var existing = await context.Set<SchedaTrasporto>()
                        .FirstOrDefaultAsync(s => s.IdSchedaTrasporto == scheda.IdSchedaTrasporto);

                    if (existing != null)
                    {
                        // Aggiorna tutti i campi
                        existing.VettoreDescrizione = scheda.VettoreDescrizione;
                        existing.VettorePartitaIva = scheda.VettorePartitaIva;
                        existing.VettoreAlboAutotrasportatori = scheda.VettoreAlboAutotrasportatori;
                        existing.CommittenteDescrizione = scheda.CommittenteDescrizione;
                        existing.CommittentePartitaIva = scheda.CommittentePartitaIva;
                        existing.CaricatoreDescrizione = scheda.CaricatoreDescrizione;
                        existing.CaricatorePartitaIva = scheda.CaricatorePartitaIva;
                        existing.ProprietarioDescrizione = scheda.ProprietarioDescrizione;
                        existing.ProprietarioPartitaIva = scheda.ProprietarioPartitaIva;
                        existing.Dichiarazioni = scheda.Dichiarazioni;
                        existing.MerceTipologia = scheda.MerceTipologia;
                        existing.MerceQuantitaPeso = scheda.MerceQuantitaPeso;
                        existing.MerceLuogoCarico = scheda.MerceLuogoCarico;
                        existing.MerceLuogoScarico = scheda.MerceLuogoScarico;
                        existing.Luogo = scheda.Luogo;
                        existing.Compilatore = scheda.Compilatore;

                        context.Entry(existing).State = EntityState.Modified;
                    }
                    else
                    {
                        // Se non esiste, aggiungila come nuova (con ID già valorizzato)
                        context.Set<SchedaTrasporto>().Add(scheda);
                    }
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                // Log dell'errore se necessario
                System.Diagnostics.Debug.WriteLine($"Errore nel salvataggio: {ex.Message} - {ex.InnerException}");
                throw new InvalidOperationException("Impossibile salvare la scheda trasporto.", ex);
            }
        }


        public async Task<bool> CancellaSchedaTrasportoAsync(int idScheda)
        {
            if (idScheda <= 0)
                return false;

            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                var scheda = await context.Set<SchedaTrasporto>()
                    .FirstOrDefaultAsync(s => s.IdSchedaTrasporto == idScheda);

                if (scheda == null)
                    return false;

                context.Set<SchedaTrasporto>().Remove(scheda);
                await context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore nella cancellazione: {ex.Message}");
                throw new InvalidOperationException("Impossibile cancellare la scheda trasporto.", ex);
            }
        }

        public async Task<bool> EsisteSchedaTrasportoAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Set<SchedaTrasporto>().AnyAsync();
        }
    }
}