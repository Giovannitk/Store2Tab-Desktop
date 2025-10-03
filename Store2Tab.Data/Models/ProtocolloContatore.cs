using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store2Tab.Data.Models
{
    [Table("TProtocolloContatore")]
    public class ProtocolloContatore
    {
        // Chiave primaria composta
        [Key]
        [Column("IdProtocollo", Order = 0)]
        public short IdProtocollo { get; set; }


        [Key]
        [Column("Esercizio", Order = 1)]
        public short Esercizio { get; set; }

        [Column("Contatore")]
        [Required]
        public int Contatore { get; set; }

        // Navigation property verso Protocollo
        [ForeignKey(nameof(IdProtocollo))]
        public virtual Protocollo? Protocollo { get; set; }
    }
}