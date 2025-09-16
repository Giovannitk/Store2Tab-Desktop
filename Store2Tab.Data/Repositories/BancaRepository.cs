using Microsoft.EntityFrameworkCore;
using Store2Tab.Data.Models;
using Store2Tab.Data.Repositories.Interfaces;

namespace Store2Tab.Data.Repositories
{
    public class BancaRepository : IBancaRepository
    {
        private readonly AppDbContext _context;

        public BancaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Banca>> GetAllAsync()
        {
            return await _context.Banche
                .OrderBy(b => b.Codice)
                .ToListAsync();
        }

        public async Task<Banca?> GetByIdAsync(short id)
        {
            return await _context.Banche
                .FirstOrDefaultAsync(b => b.ID == id);
        }

        public async Task<IEnumerable<Banca>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var term = searchTerm.ToLower();
            return await _context.Banche
                .Where(b => (b.Codice != null && b.Codice.ToLower().Contains(term)) ||
                           (b.Denominazione != null && b.Denominazione.ToLower().Contains(term)) ||
                           (b.Agenzia != null && b.Agenzia.ToLower().Contains(term)))
                .OrderBy(b => b.Codice)
                .ToListAsync();
        }

        public async Task<Banca> AddAsync(Banca banca)
        {
            try
            {
                // Per le colonne IDENTITY, NON impostare l'ID
                // Entity Framework gestirà automaticamente l'ID
                var entity = new Banca
                {
                    // NON impostare ID per inserimenti
                    Codice = banca.Codice,
                    Denominazione = banca.Denominazione,
                    Agenzia = banca.Agenzia,
                    ABI = banca.ABI,
                    CAB = banca.CAB,
                    CC = banca.CC,
                    CIN = banca.CIN,
                    IBAN = banca.IBAN,
                    SWIFT = banca.SWIFT,
                    NoteInterne = banca.NoteInterne,
                    Predefinita = banca.Predefinita
                };

                _context.Banche.Add(entity);
                if (banca.Predefinita == default)
                    banca.Predefinita = 0;
                await _context.SaveChangesAsync();

                // Ora l'entity avrà l'ID generato dal database
                return entity;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddAsync Error: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"AddAsync Inner: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<Banca> UpdateAsync(Banca banca)
        {
            try
            {
                var existingEntity = await _context.Banche.FindAsync(banca.ID);
                if (existingEntity == null)
                {
                    throw new ArgumentException($"Banca con ID {banca.ID} non trovata");
                }

                // Aggiorna solo i campi necessari
                existingEntity.Codice = banca.Codice;
                existingEntity.Denominazione = banca.Denominazione;
                existingEntity.Agenzia = banca.Agenzia;
                existingEntity.ABI = banca.ABI;
                existingEntity.CAB = banca.CAB;
                existingEntity.CC = banca.CC;
                existingEntity.CIN = banca.CIN;
                existingEntity.IBAN = banca.IBAN;
                existingEntity.SWIFT = banca.SWIFT;
                existingEntity.NoteInterne = banca.NoteInterne;
                existingEntity.Predefinita = banca.Predefinita;

                _context.Entry(existingEntity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return existingEntity;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateAsync Error: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(short id)
        {
            var banca = await GetByIdAsync(id);
            if (banca != null)
            {
                _context.Banche.Remove(banca);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(short id)
        {
            return await _context.Banche
                .AnyAsync(b => b.ID == id);
        }

        public async Task<Banca?> GetPredefinitaAsync()
        {
            return await _context.Banche
                .FirstOrDefaultAsync(b => b.Predefinita == 1);
        }

        public async Task SetPredefinitaAsync(short idBanca)
        {
            // Prima rimuove il flag da tutte le banche
            await _context.Banche
                .ExecuteUpdateAsync(b => b.SetProperty(x => x.Predefinita, (byte)0));

            // Poi lo imposta sulla banca selezionata
            await _context.Banche
                .Where(b => b.ID == idBanca)
                .ExecuteUpdateAsync(b => b.SetProperty(x => x.Predefinita, (byte)1));
        }

        public async Task<bool> ExistsCodiceAsync(string codice, short excludeId = 0)
        {
            return await _context.Banche
                .AnyAsync(b => b.Codice == codice && b.ID != excludeId);
        }
    }
}