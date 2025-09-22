using Microsoft.EntityFrameworkCore;
using Store2Tab.Core.Interfaces;
using Store2Tab.Data;
using Store2Tab.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Core.Services
{
    public class PassPianteCeePortinnestoService : IPassPianteCeePortinnestoService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public PassPianteCeePortinnestoService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<PassPianteCEE_Portinnesto>> GetAllAsync(string? filtroCodice = null, string? filtroPortinnesto = null, short? filtroSpecieId = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.PassPianteCeePortinnesto
                .Include(p => p.SpecieBotanica)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(filtroCodice))
            {
                if (short.TryParse(filtroCodice, out var code))
                {
                    query = query.Where(p => p.IdPassPianteCEE_Portinnesto == code);
                }
            }
            if (!string.IsNullOrWhiteSpace(filtroPortinnesto))
            {
                query = query.Where(p => p.Portinnesto.Contains(filtroPortinnesto));
            }
            if (filtroSpecieId.HasValue && filtroSpecieId.Value > 0)
            {
                query = query.Where(p => p.IdPassPianteCEE_Specie == filtroSpecieId.Value);
            }
            return await query
                .OrderBy(p => p.SpecieBotanica!.Specie)
                .ThenBy(p => p.Portinnesto)
                .ToListAsync();
        }

        public async Task<PassPianteCEE_Portinnesto?> GetByIdAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PassPianteCeePortinnesto
                .Include(p => p.SpecieBotanica)
                .FirstOrDefaultAsync(p => p.IdPassPianteCEE_Portinnesto == id);
        }

        public async Task<PassPianteCEE_Portinnesto> SaveAsync(PassPianteCEE_Portinnesto portinnesto)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            try
            {
                // Normalizzazione dati
                portinnesto.Portinnesto = portinnesto.Portinnesto?.Trim() ?? string.Empty;
                
                // Validazione
                if (portinnesto.IdPassPianteCEE_Specie <= 0)
                {
                    throw new ArgumentException("La specie botanica è obbligatoria");
                }

                if (string.IsNullOrWhiteSpace(portinnesto.Portinnesto))
                {
                    throw new ArgumentException("Il portinnesto è obbligatorio");
                }

                // Controllo duplicati
                var specieExists = await context.PassPianteCeeSpecie
                    .AnyAsync(s => s.IdPassPianteCEE_Specie == portinnesto.IdPassPianteCEE_Specie);

                if (!specieExists)
                    throw new ArgumentException("La specie botanica specificata non esiste");

                var existing = portinnesto.IdPassPianteCEE_Portinnesto > 0
                    ? await context.PassPianteCeePortinnesto.FirstOrDefaultAsync(v => v.IdPassPianteCEE_Portinnesto == portinnesto.IdPassPianteCEE_Portinnesto)
                    : null;

                if (existing == null)
                {
                    // Insert: crea un nuovo oggetto
                    var nuovaPortinnesto = new PassPianteCEE_Portinnesto
                    {
                        IdPassPianteCEE_Portinnesto = 0, // IDENTITY
                        IdPassPianteCEE_Specie = portinnesto.IdPassPianteCEE_Specie,
                        Portinnesto = portinnesto.Portinnesto
                    };

                    await context.PassPianteCeePortinnesto.AddAsync(nuovaPortinnesto);
                    await context.SaveChangesAsync();

                    // Ricarica con la navigazione property
                    var savedPortinnesto = await context.PassPianteCeePortinnesto
                        .Include(v => v.SpecieBotanica)
                        .FirstAsync(v => v.IdPassPianteCEE_Portinnesto == nuovaPortinnesto.IdPassPianteCEE_Portinnesto);

                    return savedPortinnesto;
                }
                else
                {
                    // Update
                    existing.IdPassPianteCEE_Specie = portinnesto.IdPassPianteCEE_Specie;
                    existing.Portinnesto = portinnesto.Portinnesto;

                    context.PassPianteCeePortinnesto.Update(existing);
                    await context.SaveChangesAsync();

                    // Ricarica con la navigazione property
                    return await context.PassPianteCeePortinnesto
                        .Include(v => v.SpecieBotanica)
                        .FirstAsync(v => v.IdPassPianteCEE_Portinnesto == existing.IdPassPianteCEE_Portinnesto);
                }
            }
            catch (DbUpdateException ex)
            {
                // Log the exception (you can use any logging framework you prefer)
                Console.Error.WriteLine($"An error occurred while saving the Portinnesto: {ex.Message}");
                throw; // Re-throw the exception after logging it
            }
        }
    
        public async Task<bool> DeleteAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var existing = await context.PassPianteCeePortinnesto.FirstOrDefaultAsync(v => v.IdPassPianteCEE_Portinnesto == id);
            if (existing != null)
            {
                context.PassPianteCeePortinnesto.Remove(existing);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ExistsAsync(short id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PassPianteCeePortinnesto.AnyAsync(v => v.IdPassPianteCEE_Portinnesto == id);
        }

        public async Task<short> GetNextIdAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            var max = await context.PassPianteCeePortinnesto
                .Select(v => (short?)v.IdPassPianteCEE_Portinnesto)
                .MaxAsync() ?? 0;
            return (short)(max + 1);
        }

        public async Task<List<PassPianteCEE_Portinnesto>> GetBySpecieAsync(short specieId)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();
            return await context.PassPianteCeePortinnesto
                .Include(p => p.SpecieBotanica)
                .Where(p => p.IdPassPianteCEE_Specie == specieId)
                .OrderBy(p => p.Portinnesto)
                .ToListAsync();
        }
    }
}
