namespace Store2Tab.Data.Models
{
    /// <summary>
    /// Modello per la tabella TPassPianteCEE_Varieta
    /// </summary>
    public class PassPianteCeeVarieta
    {
        /// <summary>
        /// ID univoco della varietà (IDENTITY)
        /// </summary>
        public short IdPassPianteCEE_Varieta { get; set; }

        /// <summary>
        /// Riferimento alla specie botanica
        /// </summary>
        public short IdPassPianteCEE_Specie { get; set; }

        /// <summary>
        /// Nome della varietà
        /// </summary>
        public string Varieta { get; set; } = string.Empty;

        /// <summary>
        /// Proprietà di navigazione per la specie botanica associata
        /// </summary>
        public virtual PassPianteCeeSpecie? SpecieBotanica { get; set; }
    }
}