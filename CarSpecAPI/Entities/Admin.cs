using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<ImportAuditChange> ImportAuditChanges { get; set; } = new List<ImportAuditChange>();

    public virtual ICollection<ImportBatch> ImportBatches { get; set; } = new List<ImportBatch>();

    public virtual ICollection<ImportRecord> ImportRecords { get; set; } = new List<ImportRecord>();

    public virtual ICollection<ImportWarning> ImportWarnings { get; set; } = new List<ImportWarning>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
