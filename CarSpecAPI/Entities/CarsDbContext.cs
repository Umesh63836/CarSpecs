using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CarSpecAPI.Entities;

public partial class CarsDbContext : DbContext
{
    private readonly IConfiguration configuration;

    public CarsDbContext(DbContextOptions<CarsDbContext> options, IConfiguration configuration)
        : base(options)
    {
        this.configuration = configuration;
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Drivetrain> Drivetrains { get; set; }

    public virtual DbSet<Engine> Engines { get; set; }

    public virtual DbSet<FuelType> FuelTypes { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Transmission> Transmissions { get; set; }

    public virtual DbSet<Variant> Variants { get; set; }

    public virtual DbSet<VariantImage> VariantImages { get; set; }

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

        modelBuilder.Entity<Drivetrain>(entity =>
        {
            entity.ToTable("Drivetrain");

            entity.HasIndex(e => e.DrivetrainType, "UQ_Drivetrain_DrivetrainType").IsUnique();

            entity.Property(e => e.DrivetrainType)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Engine>(entity =>
        {
            entity.ToTable("Engine");

            entity.Property(e => e.Displacement).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.EmissionStandard)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.EngineName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MaxPower).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.MaxTorque).HasColumnType("decimal(7, 2)");

            entity.HasOne(d => d.FuelType).WithMany(p => p.Engines)
                .HasForeignKey(d => d.FuelTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Engine_FuelType");
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

        modelBuilder.Entity<Model>(entity =>
        {
            entity.ToTable("Model");

            entity.HasIndex(e => new { e.BrandId, e.ModelName }, "UQ_Model_Brand_ModelName").IsUnique();

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

            entity.HasOne(d => d.Engine).WithMany(p => p.Variants)
                .HasForeignKey(d => d.EngineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Variant_Engine");

            entity.HasOne(d => d.Model).WithMany(p => p.Variants)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Variant_Model");

            entity.HasOne(d => d.Transmission).WithMany(p => p.Variants)
                .HasForeignKey(d => d.TransmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Variant_Transmission");
        });

        modelBuilder.Entity<VariantImage>(entity =>
        {
            entity.ToTable("VariantImage");

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantImages)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VariantImage_Variant");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
