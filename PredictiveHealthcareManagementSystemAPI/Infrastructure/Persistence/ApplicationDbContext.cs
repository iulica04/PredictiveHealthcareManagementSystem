using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Pacient> Pacients { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<MedicalCondition> MedicalConditions { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        //public DbSet<Medic> Medics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            // Configurarea entității Pacient
            modelBuilder.Entity<Pacient>(entity =>
            {
                entity.ToTable("pacients");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                entity.Property(p => p.Role).IsRequired().HasMaxLength(13);
                entity.Property(p => p.FirstName).IsRequired().HasMaxLength(200);
                entity.Property(p => p.LastName).IsRequired().HasMaxLength(200);
                entity.Property(p => p.BirthDate).IsRequired();
                entity.Property(p => p.Gender).IsRequired().HasMaxLength(1);
                entity.Property(p => p.Email).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Phone).IsRequired().HasMaxLength(10);
                entity.Property(p => p.Address).IsRequired().HasMaxLength(300);

                entity.HasOne(p => p.PacientRecord)
                      .WithOne(pr => pr.Pacient)
                      .HasForeignKey<PacientRecord>(pr => pr.PatientId);
            });

            // Configurarea entității PacientRecord
            modelBuilder.Entity<PacientRecord>(entity =>
            {
                entity.ToTable("pacient_records");
                entity.HasKey(pr => pr.Id);
                entity.Property(pr => pr.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                entity.HasMany(pr => pr.MedicalConditions)
                      .WithOne(mc => mc.Pacient)
                      .HasForeignKey(mc => mc.PacientId);
                entity.HasMany(pr => pr.Treatments)
                      .WithOne(t => t.Pacient)
                      .HasForeignKey(t => t.PacientId);
                entity.HasMany(pr => pr.Consultations)
                      .WithOne(c => c.Pacient)
                      .HasForeignKey(c => c.PacientId);
            });



            // Configurarea entităților MedicalCondition, Treatment și Consultation
            modelBuilder.Entity<MedicalCondition>(entity =>
            {
                entity.ToTable("medical_conditions");
                entity.HasKey(mc => mc.Id);
                entity.Property(mc => mc.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Treatment>(entity =>
            {
                entity.ToTable("treatments");
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Consultation>(entity =>
            {
                entity.ToTable("consultations");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
            });
        }
    }
}