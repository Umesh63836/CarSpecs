using System;
using System.Collections.Generic;

namespace CarSpecAPI.Entities;

public partial class DataSource
{
    public int DataSourceId { get; set; }

    public string SourceName { get; set; } = null!;

    public string SourceType { get; set; } = null!;

    public string? SourceUrl { get; set; }

    public string? Publisher { get; set; }

    public DateOnly? PublishedDate { get; set; }

    public DateTime? RetrievedDate { get; set; }

    public virtual ICollection<ImportBatch> ImportBatches { get; set; } = new List<ImportBatch>();

    public virtual ICollection<ImportFuelEfficiency> ImportFuelEfficiencies { get; set; } = new List<ImportFuelEfficiency>();

    public virtual ICollection<ImportRecord> ImportRecords { get; set; } = new List<ImportRecord>();

    public virtual ICollection<VariantSpecification> VariantSpecifications { get; set; } = new List<VariantSpecification>();
}
