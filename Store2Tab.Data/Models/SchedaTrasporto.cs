using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store2Tab.Data.Models
{
    [Table("TSchedaTrasporto")]
    public class SchedaTrasporto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("IdSchedaTrasporto")]
        public byte IdSchedaTrasporto { get; set; }

        [Column("Vettore_Descrizione")]
        [MaxLength(800)]
        public string VettoreDescrizione { get; set; } = string.Empty;

        [Column("Vettore_PartitaIva")]
        [MaxLength(30)]
        public string VettorePartitaIva { get; set; } = string.Empty;

        [Column("Vettore_AlboAutotrasportatori")]
        [MaxLength(30)]
        public string VettoreAlboAutotrasportatori { get; set; } = string.Empty;

        [Column("Committente_Descrizione")]
        [MaxLength(800)]
        public string CommittenteDescrizione { get; set; } = string.Empty;

        [Column("Committente_PartitaIva")]
        [MaxLength(30)]
        public string CommittentePartitaIva { get; set; } = string.Empty;

        [Column("Caricatore_Descrizione")]
        [MaxLength(800)]
        public string CaricatoreDescrizione { get; set; } = string.Empty;

        [Column("Caricatore_PartitaIva")]
        [MaxLength(30)]
        public string CaricatorePartitaIva { get; set; } = string.Empty;

        [Column("Proprietario_Descrizione")]
        [MaxLength(800)]
        public string ProprietarioDescrizione { get; set; } = string.Empty;

        [Column("Proprietario_PartitaIva")]
        [MaxLength(30)]
        public string ProprietarioPartitaIva { get; set; } = string.Empty;

        [Column("Dichiarazioni")]
        [MaxLength(800)]
        public string Dichiarazioni { get; set; } = string.Empty;

        [Column("Merce_Tipologia")]
        [MaxLength(100)]
        public string MerceTipologia { get; set; } = string.Empty;

        [Column("Merce_QuantitaPeso")]
        [MaxLength(100)]
        public string MerceQuantitaPeso { get; set; } = string.Empty;

        [Column("Merce_LuogoCarico")]
        [MaxLength(100)]
        public string MerceLuogoCarico { get; set; } = string.Empty;

        [Column("Merce_LuogoScarico")]
        [MaxLength(100)]
        public string MerceLuogoScarico { get; set; } = string.Empty;

        [Column("Luogo")]
        [MaxLength(100)]
        public string Luogo { get; set; } = string.Empty;

        [Column("Compilatore")]
        [MaxLength(100)]
        public string Compilatore { get; set; } = string.Empty;
    }
}