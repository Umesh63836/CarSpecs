using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Entities;

public partial class CarsDbContext : DbContext
{
    private readonly IConfiguration configuration;

    public CarsDbContext()
    {
    }

    public CarsDbContext(DbContextOptions<CarsDbContext> options, IConfiguration configuration)
        : base(options)
    {
        this.configuration = configuration;
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Colour> Colours { get; set; }

    public virtual DbSet<DataSource> DataSources { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Drivetrain> Drivetrains { get; set; }

    public virtual DbSet<Engine> Engines { get; set; }

    public virtual DbSet<EngineLegacy20260909> EngineLegacy20260909s { get; set; }

    public virtual DbSet<EngineLegacyBackup20260909> EngineLegacyBackup20260909s { get; set; }

    public virtual DbSet<EnginePerformance> EnginePerformances { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<FeatureCategory> FeatureCategories { get; set; }

    public virtual DbSet<FeatureValueOption> FeatureValueOptions { get; set; }

    public virtual DbSet<FuelType> FuelTypes { get; set; }

    public virtual DbSet<ImportAuditChange> ImportAuditChanges { get; set; }

    public virtual DbSet<ImportBatch> ImportBatches { get; set; }

    public virtual DbSet<ImportDocument> ImportDocuments { get; set; }

    public virtual DbSet<ImportDocumentPage> ImportDocumentPages { get; set; }

    public virtual DbSet<ImportDrivetrain> ImportDrivetrains { get; set; }

    public virtual DbSet<ImportEngine> ImportEngines { get; set; }

    public virtual DbSet<ImportEnginePerformance> ImportEnginePerformances { get; set; }

    public virtual DbSet<ImportFeature> ImportFeatures { get; set; }

    public virtual DbSet<ImportFeatureValueOption> ImportFeatureValueOptions { get; set; }

    public virtual DbSet<ImportFeatureVariant> ImportFeatureVariants { get; set; }

    public virtual DbSet<ImportFuelEfficiency> ImportFuelEfficiencies { get; set; }

    public virtual DbSet<ImportFuelEfficiencyVariant> ImportFuelEfficiencyVariants { get; set; }

    public virtual DbSet<ImportModel> ImportModels { get; set; }

    public virtual DbSet<ImportModelDimension> ImportModelDimensions { get; set; }

    public virtual DbSet<ImportMotorPerformance> ImportMotorPerformances { get; set; }

    public virtual DbSet<ImportPowertrain> ImportPowertrains { get; set; }

    public virtual DbSet<ImportRecord> ImportRecords { get; set; }

    public virtual DbSet<ImportSpecification> ImportSpecifications { get; set; }

    public virtual DbSet<ImportSpecificationVariant> ImportSpecificationVariants { get; set; }

    public virtual DbSet<ImportTransmission> ImportTransmissions { get; set; }

    public virtual DbSet<ImportVariant> ImportVariants { get; set; }

    public virtual DbSet<ImportVariantParent> ImportVariantParents { get; set; }

    public virtual DbSet<ImportWarning> ImportWarnings { get; set; }

    public virtual DbSet<ImportWarranty> ImportWarranties { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<ModelDimension> ModelDimensions { get; set; }

    public virtual DbSet<ModelSafetyRating> ModelSafetyRatings { get; set; }

    public virtual DbSet<MotorPerformance> MotorPerformances { get; set; }

    public virtual DbSet<Powertrain> Powertrains { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Specification> Specifications { get; set; }

    public virtual DbSet<SpecificationCategory> SpecificationCategories { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<SubDistrict> SubDistricts { get; set; }

    public virtual DbSet<Transmission> Transmissions { get; set; }

    public virtual DbSet<Variant> Variants { get; set; }

    public virtual DbSet<VariantColour> VariantColours { get; set; }

    public virtual DbSet<VariantFeature> VariantFeatures { get; set; }

    public virtual DbSet<VariantFuelEfficiency> VariantFuelEfficiencies { get; set; }

    public virtual DbSet<VariantImage> VariantImages { get; set; }

    public virtual DbSet<VariantLegacyBackup20260909> VariantLegacyBackup20260909s { get; set; }

    public virtual DbSet<VariantSpecification> VariantSpecifications { get; set; }

    public virtual DbSet<Warranty> Warranties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__719FE48824C1D712");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.Username, "UQ__Admin__536C85E47A6816AB").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("Brand");

            entity.HasIndex(e => e.BrandName, "UQ_Brand_BrandName").IsUnique();

            entity.Property(e => e.BrandName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Brand_IsActive");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Colour>(entity =>
        {
            entity.ToTable("Colour");

            entity.HasIndex(e => e.ColourName, "UQ_Colour_ColourName").IsUnique();

            entity.Property(e => e.ColourName).HasMaxLength(100);
            entity.Property(e => e.ColourType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HexCode)
                .HasMaxLength(7)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DataSource>(entity =>
        {
            entity.ToTable("DataSource");

            entity.Property(e => e.Publisher).HasMaxLength(200);
            entity.Property(e => e.SourceName).HasMaxLength(200);
            entity.Property(e => e.SourceType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SourceUrl)
                .HasMaxLength(1000)
                .IsUnicode(false);
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.DistrictCode);

            entity.ToTable("District");

            entity.HasIndex(e => e.StateCode, "IX_District_StateCode");

            entity.Property(e => e.DistrictCode).ValueGeneratedNever();
            entity.Property(e => e.DistrictName).HasMaxLength(150);

            entity.HasOne(d => d.StateCodeNavigation).WithMany(p => p.Districts)
                .HasForeignKey(d => d.StateCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_District_State");
        });

        modelBuilder.Entity<Drivetrain>(entity =>
        {
            entity.ToTable("Drivetrain");

            entity.HasIndex(e => e.DrivetrainType, "UQ_Drivetrain_DrivetrainType").IsUnique();

            entity.Property(e => e.DifferentialType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DrivetrainType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Engine>(entity =>
        {
            entity.HasKey(e => e.EngineId).HasName("PK_Engine_New");

            entity.ToTable("Engine");

            entity.HasIndex(e => e.PowertrainId, "UX_Engine_PowertrainId").IsUnique();

            entity.Property(e => e.EngineId).ValueGeneratedNever();
            entity.Property(e => e.Aspiration)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Displacement).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EmissionStandard)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EngineName)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.EngineType)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Powertrain).WithOne(p => p.Engine)
                .HasForeignKey<Engine>(d => d.PowertrainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Engine_Powertrain");
        });

        modelBuilder.Entity<EngineLegacy20260909>(entity =>
        {
            entity.HasKey(e => e.EngineId).HasName("PK_Engine");

            entity.ToTable("Engine_Legacy_20260909");

            entity.Property(e => e.Aspiration)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BatteryCapacityKwh)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("BatteryCapacityKWh");
            entity.Property(e => e.CombinedMaxPower).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CombinedMaxPowerRpm).HasColumnName("CombinedMaxPowerRPM");
            entity.Property(e => e.CombinedMaxPowerRpmmax).HasColumnName("CombinedMaxPowerRPMMax");
            entity.Property(e => e.CombinedMaxPowerRpmmin).HasColumnName("CombinedMaxPowerRPMMin");
            entity.Property(e => e.CombinedMaxTorque).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CombinedMaxTorqueRpm).HasColumnName("CombinedMaxTorqueRPM");
            entity.Property(e => e.CombinedMaxTorqueRpmmax).HasColumnName("CombinedMaxTorqueRPMMax");
            entity.Property(e => e.CombinedMaxTorqueRpmmin).HasColumnName("CombinedMaxTorqueRPMMin");
            entity.Property(e => e.Displacement).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.EmissionStandard)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.EngineName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EngineType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.FuelType).WithMany(p => p.EngineLegacy20260909s)
                .HasForeignKey(d => d.FuelTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Engine_FuelType");
        });

        modelBuilder.Entity<EngineLegacyBackup20260909>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Engine_LegacyBackup_20260909");

            entity.Property(e => e.Aspiration)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BatteryCapacityKwh)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("BatteryCapacityKWh");
            entity.Property(e => e.CombinedMaxPower).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CombinedMaxPowerRpm).HasColumnName("CombinedMaxPowerRPM");
            entity.Property(e => e.CombinedMaxPowerRpmmax).HasColumnName("CombinedMaxPowerRPMMax");
            entity.Property(e => e.CombinedMaxPowerRpmmin).HasColumnName("CombinedMaxPowerRPMMin");
            entity.Property(e => e.CombinedMaxTorque).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CombinedMaxTorqueRpm).HasColumnName("CombinedMaxTorqueRPM");
            entity.Property(e => e.CombinedMaxTorqueRpmmax).HasColumnName("CombinedMaxTorqueRPMMax");
            entity.Property(e => e.CombinedMaxTorqueRpmmin).HasColumnName("CombinedMaxTorqueRPMMin");
            entity.Property(e => e.Displacement).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.EmissionStandard)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.EngineId).ValueGeneratedOnAdd();
            entity.Property(e => e.EngineName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EngineType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EnginePerformance>(entity =>
        {
            entity.ToTable("EnginePerformance");

            entity.HasIndex(e => new { e.EngineId, e.ModeName }, "UX_EnginePerformance_Engine_Mode").IsUnique();

            entity.Property(e => e.MaxPower).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxPowerRpm).HasColumnName("MaxPowerRPM");
            entity.Property(e => e.MaxPowerRpmmax).HasColumnName("MaxPowerRPMMax");
            entity.Property(e => e.MaxPowerRpmmin).HasColumnName("MaxPowerRPMMin");
            entity.Property(e => e.MaxPowerUnit).HasMaxLength(20);
            entity.Property(e => e.MaxTorque).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxTorqueRpm).HasColumnName("MaxTorqueRPM");
            entity.Property(e => e.MaxTorqueRpmmax).HasColumnName("MaxTorqueRPMMax");
            entity.Property(e => e.MaxTorqueRpmmin).HasColumnName("MaxTorqueRPMMin");
            entity.Property(e => e.MaxTorqueUnit).HasMaxLength(20);
            entity.Property(e => e.ModeName).HasMaxLength(100);

            entity.HasOne(d => d.Engine).WithMany(p => p.EnginePerformances)
                .HasForeignKey(d => d.EngineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EnginePerformance_Engine");

            entity.HasOne(d => d.FuelType).WithMany(p => p.EnginePerformances)
                .HasForeignKey(d => d.FuelTypeId)
                .HasConstraintName("FK_EnginePerformance_FuelType");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.ToTable("Feature");

            entity.HasIndex(e => new { e.FeatureCategoryId, e.FeatureName }, "UQ_Feature_Category_Name").IsUnique();

            entity.HasIndex(e => e.FeatureCode, "UX_Feature_FeatureCode")
                .IsUnique()
                .HasFilter("([FeatureCode] IS NOT NULL)");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ExtractionGuidance).HasMaxLength(2000);
            entity.Property(e => e.FeatureCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FeatureName).HasMaxLength(150);
            entity.Property(e => e.IconUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Feature_IsActive");
            entity.Property(e => e.Unit)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ValueType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.FeatureCategory).WithMany(p => p.Features)
                .HasForeignKey(d => d.FeatureCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Feature_FeatureCategory");
        });

        modelBuilder.Entity<FeatureCategory>(entity =>
        {
            entity.ToTable("FeatureCategory");

            entity.HasIndex(e => e.CategoryName, "UQ_FeatureCategory_CategoryName").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_FeatureCategory_IsActive");
        });

        modelBuilder.Entity<FeatureValueOption>(entity =>
        {
            entity.ToTable("FeatureValueOption");

            entity.HasIndex(e => e.FeatureId, "IX_FeatureValueOption_FeatureId");

            entity.HasIndex(e => new { e.FeatureId, e.Value }, "UQ_FeatureValueOption_Feature_Value").IsUnique();

            entity.Property(e => e.DisplayName).HasMaxLength(200);
            entity.Property(e => e.DisplayOrder).HasDefaultValue(10, "DF_FeatureValueOption_DisplayOrder");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_FeatureValueOption_IsActive");
            entity.Property(e => e.Value)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Feature).WithMany(p => p.FeatureValueOptions)
                .HasForeignKey(d => d.FeatureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FeatureValueOption_Feature");
        });

        modelBuilder.Entity<FuelType>(entity =>
        {
            entity.ToTable("FuelType");

            entity.HasIndex(e => e.FuelType1, "UQ_FuelType_FuelType").IsUnique();

            entity.Property(e => e.FuelType1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FuelType");
        });

        modelBuilder.Entity<ImportAuditChange>(entity =>
        {
            entity.ToTable("ImportAuditChange");

            entity.HasIndex(e => e.AuditChangeId, "IX_ImportAuditChange_AuditId");

            entity.HasIndex(e => e.ImportBatchId, "IX_ImportAuditChange_Batch");

            entity.HasIndex(e => e.TargetImportRecordId, "IX_ImportAuditChange_Record");

            entity.Property(e => e.Action)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.AuditChangeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EntityKey).HasMaxLength(200);
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FieldName).HasMaxLength(200);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.ImportBatch).WithMany(p => p.ImportAuditChanges)
                .HasForeignKey(d => d.ImportBatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportAuditChange_Batch");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.ImportAuditChanges)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK_ImportAuditChange_Admin");

            entity.HasOne(d => d.TargetImportRecord).WithMany(p => p.ImportAuditChanges)
                .HasForeignKey(d => d.TargetImportRecordId)
                .HasConstraintName("FK_ImportAuditChange_Record");
        });

        modelBuilder.Entity<ImportBatch>(entity =>
        {
            entity.ToTable("ImportBatch");

            entity.HasIndex(e => e.DataSourceId, "IX_ImportBatch_DataSourceId");

            entity.Property(e => e.Aimodel)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("AIModel");
            entity.Property(e => e.AiprocessedAt).HasColumnName("AIProcessedAt");
            entity.Property(e => e.AiresultJson).HasColumnName("AIResultJson");
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.StartedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_ImportBatch_StartedAt");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pending", "DF_ImportBatch_Status");

            entity.HasOne(d => d.DataSource).WithMany(p => p.ImportBatches)
                .HasForeignKey(d => d.DataSourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportBatch_DataSource");

            entity.HasOne(d => d.ImportedByNavigation).WithMany(p => p.ImportBatches)
                .HasForeignKey(d => d.ImportedBy)
                .HasConstraintName("FK_ImportBatch_Admin");
        });

        modelBuilder.Entity<ImportDocument>(entity =>
        {
            entity.ToTable("ImportDocument");

            entity.HasIndex(e => e.ImportBatchId, "IX_ImportDocument_ImportBatchId");

            entity.Property(e => e.BlobContainer)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BlobPath)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ContentHash)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_ImportDocument_CreatedAt");
            entity.Property(e => e.DocumentName).HasMaxLength(300);
            entity.Property(e => e.DocumentType)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ExtractionStatus)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.LocalFilePath).HasMaxLength(1000);
            entity.Property(e => e.MimeType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SourceUrl)
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.HasOne(d => d.ImportBatch).WithMany(p => p.ImportDocuments)
                .HasForeignKey(d => d.ImportBatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportDocument_ImportBatch");
        });

        modelBuilder.Entity<ImportDocumentPage>(entity =>
        {
            entity.ToTable("ImportDocumentPage");

            entity.HasIndex(e => new { e.ImportDocumentId, e.PageNumber }, "UQ_ImportDocumentPage").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_ImportDocumentPage_CreatedAt");
            entity.Property(e => e.PageImagePath)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.PagePdfPath)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ProcessingStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ImportDocument).WithMany(p => p.ImportDocumentPages)
                .HasForeignKey(d => d.ImportDocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportDocumentPage_ImportDocument");
        });

        modelBuilder.Entity<ImportDrivetrain>(entity =>
        {
            entity.ToTable("ImportDrivetrain");

            entity.HasIndex(e => new { e.ImportModelId, e.DrivetrainRef }, "UX_ImportDrivetrain_Model_Ref").IsUnique();

            entity.Property(e => e.DifferentialType).HasMaxLength(100);
            entity.Property(e => e.DrivetrainRef)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.DrivetrainType).HasMaxLength(100);

            entity.HasOne(d => d.ImportModel).WithMany(p => p.ImportDrivetrains)
                .HasForeignKey(d => d.ImportModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportDrivetrain_Model");
        });

        modelBuilder.Entity<ImportEngine>(entity =>
        {
            entity.ToTable("ImportEngine");

            entity.HasIndex(e => e.ImportPowertrainId, "UX_ImportEngine_Powertrain").IsUnique();

            entity.Property(e => e.Aspiration).HasMaxLength(100);
            entity.Property(e => e.Displacement).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.EmissionStandard).HasMaxLength(100);
            entity.Property(e => e.EngineName).HasMaxLength(200);
            entity.Property(e => e.EngineRef)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.EngineType).HasMaxLength(100);

            entity.HasOne(d => d.ImportPowertrain).WithOne(p => p.ImportEngine)
                .HasForeignKey<ImportEngine>(d => d.ImportPowertrainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportEngine_Powertrain");
        });

        modelBuilder.Entity<ImportEnginePerformance>(entity =>
        {
            entity.ToTable("ImportEnginePerformance");

            entity.Property(e => e.FuelTypeName).HasMaxLength(50);
            entity.Property(e => e.MaxPower).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MaxPowerRpm).HasColumnName("MaxPowerRPM");
            entity.Property(e => e.MaxPowerRpmmax).HasColumnName("MaxPowerRPMMax");
            entity.Property(e => e.MaxPowerRpmmin).HasColumnName("MaxPowerRPMMin");
            entity.Property(e => e.MaxPowerUnit).HasMaxLength(20);
            entity.Property(e => e.MaxTorque).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MaxTorqueRpm).HasColumnName("MaxTorqueRPM");
            entity.Property(e => e.MaxTorqueRpmmax).HasColumnName("MaxTorqueRPMMax");
            entity.Property(e => e.MaxTorqueRpmmin).HasColumnName("MaxTorqueRPMMin");
            entity.Property(e => e.MaxTorqueUnit).HasMaxLength(20);
            entity.Property(e => e.ModeName).HasMaxLength(100);

            entity.HasOne(d => d.FuelType).WithMany(p => p.ImportEnginePerformances)
                .HasForeignKey(d => d.FuelTypeId)
                .HasConstraintName("FK_ImportEnginePerformance_FuelType");

            entity.HasOne(d => d.ImportEngine).WithMany(p => p.ImportEnginePerformances)
                .HasForeignKey(d => d.ImportEngineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportEnginePerformance_Engine");
        });

        modelBuilder.Entity<ImportFeature>(entity =>
        {
            entity.ToTable("ImportFeature");

            entity.Property(e => e.ImportFeatureId).ValueGeneratedNever();
            entity.Property(e => e.FeatureCode).HasMaxLength(200);

            entity.HasOne(d => d.Feature).WithMany(p => p.ImportFeatures)
                .HasForeignKey(d => d.FeatureId)
                .HasConstraintName("FK_ImportFeature_Feature");
        });

        modelBuilder.Entity<ImportFeatureValueOption>(entity =>
        {
            entity.HasKey(e => new { e.ImportFeatureVariantId, e.ValueOptionId });

            entity.ToTable("ImportFeatureValueOption");

            entity.Property(e => e.ImportFeatureValueOptionId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.ImportFeatureVariant).WithMany(p => p.ImportFeatureValueOptions)
                .HasForeignKey(d => d.ImportFeatureVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportFeatureValueOption_ImportFeatureVariant");

            entity.HasOne(d => d.ValueOption).WithMany(p => p.ImportFeatureValueOptions)
                .HasForeignKey(d => d.ValueOptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportFeatureValueOption_ValueOption");
        });

        modelBuilder.Entity<ImportFeatureVariant>(entity =>
        {
            entity.ToTable("ImportFeatureVariant");

            entity.Property(e => e.Confidence).HasColumnType("decimal(5, 4)");
            entity.Property(e => e.SourceColumn).HasMaxLength(200);

            entity.HasOne(d => d.ImportFeature).WithMany(p => p.ImportFeatureVariants)
                .HasForeignKey(d => d.ImportFeatureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportFeatureVariant_Feature");

            entity.HasOne(d => d.ImportVariant).WithMany(p => p.ImportFeatureVariants)
                .HasForeignKey(d => d.ImportVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportFeatureVariant_Variant");
        });

        modelBuilder.Entity<ImportFuelEfficiency>(entity =>
        {
            entity.ToTable("ImportFuelEfficiency");

            entity.Property(e => e.Confidence).HasColumnType("decimal(5, 4)");
            entity.Property(e => e.FuelEfficiency).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.FuelEfficiencyUnit).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_ImportFuelEfficiency_IsActive");
            entity.Property(e => e.SourceColumn).HasMaxLength(500);
            entity.Property(e => e.SourceType).HasMaxLength(50);

            entity.HasOne(d => d.Source).WithMany(p => p.ImportFuelEfficiencies)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK_ImportFuelEfficiency_DataSource");
        });

        modelBuilder.Entity<ImportFuelEfficiencyVariant>(entity =>
        {
            entity.ToTable("ImportFuelEfficiencyVariant");

            entity.HasIndex(e => new { e.ImportFuelEfficiencyId, e.ImportVariantId }, "UQ_ImportFuelEfficiencyVariant_Fuel_Variant").IsUnique();

            entity.HasOne(d => d.ImportFuelEfficiency).WithMany(p => p.ImportFuelEfficiencyVariants)
                .HasForeignKey(d => d.ImportFuelEfficiencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportFuelEfficiencyVariant_Fuel");

            entity.HasOne(d => d.ImportVariant).WithMany(p => p.ImportFuelEfficiencyVariants)
                .HasForeignKey(d => d.ImportVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportFuelEfficiencyVariant_Variant");
        });

        modelBuilder.Entity<ImportModel>(entity =>
        {
            entity.ToTable("ImportModel");

            entity.HasIndex(e => e.ModelName, "IX_ImportModel_ModelName");

            entity.HasIndex(e => e.ImportRecordId, "UQ_ImportModel_ImportRecord").IsUnique();

            entity.Property(e => e.BodyType).HasMaxLength(100);
            entity.Property(e => e.BrandName).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.ModelImageUrl).HasMaxLength(1000);
            entity.Property(e => e.ModelName).HasMaxLength(200);

            entity.HasOne(d => d.ImportRecord).WithOne(p => p.ImportModel)
                .HasForeignKey<ImportModel>(d => d.ImportRecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportModel_ImportRecord");
        });

        modelBuilder.Entity<ImportModelDimension>(entity =>
        {
            entity.HasIndex(e => e.ImportModelId, "UQ_ImportModelDimensions_Model").IsUnique();

            entity.Property(e => e.BootSpaceLitres).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Confidence).HasColumnType("decimal(5, 4)");
            entity.Property(e => e.EvidenceText).HasMaxLength(1000);
            entity.Property(e => e.FuelTankCapacityLitres).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.GroundClearanceMm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("GroundClearanceMM");
            entity.Property(e => e.HeightMm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("HeightMM");
            entity.Property(e => e.LengthMm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("LengthMM");
            entity.Property(e => e.WheelbaseMm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("WheelbaseMM");
            entity.Property(e => e.WidthMm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("WidthMM");

            entity.HasOne(d => d.ImportModel).WithOne(p => p.ImportModelDimension)
                .HasForeignKey<ImportModelDimension>(d => d.ImportModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportModelDimensions_Model");
        });

        modelBuilder.Entity<ImportMotorPerformance>(entity =>
        {
            entity.ToTable("ImportMotorPerformance");

            entity.Property(e => e.MaxPower).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MaxPowerRpm).HasColumnName("MaxPowerRPM");
            entity.Property(e => e.MaxPowerRpmmax).HasColumnName("MaxPowerRPMMax");
            entity.Property(e => e.MaxPowerRpmmin).HasColumnName("MaxPowerRPMMin");
            entity.Property(e => e.MaxPowerUnit).HasMaxLength(20);
            entity.Property(e => e.MaxTorque).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.MaxTorqueRpm).HasColumnName("MaxTorqueRPM");
            entity.Property(e => e.MaxTorqueRpmmax).HasColumnName("MaxTorqueRPMMax");
            entity.Property(e => e.MaxTorqueRpmmin).HasColumnName("MaxTorqueRPMMin");
            entity.Property(e => e.MaxTorqueUnit).HasMaxLength(20);
            entity.Property(e => e.MotorName).HasMaxLength(200);

            entity.HasOne(d => d.ImportPowertrain).WithMany(p => p.ImportMotorPerformances)
                .HasForeignKey(d => d.ImportPowertrainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportMotorPerformance_Powertrain");
        });

        modelBuilder.Entity<ImportPowertrain>(entity =>
        {
            entity.ToTable("ImportPowertrain");

            entity.HasIndex(e => new { e.ImportModelId, e.PowertrainRef }, "UX_ImportPowertrain_Model_Ref").IsUnique();

            entity.Property(e => e.BatteryCapacityKwh)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("BatteryCapacityKWh");
            entity.Property(e => e.CombinedMaxPower).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CombinedMaxPowerRpm).HasColumnName("CombinedMaxPowerRPM");
            entity.Property(e => e.CombinedMaxPowerRpmmax).HasColumnName("CombinedMaxPowerRPMMax");
            entity.Property(e => e.CombinedMaxPowerRpmmin).HasColumnName("CombinedMaxPowerRPMMin");
            entity.Property(e => e.CombinedMaxPowerUnit).HasMaxLength(20);
            entity.Property(e => e.CombinedMaxTorque).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CombinedMaxTorqueRpm).HasColumnName("CombinedMaxTorqueRPM");
            entity.Property(e => e.CombinedMaxTorqueRpmmax).HasColumnName("CombinedMaxTorqueRPMMax");
            entity.Property(e => e.CombinedMaxTorqueRpmmin).HasColumnName("CombinedMaxTorqueRPMMin");
            entity.Property(e => e.CombinedMaxTorqueUnit).HasMaxLength(20);
            entity.Property(e => e.EngineRef)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.PowertrainRef)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.PowertrainType)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.ImportModel).WithMany(p => p.ImportPowertrains)
                .HasForeignKey(d => d.ImportModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportPowertrain_Model");
        });

        modelBuilder.Entity<ImportRecord>(entity =>
        {
            entity.ToTable("ImportRecord");

            entity.HasIndex(e => e.ImportBatchId, "IX_ImportRecord_Batch");

            entity.HasIndex(e => e.ImportDocumentId, "IX_ImportRecord_Document");

            entity.HasIndex(e => new { e.EntityType, e.EntityKey }, "IX_ImportRecord_Entity");

            entity.HasIndex(e => e.ParentImportRecordId, "IX_ImportRecord_Parent");

            entity.HasIndex(e => new { e.ImportBatchId, e.ReviewStatus }, "IX_ImportRecord_ReviewStatus");

            entity.Property(e => e.AuditChangeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Confidence).HasColumnType("decimal(5, 4)");
            entity.Property(e => e.EntityKey).HasMaxLength(200);
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FieldName).HasMaxLength(200);
            entity.Property(e => e.NumericValue).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.ReviewStatus)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Unit).HasMaxLength(50);

            entity.HasOne(d => d.ImportBatch).WithMany(p => p.ImportRecords)
                .HasForeignKey(d => d.ImportBatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportRecord_ImportBatch");

            entity.HasOne(d => d.ImportDocument).WithMany(p => p.ImportRecords)
                .HasForeignKey(d => d.ImportDocumentId)
                .HasConstraintName("FK_ImportRecord_ImportDocument");

            entity.HasOne(d => d.ParentImportRecord).WithMany(p => p.InverseParentImportRecord)
                .HasForeignKey(d => d.ParentImportRecordId)
                .HasConstraintName("FK_ImportRecord_Parent");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.ImportRecords)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK_ImportRecord_Admin");

            entity.HasOne(d => d.Source).WithMany(p => p.ImportRecords)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK_ImportRecord_DataSource");
        });

        modelBuilder.Entity<ImportSpecification>(entity =>
        {
            entity.ToTable("ImportSpecification");

            entity.HasIndex(e => e.SpecificationCode, "IX_ImportSpecification_Code");

            entity.Property(e => e.ImportSpecificationId).ValueGeneratedNever();
            entity.Property(e => e.SpecificationCode).HasMaxLength(200);

            entity.HasOne(d => d.Specification).WithMany(p => p.ImportSpecifications)
                .HasForeignKey(d => d.SpecificationId)
                .HasConstraintName("FK_ImportSpecification_Specification");
        });

        modelBuilder.Entity<ImportSpecificationVariant>(entity =>
        {
            entity.ToTable("ImportSpecificationVariant");

            entity.Property(e => e.Confidence).HasColumnType("decimal(5, 4)");
            entity.Property(e => e.NumericValue).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.SourceColumn).HasMaxLength(200);
            entity.Property(e => e.Unit).HasMaxLength(100);

            entity.HasOne(d => d.ImportSpecification).WithMany(p => p.ImportSpecificationVariants)
                .HasForeignKey(d => d.ImportSpecificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportSpecificationVariant_Specification");

            entity.HasOne(d => d.ImportVariant).WithMany(p => p.ImportSpecificationVariants)
                .HasForeignKey(d => d.ImportVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportSpecificationVariant_Variant");
        });

        modelBuilder.Entity<ImportTransmission>(entity =>
        {
            entity.ToTable("ImportTransmission");

            entity.HasIndex(e => new { e.ImportModelId, e.TransmissionRef }, "UX_ImportTransmission_Model_Ref").IsUnique();

            entity.Property(e => e.TransmissionRef)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TransmissionType).HasMaxLength(100);

            entity.HasOne(d => d.ImportModel).WithMany(p => p.ImportTransmissions)
                .HasForeignKey(d => d.ImportModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportTransmission_Model");
        });

        modelBuilder.Entity<ImportVariant>(entity =>
        {
            entity.ToTable("ImportVariant");

            entity.HasIndex(e => e.ImportDrivetrainId, "IX_ImportVariant_Drivetrain");

            entity.HasIndex(e => e.VariantName, "IX_ImportVariant_Name");

            entity.HasIndex(e => e.ImportVariantParentId, "IX_ImportVariant_Parent");

            entity.HasIndex(e => e.ImportPowertrainId, "IX_ImportVariant_Powertrain");

            entity.HasIndex(e => e.ImportTransmissionId, "IX_ImportVariant_Transmission");

            entity.Property(e => e.BaseVariantName).HasMaxLength(200);
            entity.Property(e => e.DrivetrainRef)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ExShowroomPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PowertrainRef)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.TransmissionRef)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.VariantName).HasMaxLength(200);
            entity.Property(e => e.VariantType)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.ImportDrivetrain).WithMany(p => p.ImportVariants)
                .HasForeignKey(d => d.ImportDrivetrainId)
                .HasConstraintName("FK_ImportVariant_Drivetrain");

            entity.HasOne(d => d.ImportPowertrain).WithMany(p => p.ImportVariants)
                .HasForeignKey(d => d.ImportPowertrainId)
                .HasConstraintName("FK_ImportVariant_Powertrain");

            entity.HasOne(d => d.ImportTransmission).WithMany(p => p.ImportVariants)
                .HasForeignKey(d => d.ImportTransmissionId)
                .HasConstraintName("FK_ImportVariant_Transmission");

            entity.HasOne(d => d.ImportVariantParent).WithMany(p => p.ImportVariants)
                .HasForeignKey(d => d.ImportVariantParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportVariant_Parent");
        });

        modelBuilder.Entity<ImportVariantParent>(entity =>
        {
            entity.ToTable("ImportVariantParent");

            entity.HasIndex(e => e.ImportModelId, "IX_ImportVariantParent_Model");

            entity.Property(e => e.ParentVariantName).HasMaxLength(200);

            entity.HasOne(d => d.ImportModel).WithMany(p => p.ImportVariantParents)
                .HasForeignKey(d => d.ImportModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportVariantParent_Model");
        });

        modelBuilder.Entity<ImportWarning>(entity =>
        {
            entity.ToTable("ImportWarning");

            entity.HasIndex(e => e.ImportBatchId, "IX_ImportWarning_Batch");

            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.ImportBatch).WithMany(p => p.ImportWarnings)
                .HasForeignKey(d => d.ImportBatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportWarning_Batch");

            entity.HasOne(d => d.ImportRecord).WithMany(p => p.ImportWarnings)
                .HasForeignKey(d => d.ImportRecordId)
                .HasConstraintName("FK_ImportWarning_Record");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.ImportWarnings)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK_ImportWarning_Admin");
        });

        modelBuilder.Entity<ImportWarranty>(entity =>
        {
            entity.ToTable("ImportWarranty");

            entity.Property(e => e.Confidence).HasColumnType("decimal(5, 4)");
            entity.Property(e => e.DurationYears).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.EvidenceText).HasMaxLength(1000);
            entity.Property(e => e.MaximumDurationYears).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.WarrantyType)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.ImportModel).WithMany(p => p.ImportWarranties)
                .HasForeignKey(d => d.ImportModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImportWarranty_Model");

            entity.HasOne(d => d.ImportVariant).WithMany(p => p.ImportWarranties)
                .HasForeignKey(d => d.ImportVariantId)
                .HasConstraintName("FK_ImportWarranty_Variant");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.ToTable("Model");

            entity.HasIndex(e => new { e.BrandId, e.ModelName }, "UQ_Model_Brand_ModelName").IsUnique();

            entity.Property(e => e.BodyType).HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Model_IsActive");
            entity.Property(e => e.ModelImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ModelName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Brand).WithMany(p => p.Models)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Model_Brand");
        });

        modelBuilder.Entity<ModelDimension>(entity =>
        {
            entity.HasIndex(e => e.ModelId, "UQ_ModelDimensions_Model").IsUnique();

            entity.Property(e => e.BootSpaceLitres).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.FuelTankCapacityLitres).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.GroundClearanceMm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("GroundClearanceMM");
            entity.Property(e => e.HeightMm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("HeightMM");
            entity.Property(e => e.LengthMm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("LengthMM");
            entity.Property(e => e.WheelbaseMm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("WheelbaseMM");
            entity.Property(e => e.WidthMm)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("WidthMM");

            entity.HasOne(d => d.Model).WithOne(p => p.ModelDimension)
                .HasForeignKey<ModelDimension>(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ModelDimensions_Model");
        });

        modelBuilder.Entity<ModelSafetyRating>(entity =>
        {
            entity.HasKey(e => e.SafetyRatingId).HasName("PK__ModelSaf__2C5CD088C135AFB8");

            entity.HasIndex(e => new { e.ModelId, e.Agency }, "UQ_ModelSafetyRatings_Model_Agency").IsUnique();

            entity.Property(e => e.AdultOccupantMaxScore).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.AdultOccupantScore).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.Agency)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ChildOccupantMaxScore).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.ChildOccupantScore).HasColumnType("decimal(6, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Rating).HasColumnType("decimal(2, 1)");
            entity.Property(e => e.SourceUrl)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TestedConfiguration)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Model).WithMany(p => p.ModelSafetyRatings)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ModelSafetyRatings_Models");
        });

        modelBuilder.Entity<MotorPerformance>(entity =>
        {
            entity.ToTable("MotorPerformance");

            entity.Property(e => e.MaxPower).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxPowerRpm).HasColumnName("MaxPowerRPM");
            entity.Property(e => e.MaxPowerRpmmax).HasColumnName("MaxPowerRPMMax");
            entity.Property(e => e.MaxPowerRpmmin).HasColumnName("MaxPowerRPMMin");
            entity.Property(e => e.MaxPowerUnit).HasMaxLength(20);
            entity.Property(e => e.MaxTorque).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaxTorqueRpm).HasColumnName("MaxTorqueRPM");
            entity.Property(e => e.MaxTorqueRpmmax).HasColumnName("MaxTorqueRPMMax");
            entity.Property(e => e.MaxTorqueRpmmin).HasColumnName("MaxTorqueRPMMin");
            entity.Property(e => e.MaxTorqueUnit).HasMaxLength(20);
            entity.Property(e => e.MotorName).HasMaxLength(100);

            entity.HasOne(d => d.Powertrain).WithMany(p => p.MotorPerformances)
                .HasForeignKey(d => d.PowertrainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MotorPerformance_Powertrain");
        });

        modelBuilder.Entity<Powertrain>(entity =>
        {
            entity.ToTable("Powertrain");

            entity.Property(e => e.BatteryCapacityKwh)
                .HasColumnType("decimal(8, 2)")
                .HasColumnName("BatteryCapacityKWh");
            entity.Property(e => e.CombinedMaxPower).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CombinedMaxPowerRpm).HasColumnName("CombinedMaxPowerRPM");
            entity.Property(e => e.CombinedMaxPowerRpmmax).HasColumnName("CombinedMaxPowerRPMMax");
            entity.Property(e => e.CombinedMaxPowerRpmmin).HasColumnName("CombinedMaxPowerRPMMin");
            entity.Property(e => e.CombinedMaxPowerUnit).HasMaxLength(20);
            entity.Property(e => e.CombinedMaxTorque).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CombinedMaxTorqueRpm).HasColumnName("CombinedMaxTorqueRPM");
            entity.Property(e => e.CombinedMaxTorqueRpmmax).HasColumnName("CombinedMaxTorqueRPMMax");
            entity.Property(e => e.CombinedMaxTorqueRpmmin).HasColumnName("CombinedMaxTorqueRPMMin");
            entity.Property(e => e.CombinedMaxTorqueUnit).HasMaxLength(20);
            entity.Property(e => e.PowertrainType)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId).HasName("PK__RefreshT__F5845E39BF4DBAFA");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.TokenHash).HasMaxLength(500);

            entity.HasOne(d => d.Admin).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshToken_Admin");
        });

        modelBuilder.Entity<Specification>(entity =>
        {
            entity.ToTable("Specification");

            entity.HasIndex(e => new { e.SpecificationCategoryId, e.SpecificationName }, "UQ_Specification_Category_Name").IsUnique();

            entity.HasIndex(e => e.SpecificationCode, "UX_Specification_SpecificationCode")
                .IsUnique()
                .HasFilter("([SpecificationCode] IS NOT NULL)");

            entity.Property(e => e.DataType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ExtractionGuidance).HasMaxLength(2000);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Specification_IsActive");
            entity.Property(e => e.SpecificationCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SpecificationName).HasMaxLength(150);
            entity.Property(e => e.Unit)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.SpecificationCategory).WithMany(p => p.Specifications)
                .HasForeignKey(d => d.SpecificationCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Specification_SpecificationCategory");
        });

        modelBuilder.Entity<SpecificationCategory>(entity =>
        {
            entity.ToTable("SpecificationCategory");

            entity.HasIndex(e => e.CategoryName, "UQ_SpecificationCategory_CategoryName").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_SpecificationCategory_IsActive");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateCode);

            entity.ToTable("State");

            entity.Property(e => e.StateCode).ValueGeneratedNever();
            entity.Property(e => e.StateName).HasMaxLength(150);
            entity.Property(e => e.StateOrUt)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("StateOrUT");
        });

        modelBuilder.Entity<SubDistrict>(entity =>
        {
            entity.HasKey(e => e.SubDistrictCode);

            entity.ToTable("SubDistrict");

            entity.HasIndex(e => e.DistrictCode, "IX_SubDistrict_DistrictCode");

            entity.Property(e => e.SubDistrictCode).ValueGeneratedNever();
            entity.Property(e => e.SubDistrictName).HasMaxLength(150);

            entity.HasOne(d => d.DistrictCodeNavigation).WithMany(p => p.SubDistricts)
                .HasForeignKey(d => d.DistrictCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubDistrict_District");
        });

        modelBuilder.Entity<Transmission>(entity =>
        {
            entity.ToTable("Transmission");

            entity.HasIndex(e => new { e.TransmissionType, e.NumberOfGears }, "UQ_Transmission_Type_Gears").IsUnique();

            entity.Property(e => e.TransmissionType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Variant>(entity =>
        {
            entity.ToTable("Variant");

            entity.HasIndex(e => new { e.ModelId, e.VariantName }, "UQ_Variant_Model_VariantName").IsUnique();

            entity.Property(e => e.ExShowroomPrice).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.VariantName)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.Drivetrain).WithMany(p => p.Variants)
                .HasForeignKey(d => d.DrivetrainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Variant_Drivetrain");

            entity.HasOne(d => d.Model).WithMany(p => p.Variants)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Variant_Model");

            entity.HasOne(d => d.Powertrain).WithMany(p => p.Variants)
                .HasForeignKey(d => d.PowertrainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Variant_Powertrain");

            entity.HasOne(d => d.Transmission).WithMany(p => p.Variants)
                .HasForeignKey(d => d.TransmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Variant_Transmission");
        });

        modelBuilder.Entity<VariantColour>(entity =>
        {
            entity.ToTable("VariantColour");

            entity.HasIndex(e => new { e.VariantId, e.ColourId }, "UQ_VariantColour_Variant_Colour").IsUnique();

            entity.Property(e => e.IsAvailable).HasDefaultValue(true, "DF_VariantColour_IsAvailable");

            entity.HasOne(d => d.Colour).WithMany(p => p.VariantColours)
                .HasForeignKey(d => d.ColourId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VariantColour_Colour");

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantColours)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VariantColour_Variant");
        });

        modelBuilder.Entity<VariantFeature>(entity =>
        {
            entity.ToTable("VariantFeature");

            entity.HasIndex(e => new { e.VariantId, e.FeatureId }, "UQ_VariantFeature_Variant_Feature").IsUnique();

            entity.Property(e => e.FeatureValue).HasMaxLength(200);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_VariantFeature_IsActive");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true, "DF_VariantFeature_IsAvailable");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.NumericValue).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.SourceType)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.SourceUrl)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Unit)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.Feature).WithMany(p => p.VariantFeatures)
                .HasForeignKey(d => d.FeatureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VariantFeature_Feature");

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantFeatures)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VariantFeature_Variant");
        });

        modelBuilder.Entity<VariantFuelEfficiency>(entity =>
        {
            entity.HasKey(e => e.VariantFuelEfficiencyId).HasName("PK__VariantF__A62A31552FA22B8D");

            entity.ToTable("VariantFuelEfficiency");

            entity.Property(e => e.FuelEfficiency).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.FuelEfficiencyUnit)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SourceType)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.SourceUrl)
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantFuelEfficiencies)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VariantFuelEfficiency_Variants");
        });

        modelBuilder.Entity<VariantImage>(entity =>
        {
            entity.ToTable("VariantImage");

            entity.Property(e => e.ImageType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantImages)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VariantImage_Variant");
        });

        modelBuilder.Entity<VariantLegacyBackup20260909>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Variant_LegacyBackup_20260909");

            entity.Property(e => e.ExShowroomPrice).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.VariantId).ValueGeneratedOnAdd();
            entity.Property(e => e.VariantName)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VariantSpecification>(entity =>
        {
            entity.ToTable("VariantSpecification");

            entity.HasIndex(e => new { e.VariantId, e.SpecificationId }, "UQ_VariantSpecification_Variant_Specification").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_VariantSpecification_IsActive");
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.NumericValue).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.SourceType)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.SourceUrl)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.TextValue).HasMaxLength(500);

            entity.HasOne(d => d.Source).WithMany(p => p.VariantSpecifications)
                .HasForeignKey(d => d.SourceId)
                .HasConstraintName("FK_VariantSpecification_DataSource");

            entity.HasOne(d => d.Specification).WithMany(p => p.VariantSpecifications)
                .HasForeignKey(d => d.SpecificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VariantSpecification_Specification");

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantSpecifications)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VariantSpecification_Variant");
        });

        modelBuilder.Entity<Warranty>(entity =>
        {
            entity.ToTable("Warranty");

            entity.Property(e => e.DurationYears).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MaximumDurationYears).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.WarrantyType)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Model).WithMany(p => p.Warranties)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Warranty_Model");

            entity.HasOne(d => d.Variant).WithMany(p => p.Warranties)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("FK_Warranty_Variant");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
