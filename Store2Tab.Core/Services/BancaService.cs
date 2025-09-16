using Store2Tab.Core.Services.Interfaces;
using Store2Tab.Data.Models;
using Store2Tab.Data.Repositories.Interfaces;
using System.Diagnostics;

namespace Store2Tab.Core.Services
{
    public class BancaService : IBancaService
    {
        private readonly IBancaRepository _bancaRepository;

        public BancaService(IBancaRepository bancaRepository)
        {
            _bancaRepository = bancaRepository;
        }

        public async Task<IEnumerable<Banca>> GetAllBancheAsync()
        {
            return await _bancaRepository.GetAllAsync();
        }

        public async Task<Banca?> GetBancaByIdAsync(short id)
        {
            return await _bancaRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Banca>> SearchBancheAsync(string searchTerm)
        {
            return await _bancaRepository.SearchAsync(searchTerm);
        }

        public async Task<Banca> CreateBancaAsync(Banca banca, bool licenzaScaduta = false, Func<string, bool>? confermaUtente = null)
        {
            try
            {
                if (licenzaScaduta)
                    throw new InvalidOperationException("FUNZIONALITÀ NON DISPONIBILE CON LICENZA D'USO SCADUTA.");

                if (!await ValidateBancaAsync(banca))
                    throw new ArgumentException("Dati banca non validi: verificare che Codice e Denominazione siano compilati");

                // 🔹 Controllo unicità codice
                var codiceDuplicato = await _bancaRepository.ExistsCodiceAsync(banca.Codice, banca.ID);
                if (codiceDuplicato)
                {
                    if (confermaUtente != null)
                    {
                        var continua = confermaUtente(
                            $"ATTENZIONE!! IL CODICE INSERITO È GIÀ ASSOCIATO AD UN'ALTRA SCHEDA.\n\nVUOI PROSEGUIRE UGUALMENTE?"
                        );
                        if (!continua)
                            throw new InvalidOperationException("Salvataggio annullato dall'utente.");
                    }
                    else
                    {
                        throw new InvalidOperationException("Codice già usato da un'altra banca.");
                    }
                }

                EnsureDefaultValues(banca);

                var result = await _bancaRepository.AddAsync(banca);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateBancaAsync - Errore: {ex.Message}");
                throw;
            }
        }

        public async Task<Banca> UpdateBancaAsync(Banca banca, Func<string, bool>? confermaUtente = null)
        {
            if (!await ValidateBancaAsync(banca))
                throw new ArgumentException("Dati banca non validi: verificare che Codice e Denominazione siano compilati");

            if (!await _bancaRepository.ExistsAsync(banca.ID))
                throw new ArgumentException("Banca non trovata");

            // 🔹 Controllo unicità codice anche in update
            var codiceDuplicato = await _bancaRepository.ExistsCodiceAsync(banca.Codice, banca.ID);
            if (codiceDuplicato)
            {
                if (confermaUtente != null)
                {
                    var continua = confermaUtente(
                        $"ATTENZIONE!! IL CODICE INSERITO È GIÀ ASSOCIATO AD UN'ALTRA SCHEDA.\n\nVUOI PROSEGUIRE UGUALMENTE?"
                    );
                    if (!continua)
                        throw new InvalidOperationException("Salvataggio annullato dall'utente.");
                }
                else
                {
                    throw new InvalidOperationException("Codice già usato da un'altra banca.");
                }
            }

            EnsureDefaultValues(banca);

            return await _bancaRepository.UpdateAsync(banca);
        }


        public async Task DeleteBancaAsync(short id)
        {
            if (!await _bancaRepository.ExistsAsync(id))
                throw new ArgumentException("Banca non trovata");

            await _bancaRepository.DeleteAsync(id);
        }

        public async Task<bool> BancaExistsAsync(short id)
        {
            return await _bancaRepository.ExistsAsync(id);
        }

        public async Task<Banca?> GetBancaPredefinitaAsync()
        {
            return await _bancaRepository.GetPredefinitaAsync();
        }

        public async Task SetBancaPredefinitaAsync(short idBanca)
        {
            if (!await _bancaRepository.ExistsAsync(idBanca))
                throw new ArgumentException("Banca non trovata");

            await _bancaRepository.SetPredefinitaAsync(idBanca);
        }

        public async Task<bool> ValidateBancaAsync(Banca banca)
        {
            if (banca == null)
            {
                Debug.WriteLine("ValidateBancaAsync - Banca è null");
                return false;
            }

            // Validazioni obbligatorie
            if (string.IsNullOrWhiteSpace(banca.Codice))
            {
                Debug.WriteLine("ValidateBancaAsync - Codice mancante");
                return false;
            }

            if (string.IsNullOrWhiteSpace(banca.Denominazione))
            {
                Debug.WriteLine("ValidateBancaAsync - Denominazione mancante");
                return false;
            }

            // Validazione IBAN opzionale
            if (!string.IsNullOrWhiteSpace(banca.IBAN) && !ValidateIBAN(banca.IBAN))
            {
                Debug.WriteLine("ValidateBancaAsync - IBAN non valido");
                return false;
            }

            Debug.WriteLine("ValidateBancaAsync - Validazione OK");
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Assicura che tutti i campi non-nullable abbiano valori di default
        /// </summary>
        private void EnsureDefaultValues(Banca banca)
        {
            Debug.WriteLine("EnsureDefaultValues - Inizio");

            banca.Codice ??= string.Empty;
            banca.Denominazione ??= string.Empty;
            banca.Agenzia ??= string.Empty;
            banca.ABI ??= string.Empty;
            banca.CAB ??= string.Empty;
            banca.CC ??= string.Empty;
            banca.CIN ??= string.Empty;
            banca.IBAN ??= string.Empty;
            banca.SWIFT ??= string.Empty;
            banca.NoteInterne ??= string.Empty;

            // Assicurati che non ci siano spazi all'inizio/fine
            banca.Codice = banca.Codice.Trim();
            banca.Denominazione = banca.Denominazione.Trim();
            banca.Agenzia = banca.Agenzia.Trim();
            banca.ABI = banca.ABI.Trim();
            banca.CAB = banca.CAB.Trim();
            banca.CC = banca.CC.Trim();
            banca.CIN = banca.CIN.Trim();
            banca.IBAN = banca.IBAN.Trim();
            banca.SWIFT = banca.SWIFT.Trim();
            banca.NoteInterne = banca.NoteInterne.Trim();

            Debug.WriteLine($"EnsureDefaultValues - Risultato: Codice='{banca.Codice}', Denominazione='{banca.Denominazione}'");
        }

        private bool ValidateIBAN(string iban)
        {
            // Rimuove spazi e converte in maiuscolo
            iban = iban.Replace(" ", "").ToUpper();

            // Controllo lunghezza base (minimo 15, massimo 34 caratteri)
            if (iban.Length < 15 || iban.Length > 34)
                return false;

            // Controllo che inizi con 2 lettere seguite da 2 cifre
            if (!char.IsLetter(iban[0]) || !char.IsLetter(iban[1]) ||
                !char.IsDigit(iban[2]) || !char.IsDigit(iban[3]))
                return false;

            return true;
        }

        public Task<Banca> CreateBancaAsync(Banca banca)
        {
            throw new NotImplementedException();
        }

        public Task<Banca> UpdateBancaAsync(Banca banca)
        {
            throw new NotImplementedException();
        }
    }
}