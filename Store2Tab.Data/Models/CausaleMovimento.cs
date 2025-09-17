using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store2Tab.Data.Models
{
    [Table("TContCausale")]
    public class CausaleMovimento
    {
        [Key]
        [Column("IdContCausale")]
        [Required]
        [StringLength(4)]
        public string Codice { get; set; } = string.Empty;

        [Column("ContCausale")]
        [Required]
        [StringLength(40)]
        public string Descrizione { get; set; } = string.Empty;

        [Column("IdContCausaleContro")]
        [StringLength(4)]
        public string? CodiceControMovimento { get; set; }

        [Column("Utilizzabile")]
        [Required]
        public byte Utilizzabile { get; set; } = 1;

        // Proprietà di navigazione per il contro movimento
        [ForeignKey("CodiceControMovimento")]
        public virtual CausaleMovimento? ControMovimento { get; set; }

        // Proprietà calcolata per la descrizione del contro movimento
        [NotMapped]
        public string DescrizioneControMovimento => ControMovimento?.Descrizione ?? string.Empty;
    }
}