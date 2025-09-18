using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Data.Models
{
    [Table("TPagamentoMezzo")]
    public class PagamentoMezzo
    {
        [Key]
        [Column("IdPagamentoMezzo")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short IdPagamentoMezzo { get; set; }

        [Column("PagamentoMezzo")]
        [Required]
        [StringLength(40)]
        public string pagamentoMezzo { get; set; } = string.Empty;

        [Column("Gruppo")]
        [Required]
        [StringLength(3)]
        public string Gruppo { get; set; } = string.Empty;

        [Column("PagamentoInterbancario")]
        [Required]
        public byte PagamentoInterbancario { get; set; } = 0;

        [Column("IdBanca_Emesso")]
        [Required]
        public short IdBanca_Emesso { get; set; } = 0;

        [Column("IdBanca_Ricevuto")]
        [Required]
        public short IdBanca_Ricevuto { get; set; } = 0;

        [Column("FE_ModPag")]
        [Required]
        [StringLength(10)]
        public string FE_ModPag { get; set; } = string.Empty;

        // Constructor per assicurarsi che tutti i valori siano inizializzati
        public PagamentoMezzo()
        {
            pagamentoMezzo = string.Empty;
            Gruppo = string.Empty;
            FE_ModPag = string.Empty;
            PagamentoInterbancario = 0;
            IdBanca_Emesso = 0;
            IdBanca_Ricevuto = 0;
        }
    }
}