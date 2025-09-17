using Store2Tab.Data.Models;
using Store2Tab.Data;
using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Core.Services
{
    public class CausaleMovimentoService : ICausaleMovimentoService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public CausaleMovimentoService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<CausaleMovimento>> GetCausaliMovimentoAsync(string? filtroCodice = null, string? filtroDescrizione = null)
        {
            try
            {
                await using var _context = await _contextFactory.CreateDbContextAsync();
                var query = _context.CausaliMovimento
                    .Include(c => c.ControMovimento)
                    .Where(c => c.Utilizzabile == 1);

                // Applica filtri se forniti
                if (!string.IsNullOrWhiteSpace(filtroCodice))
                {
                    query = query.Where(c => c.Codice == filtroCodice);
                }

                if (!string.IsNullOrWhiteSpace(filtroDescrizione))
                {
                    query = query.Where(c => c.Descrizione.Contains(filtroDescrizione));
                }

                return await query
                    .OrderBy(c => c.Codice)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log dell'errore
                System.Diagnostics.Debug.WriteLine($"Errore in GetCausaliMovimentoAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<CausaleMovimento?> GetCausaleMovimentoByCodeAsync(string codice)
        {
            try
            {
                await using var _context = await _contextFactory.CreateDbContextAsync();
                return await _context.CausaliMovimento
                    .Include(c => c.ControMovimento)
                    .FirstOrDefaultAsync(c => c.Codice == codice && c.Utilizzabile == 1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore in GetCausaleMovimentoByCodeAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> SalvaCausaleMovimentoAsync(CausaleMovimento causale)
        {
            try
            {
                await using var _context = await _contextFactory.CreateDbContextAsync();
                // Validazioni
                if (string.IsNullOrWhiteSpace(causale.Codice))
                    throw new ArgumentException("Il codice è obbligatorio");

                if (string.IsNullOrWhiteSpace(causale.Descrizione))
                    throw new ArgumentException("La descrizione è obbligatoria");

                // Verifica se il codice contro movimento esiste (se specificato)
                if (!string.IsNullOrWhiteSpace(causale.CodiceControMovimento))
                {
                    var controMovimentoExists = await _context.CausaliMovimento
                        .AnyAsync(c => c.Codice == causale.CodiceControMovimento && c.Utilizzabile == 1);

                    if (!controMovimentoExists)
                        throw new ArgumentException("Il codice contro movimento specificato non esiste");
                }

                var existing = await _context.CausaliMovimento
                    .FirstOrDefaultAsync(c => c.Codice == causale.Codice);

                if (existing != null)
                {
                    // Aggiorna record esistente
                    existing.Descrizione = causale.Descrizione;
                    existing.CodiceControMovimento = string.IsNullOrWhiteSpace(causale.CodiceControMovimento) ? string.Empty : causale.CodiceControMovimento;
                    existing.Utilizzabile = 1;
                    _context.Update(existing);
                }
                else
                {
                    // Nuovo record
                    causale.Utilizzabile = 1;
                    causale.CodiceControMovimento = string.IsNullOrWhiteSpace(causale.CodiceControMovimento) ? string.Empty : causale.CodiceControMovimento;
                    await _context.CausaliMovimento.AddAsync(causale);
                }

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore in SalvaCausaleMovimentoAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CancellaCausaleMovimentoAsync(string codice)
        {
            try
            {
                // Verifica se la causale è utilizzata come contro movimento
                await using var _context = await _contextFactory.CreateDbContextAsync();
                var usedAsControMovimento = await _context.CausaliMovimento
                    .AnyAsync(c => c.CodiceControMovimento == codice && c.Utilizzabile == 1);

                if (usedAsControMovimento)
                {
                    throw new InvalidOperationException("Impossibile cancellare: la causale è utilizzata come contro movimento in altre causali");
                }

                var causale = await _context.CausaliMovimento
                    .FirstOrDefaultAsync(c => c.Codice == codice);

                if (causale != null)
                {
                    _context.CausaliMovimento.Remove(causale);
                    var result = await _context.SaveChangesAsync();
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore in CancellaCausaleMovimentoAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> EsisteCodiceAsync(string codice, string? codiceOriginale = null)
        {
            try
            {
                await using var _context = await _contextFactory.CreateDbContextAsync();
                var query = _context.CausaliMovimento.Where(c => c.Codice == codice && c.Utilizzabile == 1);

                // Se stiamo modificando un record esistente, escludiamo il codice originale
                if (!string.IsNullOrWhiteSpace(codiceOriginale))
                {
                    query = query.Where(c => c.Codice != codiceOriginale);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore in EsisteCodiceAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<CausaleMovimento>> CercaCausaliPerSelezioneAsync(string? filtroTesto = null)
        {
            try
            {
                await using var _context = await _contextFactory.CreateDbContextAsync();
                var query = _context.CausaliMovimento.Where(c => c.Utilizzabile == 1);

                if (!string.IsNullOrWhiteSpace(filtroTesto))
                {
                    query = query.Where(c =>
                        c.Codice.Contains(filtroTesto) ||
                        c.Descrizione.Contains(filtroTesto));
                }

                return await query
                    .OrderBy(c => c.Codice)
                    .Take(100) // Limita i risultati per performance
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore in CercaCausaliPerSelezioneAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetNextCodiceAsync()
        {
            try
            {
                await using var _context = await _contextFactory.CreateDbContextAsync();
                // Prende l'ultimo codice in ordine decrescente (stringa di 4 caratteri)
                var last = await _context.CausaliMovimento
                    .Where(c => c.Utilizzabile == 1)
                    .OrderByDescending(c => c.Codice)
                    .Select(c => c.Codice)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(last))
                {
                    return "0001";
                }

                // Prova a interpretare come numero e incrementa, mantenendo padding a 4
                if (int.TryParse(last, out var num))
                {
                    num += 1;
                    if (num > 9999) throw new InvalidOperationException("Superato il range massimo di codici a 4 cifre.");
                    return num.ToString("D4");
                }

                // Se non numerico, fallback semplice non distruttivo
                return last;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errore in GetNextCodiceAsync: {ex.Message}");
                throw;
            }
        }
    }
}
