using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store2Tab.Data.Models
{
    [Table("TPassPianteCEE_Numerazione")]
    public class PassPianteCeeNumerazione
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short IdPassPianteCEE_Numerazione { get; set; }

        [Required]
        [MaxLength(40)]
        public string Descrizione { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Sigla { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Prefisso { get; set; } = string.Empty;
    }
}