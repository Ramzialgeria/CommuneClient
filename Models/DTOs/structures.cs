using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommuneClient.Models.DTOs
{
    public class Structure
    {

        public string IdStructure { get; set; }

        public string LIBELLEARABE { get; set; }

        public string LIBELLELATIN { get; set; }
    }

    public class Wilaya : Structure
    {
        public long IdWilaya { get; set; }

        [JsonIgnore]
        public virtual ICollection<Commune> Communes { get; set; }
    }

    public class Commune : Structure
    {
        [ForeignKey("Wilaya")]
        public string CodeWilaya { get; set; }

        public virtual Wilaya Wilaya { get; set; }

        public long IdCommune { get; set; }
        [InverseProperty("Commune")]
        public virtual ICollection<CentreDto> Centres { get; set; }

    }


    public class WilayaEC
    {

        public string CodeWilaya { get; set; }

        public string LIBELLEARABE { get; set; }

        public string LIBELLELATIN { get; set; }

        public long IdWilaya { get; set; }

        public virtual ICollection<CommuneEC> Communes { get; set; }
    }

    public class CommuneEC
    {

        public string CodeCommune { get; set; }

        public long IdCommune { get; set; }

        [ForeignKey("WilayaEC")]
        public string CodeWilaya { get; set; }

        public virtual WilayaEC WilayaEC { get; set; }

        public string LIBELLEARABE { get; set; }

        public string LIBELLELATIN { get; set; }
    }
}