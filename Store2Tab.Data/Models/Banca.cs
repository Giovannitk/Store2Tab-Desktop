// Store2Tab.Data/Models/Banca.cs - VERSIONE CORRETTA
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store2Tab.Data.Models
{
    [Table("TBanca")]
    public class Banca
    {
        [Key]
        [Column("IdBanca")]
        public short ID { get; set; }

        [Column("Codice")]
        [MaxLength(10)]
        [Required]
        public string Codice { get; set; } = string.Empty;

        [Column("Banca")]
        [MaxLength(50)]
        [Required]
        public string Denominazione { get; set; } = string.Empty;

        [Column("Agenzia")]
        [MaxLength(50)]
        public string Agenzia { get; set; } = string.Empty;

        [Column("ABI")]
        [MaxLength(5)]
        public string ABI { get; set; } = string.Empty;

        [Column("CAB")]
        [MaxLength(5)]
        public string CAB { get; set; } = string.Empty;

        [Column("CC")]
        [MaxLength(20)]
        public string CC { get; set; } = string.Empty;

        [Column("CIN")]
        [MaxLength(3)]
        public string CIN { get; set; } = string.Empty;

        [Column("IBAN")]
        [MaxLength(50)]
        public string IBAN { get; set; } = string.Empty;

        [Column("SWIFT")]
        [MaxLength(15)]
        public string SWIFT { get; set; } = string.Empty;

        [Column("NoteInterne")]
        [MaxLength(200)]
        public string NoteInterne { get; set; } = string.Empty;

        [Column("Predefinita")]
        public byte Predefinita { get; set; } = 0;

        // Proprietà computed per il binding WPF
        [NotMapped]
        public bool IsPredefinita
        {
            get => Predefinita == 1;
            set => Predefinita = (byte)(value ? 1 : 0);
        }
    }
}