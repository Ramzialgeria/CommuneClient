using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneClient.Models.DTOs
{
    public class CentreDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Lib { get; set; }
        public string Type { get; set; }
        public int NbInscrit { get; set; }
        public string CodeCommune { get; set; }
    }
}
