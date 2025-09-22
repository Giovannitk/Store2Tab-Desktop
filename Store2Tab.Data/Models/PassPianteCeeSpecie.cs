using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store2Tab.Data.Models
{
    [Table("TPassPianteCEE_Specie")]
    public class PassPianteCeeSpecie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short IdPassPianteCEE_Specie { get; set; }

        [Required]
        [MaxLength(50)]
        public string Specie { get; set; } = string.Empty;
    }
}