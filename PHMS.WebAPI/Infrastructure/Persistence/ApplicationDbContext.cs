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
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<MedicalCondition> MedicalConditions { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Medic> Medics { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<PatientRecord> PatientRecords { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Medication> Medications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            


            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("patients");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
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

            modelBuilder.Entity<Medic>(entity =>
            {
                entity.ToTable("medics");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Id)
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

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("admins");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id)
                      .ValueGeneratedOnAdd();
                entity.Property(a => a.FirstName).IsRequired().HasMaxLength(30);
                entity.Property(a => a.LastName).IsRequired().HasMaxLength(30);
                entity.Property(a => a.BirthDate).IsRequired();
                entity.Property(a => a.Gender).IsRequired().HasMaxLength(6);
                entity.Property(a => a.Email).IsRequired();
                entity.Property(a => a.PasswordHash).IsRequired();
                entity.Property(a => a.PhoneNumber).IsRequired().HasMaxLength(15);
                entity.Property(a => a.Address).IsRequired();

                // Seeder pentru un administrator predefinit
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword("parola123");
                string hashedPassword2 = BCrypt.Net.BCrypt.HashPassword("parola456");
                var birthDate = new DateTime(2004, 2, 15, 0, 0, 0, DateTimeKind.Utc);
                var birthDate2 = new DateTime(2003, 7, 20, 0, 0, 0, DateTimeKind.Utc);
                entity.HasData(
                    new Admin
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Admin1",
                        LastName = "User",
                        BirthDate = birthDate,
                        Gender = "Female",
                        Email = "admin1@gmail.com",
                        PasswordHash = hashedPassword,
                        PhoneNumber = "0757732675",
                        Address = "Piata Unirii nr.3, Iasi"
                    },
                    new Admin
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Admin2",
                        LastName = "User",
                        BirthDate = birthDate2,
                        Gender = "Male",
                        Email = "admin2@gmail.com",
                        PasswordHash = hashedPassword2,
                        PhoneNumber = "0751234567",
                        Address = "Strada Libertatii nr.10, Iasi"
                    }
                );
            });
        }
    }
}

