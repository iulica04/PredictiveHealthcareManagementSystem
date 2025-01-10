using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Identity.Persistence
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasDiscriminator<UserType>("Type")
                .HasValue<Admin>(UserType.Admin)
                .HasValue<Medic>(UserType.Medic)
                .HasValue<Patient>(UserType.Patient);
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id)
                      .ValueGeneratedOnAdd();

                entity.Property(u => u.Type)
                      .IsRequired();
                entity.Property(p => p.FirstName)
                      .IsRequired()
                      .HasMaxLength(30);
                entity.Property(p => p.LastName)
                      .IsRequired()
                      .HasMaxLength(30);
                entity.Property(p => p.BirthDate)
                      .IsRequired();
                entity.Property(p => p.Gender)
                      .IsRequired()
                      .HasMaxLength(6);
                entity.Property(p => p.Email)
                      .IsRequired();
                entity.Property(p => p.PasswordHash)
                      .IsRequired();
                entity.Property(p => p.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(15);
                entity.Property(p => p.Address)
                      .IsRequired();
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasMany(p => p.PatientRecords)
                      .WithOne()
                      .HasForeignKey(p => p.PatientId);
            });

            modelBuilder.Entity<Medic>(entity =>
            {
                entity.Property(m => m.Rank)
                      .IsRequired()
                      .HasMaxLength(30);
                entity.Property(m => m.Specialization)
                      .IsRequired()
                      .HasMaxLength(30);
                entity.Property(m => m.Hospital)
                      .IsRequired()
                      .HasMaxLength(30);
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword("parola123");
                string hashedPassword2 = BCrypt.Net.BCrypt.HashPassword("parola456");
                var birthDate = new DateTime(2004, 2, 15, 0, 0, 0, DateTimeKind.Utc);
                var birthDate2 = new DateTime(2003, 7, 20, 0, 0, 0, DateTimeKind.Utc);
                entity.HasData(
                    new Admin
                    {
                        Id = Guid.NewGuid(),
                        Type = UserType.Admin,
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
                        Type = UserType.Admin,
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
