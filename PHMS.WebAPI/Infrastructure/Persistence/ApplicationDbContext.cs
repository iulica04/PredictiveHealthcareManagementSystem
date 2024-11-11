using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<MedicalCondition> MedicalConditions { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Medic> Medics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("patients");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                entity.Property(p => p.FirstName).IsRequired().HasMaxLength(30);
                entity.Property(p => p.LastName).IsRequired().HasMaxLength(30);
                entity.Property(p => p.BirthDate).IsRequired();
                entity.Property(p => p.Gender).IsRequired().HasMaxLength(6);
                entity.Property(p => p.Email).IsRequired();
                entity.Property(p => p.PasswordHash).IsRequired();
                entity.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.Property(p => p.Address).IsRequired();

                entity.HasMany(p => p.PatientRecords)
                      .WithOne()
                      .HasForeignKey(p => p.PatientId);
            });

            modelBuilder.Entity<PatientRecord>(entity =>
            {
                entity.ToTable("pacient_records");
                entity.HasKey(pr => pr.PatientRecordId);
                entity.Property(pr => pr.PatientRecordId)
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()")
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
                      .HasColumnType("uuid")
                      .HasDefaultValueSql("uuid_generate_v4()")
                      .ValueGeneratedOnAdd();
                entity.Property(mc => mc.PatientId).HasColumnName("patient_id").IsRequired();
                entity.Property(mc => mc.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
                entity.Property(mc => mc.StartDate).HasColumnName("start_date").HasColumnType("date").IsRequired();
                entity.Property(mc => mc.EndDate).HasColumnName("end_date").HasColumnType("date").IsRequired(false); 
                entity.Property(mc => mc.CurrentStatus).HasColumnName("current_status").HasMaxLength(20).IsRequired();
                entity.Property(mc => mc.IsGenetic).HasColumnName("is_genetic").HasColumnType("boolean").IsRequired();
                entity.Property(mc => mc.Description).HasColumnName("description").HasMaxLength(500).IsRequired(false);
                entity.Property(mc => mc.Recommendation).HasColumnName("recommendations").HasMaxLength(500).IsRequired(false);

              //  entity.HasMany(mc => mc.Treatments)
               //       .WithOne()
                 //     .HasForeignKey("MedicalConditionId")
                 //     .OnDelete(DeleteBehavior.Cascade);
          //  });
        });

            modelBuilder.Entity<Treatment>(entity =>
            {
                entity.ToTable("treatments");
                entity.HasKey(t => t.TreatmentId);
                entity.Property(t => t.TreatmentId)
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

            modelBuilder.Entity<Medic>(entity =>
            {
                entity.ToTable("medics");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id)
                          .HasColumnType("uuid")
                          .HasDefaultValueSql("uuid_generate_v4()")
                          .ValueGeneratedOnAdd();
                entity.Property(m => m.Rank).IsRequired();
                entity.Property(m => m.Specialization).IsRequired();
                entity.Property(m => m.Hospital).IsRequired();
                entity.Property(p => p.FirstName).IsRequired().HasMaxLength(30);
                entity.Property(p => p.LastName).IsRequired().HasMaxLength(30);
                entity.Property(p => p.BirthDate).IsRequired();
                entity.Property(p => p.Gender).IsRequired().HasMaxLength(6);
                entity.Property(p => p.Email).IsRequired();
                entity.Property(p => p.PasswordHash).IsRequired();
                entity.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.Property(p => p.Address).IsRequired();
            });
        }
    }
}