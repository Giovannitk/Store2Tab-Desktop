using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store2Tab.Data.Models
{
    [Table("TOrdineNumerazione")]
    public class NumerazioneOrdini
    {
        [Key]
        [Column("IdOrdineNumerazione")]
        public short IdOrdineNumerazione { get; set; }

        [Column("OrdineNumerazione")]
        [MaxLength(25)]
        [Required]
        public string OrdineNumerazione { get; set; } = string.Empty;

        [Column("NumerazioneSigla")]
        [MaxLength(5)]
        public string NumerazioneSigla { get; set; } = string.Empty;

        [Column("DefaultCliente")]
        public byte DefaultCliente { get; set; }

        [Column("DefaultFornitore")]
        public byte DefaultFornitore { get; set; }

        [Column("Stampa_DestinatarioTel")]
        public byte StampaDestinatarioTel { get; set; }

        [Column("Stampa_Logo")]
        [MaxLength(50)]
        public string StampaLogo { get; set; } = string.Empty;

        [Column("Stampa_CodiciArticolo")]
        public byte StampaCodiciArticolo { get; set; }
    }
}