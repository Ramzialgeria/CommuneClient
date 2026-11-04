using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneClient.Models.DTOs
{
    public class BureauDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public int NbInscrit { get; set; }
    }
}
