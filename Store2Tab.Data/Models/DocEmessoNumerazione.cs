using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store2Tab.Data.Models
{
    [Table("TDocEmessoNumerazione")]
    public class DocEmessoNumerazione
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short IdDocEmessoNumerazione { get; set; }

        [Required]
        [MaxLength(25)]
        public string DocEmessoNumerazioneDescrizione { get; set; } = string.Empty;

        [Required]
        [MaxLength(5)]
        public string NumerazioneSigla { get; set; } = string.Empty;

        [Required]
        public byte DocumentoElettronico { get; set; }

        [Required]
        [MaxLength(5)]
        public string FE_TipoDoc { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string DocEmessoTipo_Stampa { get; set; } = string.Empty;
    }
}

