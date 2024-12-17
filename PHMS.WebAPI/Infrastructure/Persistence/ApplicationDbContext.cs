using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<MedicalCondition> MedicalConditions { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<PatientRecord> PatientRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("password_reset_tokens");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd();
                entity.Property(prt => prt.Email);
                entity.Property(prt => prt.Token).IsRequired();
                entity.Property(prt => prt.ExpirationDate).IsRequired();
            });

            modelBuilder.Entity<PatientRecord>(entity =>
            {
                entity.ToTable("patient_records");
                entity.HasKey(pr => pr.PatientRecordId);
                entity.Property(pr => pr.PatientRecordId)
                      .ValueGeneratedOnAdd();

                entity.HasOne<Patient>()
                      .WithMany(p => p.PatientRecords)
                      .HasForeignKey(pr => pr.PatientId);
            });

            modelBuilder.Entity<MedicalCondition>(entity =>
            {
                entity.ToTable("medical_conditions");
                entity.HasKey(mc => mc.MedicalConditionId);
                entity.Property(mc => mc.MedicalConditionId)
                      .ValueGeneratedOnAdd();
                entity.Property(mc => mc.PatientId).HasColumnName("patient_id").IsRequired();
                entity.Property(mc => mc.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                entity.Property(mc => mc.StartDate).HasColumnName("start_date").IsRequired();
                entity.Property(mc => mc.EndDate).HasColumnName("end_date").IsRequired(false);
                entity.Property(mc => mc.CurrentStatus).HasColumnName("current_status").HasMaxLength(20).IsRequired();
                entity.Property(mc => mc.IsGenetic).HasColumnName("is_genetic").IsRequired();
                entity.Property(mc => mc.Description).HasColumnName("description").HasMaxLength(500).IsRequired(false);
                entity.Property(mc => mc.Recommendation).HasColumnName("recommendations").HasMaxLength(500).IsRequired(false);
            });

            modelBuilder.Entity<Consultation>(entity =>
            {
                entity.ToTable("consultations");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id)
                      .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Medication>(entity =>
            {
                entity.ToTable("medications");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id)
                      .ValueGeneratedOnAdd();
                entity.Property(m => m.Name).IsRequired();
                entity.Property(m => m.Type)
                      .HasConversion<string>()
                      .IsRequired();
                entity.Property(m => m.Ingredients).IsRequired();
                entity.Property(m => m.AdverseEffects).IsRequired();
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.ToTable("prescriptions");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd();
                entity.Property(p => p.DateIssued).IsRequired();
                entity.HasMany(p => p.Medications).WithOne();
            });

            modelBuilder.Entity<Treatment>(entity =>
            {
                entity.ToTable("treatments");
                entity.HasKey(t => t.TreatmentId);
                entity.Property(t => t.TreatmentId)
                      .ValueGeneratedOnAdd();
                entity.Property(t => t.Type)
                      .HasConversion<string>()
                      .IsRequired();
                entity.HasOne(t => t.Prescription).WithMany();
                entity.Property(t => t.Location).IsRequired();
                entity.Property(t => t.StartDate).IsRequired();
                entity.Property(t => t.Duration).IsRequired();
                entity.Property(t => t.Frequency).IsRequired();
            });
        }
    }
}

