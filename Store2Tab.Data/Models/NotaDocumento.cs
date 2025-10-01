using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Data.Models
{
    [Table("TNotaDocumento")]
    public class NotaDocumento
    {
        [Key]
        [Column("IdNotaDocumento")]
        public short IdNotaDocumento { get; set; }

        [Column("NotaDocumento")]
        [MaxLength(400)]
        [Required]
        public string Nota { get; set; } = string.Empty;
    }
}
