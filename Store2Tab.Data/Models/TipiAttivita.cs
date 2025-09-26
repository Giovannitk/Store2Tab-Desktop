using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Data.Models
{
    [Table("TAnagraficaAttivita")]
    public class TipiAttivita
    {
        [Key]
        [Column("IdAnagraficaAttivita")]
        public short IdAnagraficaAttivita { get; set; }

        [Column("AnagraficaAttivita")]
        [MaxLength(40)]
        [Required]
        public string Attivita { get; set; } = string.Empty;
    }
}
