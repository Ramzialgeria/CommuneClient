using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APC_JOURJ.Domain.Entities;

// Where a PVResultat/ResultatBureau row came from. Two independent apps can
// both write bureau results into this same database: this web app directly,
// or an offline-capable desktop app (a separate project) whose data reaches
// here later - through a sync server (Kafka/Redis) or a direct DB injection
// - once connectivity is back. Keeping the origin on the row lets the two
// paths be told apart later (audit, reconciliation, "which source wins on
// conflict") instead of every row looking like it came from the web app.
public enum DataSource
{
    Web = 1,
    Desktop = 2,
}

public class PVResultat
{
    // Guid instead of an identity column: an offline desktop client can't
    // ask this database for "the next" autoincrement value while
    // disconnected, so two sources minting the same integer id is a real
    // collision once both rows land in the same table. A Guid is generated
    // client-side - by this app or by the desktop app - with no
    // coordination required, and CreateVersion7() keeps values roughly
    // time-ordered so the clustered index doesn't fragment the way random
    // (v4) guids would under a high insert rate.
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid IdPV { get; set; } = Guid.CreateVersion7();

    [ForeignKey("Structure")]
    public required string IdStructure { get; set; }

    public virtual Structure? Structure { get; set; }

    public required string TypeStructure { get; set; }

    public required string PVPath { get; set; }

    public string? FileEtatPath { get; set; }

    // Which app produced this PV - see DataSource.
    public required DataSource Source { get; set; }

    public required string CreatedBy { get; set; }

    public required DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ValidatedBy { get; set; }

    public DateTime? ValidatedAt { get; set; }

    // No RowVersion on the PV / result tables: they must work the same on
    // SQL Server and PostgreSQL (UsedSGBD). Which version of a row wins
    // (web app vs desktop sync) is decided from CreatedAt / UpdatedAt /
    // ValidatedAt by the synchronisation app, not by a concurrency token.
}

public class ResultatBureau
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid IdResultat { get; set; } = Guid.CreateVersion7();

    [ForeignKey("Bureau")]
    public long IdBureau { get; set; }

    public virtual Bureau? Bureau { get; set; }

    public int NBParticipation { get; set; }

    public int NBVoixExprime { get; set; }

    public int NBVoixAnnule { get; set; }

    public int NBVoixConteste { get; set; }

    public bool IsSaisie { get; set; } = false;

    public bool IsFinished { get; set; } = false;

    public string? FilePath { get; set; }

    // Which app captured this bureau's result - see DataSource.
    public required DataSource Source { get; set; }

    public string? ValidatedBy { get; set; }

    public DateTime? ValidatedAt { get; set; }

    public required string CreatedBy { get; set; }

    public required DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ResultatList>? ResultatLists { get; set; }

}

public class ResultatList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid IdResultat { get; set; } = Guid.CreateVersion7();

    [ForeignKey("ResultatBureau")]
    public Guid IdResultatBureau { get; set; }

    public virtual ResultatBureau? ResultatBureau { get; set; }

    [ForeignKey("liste")]
    public long Idliste { get; set; }

    public virtual ListesElectorale liste { get; set; }

    public int NbCandidat { get; set; }

    public int NBObtenu { get; set; }

    public bool IsFinished { get; set; } = false;

    public required string CreatedBy { get; set; }

    public required DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ICollection<ResultatCandidat>? ResultatCandidats { get; set; }

}

public class ResultatCandidat
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid IdResultat { get; set; } = Guid.CreateVersion7();

    [ForeignKey("ResultatList")]
    public Guid IdResultatList { get; set; }

    public virtual ResultatList? ResultatList { get; set; }

    [ForeignKey("Candidat")]
    public long IdCandidat { get; set; }

    public virtual Candidat? Candidat { get; set; }

    public int NBObtenu { get; set; }

    public required string CreatedBy { get; set; }

    public required DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
