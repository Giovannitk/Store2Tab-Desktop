using Store2Tab.Data.Models;

namespace Store2Tab.Core.Services.Interfaces
{
    public interface ICausaleMovimentoService
    {
        Task<List<CausaleMovimento>> GetCausaliMovimentoAsync(string? filtrocodice = null, string? filtroDescrizione = null);
        Task<CausaleMovimento?> GetCausaleMovimentoByCodeAsync(string codice);
        Task<bool> SalvaCausaleMovimentoAsync(CausaleMovimento causale);
        Task<bool> CancellaCausaleMovimentoAsync(string codice);
        Task<bool> EsisteCodiceAsync(string codice, string? codiceOriginale = null);
        Task<List<CausaleMovimento>> CercaCausaliPerSelezioneAsync(string? filtroTesto = null);
        Task<string> GetNextCodiceAsync();
    }
}