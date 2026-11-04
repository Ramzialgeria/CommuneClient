using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuneClient.Models.DTOs
{
    public class CandidatDto
    {
        public int Id { get; set; }
        public string FullNameAr { get; set; }
        public string FullNameFr { get; set; }
        public int? Ordre { get; set; }
    }
}
