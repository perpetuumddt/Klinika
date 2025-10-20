using Microsoft.EntityFrameworkCore;
using Klinika.Data;
using Klinika.Models;

namespace Klinika.Tests.Data
{
    public static class TestDbContextFactory
    {
        public static KlinikaDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<KlinikaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new KlinikaDbContext(options);
            
            SeedTestData(context);
            
            return context;
        }
        
        private static void SeedTestData(KlinikaDbContext context)
        {
            var patients = new List<Patient>
            {
                new Patient
                {
                    Id = 1,
                    FirstName = "Іван",
                    LastName = "Петренко",
                    MiddleName = "Олександрович",
                    DateOfBirth = new DateTime(1990, 5, 15),
                    Gender = "Чоловіча",
                    PhoneNumber = "+380501234567",
                    Email = "ivan.petrenko@example.com",
                    Address = "м. Київ, вул. Хрещатик, 1",
                    InsuranceNumber = "123456789",
                    RegistrationDate = DateTime.Now.AddYears(-2)
                },
                new Patient
                {
                    Id = 2,
                    FirstName = "Марія",
                    LastName = "Коваленко",
                    MiddleName = "Іванівна",
                    DateOfBirth = new DateTime(1985, 8, 20),
                    Gender = "Жіноча",
                    PhoneNumber = "+380509876543",
                    Email = "maria.kovalenko@example.com",
                    Address = "м. Київ, вул. Шевченка, 10",
                    InsuranceNumber = "987654321",
                    RegistrationDate = DateTime.Now.AddYears(-1)
                }
            };
            
            var doctors = new List<Doctor>
            {
                new Doctor
                {
                    Id = 1,
                    FirstName = "Олена",
                    LastName = "Коваль",
                    MiddleName = "Петрівна",
                    Specialization = "Терапевт",
                    Qualification = "Вища категорія",
                    ExperienceYears = 15,
                    PhoneNumber = "+380671111111",
                    Email = "olena.koval@clinic.com",
                    OfficeNumber = "101",
                    WorkingHours = "9:00-18:00",
                    HireDate = DateTime.Now.AddYears(-10),
                    IsActive = true
                },
                new Doctor
                {
                    Id = 2,
                    FirstName = "Андрій",
                    LastName = "Сидоренко",
                    MiddleName = "Васильович",
                    Specialization = "Хірург",
                    Qualification = "Перша категорія",
                    ExperienceYears = 8,
                    PhoneNumber = "+380672222222",
                    Email = "andriy.sydorenko@clinic.com",
                    OfficeNumber = "205",
                    WorkingHours = "10:00-19:00",
                    HireDate = DateTime.Now.AddYears(-5),
                    IsActive = true
                }
            };

            context.Patients.AddRange(patients);
            context.Doctors.AddRange(doctors);
            context.SaveChanges();
        }
        
        public static KlinikaDbContext CreateContextWithData(
            IEnumerable<Patient>? patients = null,
            IEnumerable<Doctor>? doctors = null,
            IEnumerable<Appointment>? appointments = null,
            IEnumerable<MedicalRecord>? medicalRecords = null)
        {
            var options = new DbContextOptionsBuilder<KlinikaDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new KlinikaDbContext(options);

            if (patients != null)
                context.Patients.AddRange(patients);

            if (doctors != null)
                context.Doctors.AddRange(doctors);

            if (appointments != null)
                context.Appointments.AddRange(appointments);

            if (medicalRecords != null)
                context.MedicalRecords.AddRange(medicalRecords);

            context.SaveChanges();
            return context;
        }
    }
}