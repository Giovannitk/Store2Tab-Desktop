using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Data.Models
{
    [Table("TProtocollo")]
    public class Protocollo
    {
        [Key]
        [Column("IdProtocollo")]
        public short IdProtocollo { get; set; }

        [Column("Protocollo")]
        [MaxLength(25)]
        [Required]
        public string NomeProtocollo { get; set; } = string.Empty;
    }
}
