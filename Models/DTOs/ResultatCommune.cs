using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APC_JOURJ.Domain.Entities;


    public class ResultatCommune
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdResultatCommune { get; set; }

        [ForeignKey("Commune")]
        public required string CodeCommune { get; set; }

        public virtual Commune? Commune { get; set; }

        public int NombreBureau { get; set; }

        public int NombreListe { get; set; }

        public int NombreSiege { get; set; }

        public int NombreElecteur { get; set; }

        public int NBParticipation { get; set; }

        public int NBVoixExprime { get; set; }

        public int NBVoixAnnule { get; set; }

        public int NBVoixConteste { get; set; }

        public int CoefElectoral { get; set; }

        public int NombreEliminatiore { get; set; }

        public virtual ICollection<ResultatListCommune>? ResultatLists { get; set; }

    }

    public class ResultatListCommune
{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdResultatListCommune { get; set; }

        [ForeignKey("ListeElectoral")]
        public long IdListe { get; set; }

        public virtual ListesElectorale? ListeElectoral { get; set; }

        [ForeignKey("ResultatCommune")]
        public long IdResultatCommune { get; set; }

        public virtual ResultatCommune? ResultatCommune { get; set; }

        public int NBObtenu { get; set; }

        public int NbObtenuRested { get; set; }

        public int NbSiege { get; set; }

        public int NbSiegeRested { get; set; }

        public bool IsParticiped { get; set; }

        public virtual ICollection<ResultatCandidatCommune>? ResultatCandidats { get; set; }

    }

    public class ResultatCandidatCommune
{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdResultatCandidatCommune { get; set; }

        [ForeignKey("ResultatListCommune")]
        public long IdResultatListCommune { get; set; }

        public virtual ResultatListCommune? ResultatListCommune { get; set; }

        [ForeignKey("Candidat")]
        public long IdCandidat { get; set; }

        public virtual Candidat? Candidat { get; set; }

        public int NbObtenu { get; set; }

        public bool IsWin { get; set; } = false;
    }

