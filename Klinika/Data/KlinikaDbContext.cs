using Microsoft.EntityFrameworkCore;
using Klinika.Models;
using Klinika.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Klinika.Data
{
    public class KlinikaDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public KlinikaDbContext(DbContextOptions<KlinikaDbContext> options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Зв'язки
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(mr => mr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Doctor)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(mr => mr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            //Indexes
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.Email)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Email)
                .IsUnique();

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.DoctorId, a.AppointmentDateTime })
                .IsUnique();

            //Properties
            modelBuilder.Entity<Patient>()
                .Property(p => p.RegistrationDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Doctor>()
                .Property(d => d.HireDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Appointment>()
                .Property(a => a.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<MedicalRecord>()
                .Property(mr => mr.RecordDate)
                .HasDefaultValueSql("GETDATE()");

            // Identity configuration
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                // Email configuration
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                // PhoneNumber configuration
                entity.Property(u => u.PhoneNumberUA)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(u => u.PhoneNumberUA)
                    .IsUnique();

                // FullName configuration
                entity.Property(u => u.FullName)
                    .IsRequired()
                    .HasMaxLength(500);

                // CreatedDate default value
                entity.Property(u => u.CreatedDate)
                    .HasDefaultValueSql("GETDATE()");

                // IsActive default value
                entity.Property(u => u.IsActive)
                    .HasDefaultValue(true);
            });
        }
    }
}

