using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store2Tab.Data.Models
{
    public class PassPianteCEE_Portinnesto
    {
        public short IdPassPianteCEE_Portinnesto { get; set; }
        public short IdPassPianteCEE_Specie { get; set; }
        public string Portinnesto { get; set; } = string.Empty;
        public virtual PassPianteCeeSpecie? SpecieBotanica { get; set; }
    }
}
