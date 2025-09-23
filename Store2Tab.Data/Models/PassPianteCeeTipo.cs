using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store2Tab.Data.Models
{
    [Table("TPassPianteCEE_Tipo")]
    public class PassPianteCeeTipo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short IdPassPianteCEE_Tipo { get; set; }

        [Required]
        public short IdPassPianteCEE_Numerazione { get; set; }

        [Required]
        [MaxLength(80)]
        public string Descrizione { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ServizioFitosanitario { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CodiceProduttore { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CodiceProduttoreOrig { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PaeseOrigine { get; set; } = string.Empty;

        [Required]
        public byte StampaTesserino { get; set; } = 0;

        [Required]
        public byte PassaportoCEE { get; set; } = 0;

        [Required]
        public byte DocumentoCommerc { get; set; } = 0;

        [Required]
        public byte CatCertCAC { get; set; } = 0;

        public DateTime? Dal { get; set; }

        public DateTime? Al { get; set; }

        [MaxLength(80)]
        public string DescrizioneStamp { get; set; } = string.Empty;

        [Required]
        public byte Raggruppamento { get; set; } = 0;

        // Navigation property
        [ForeignKey("IdPassPianteCEE_Numerazione")]
        public virtual PassPianteCeeNumerazione? Numerazione { get; set; }
    }
}