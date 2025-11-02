using FluentAssertions;
using Klinika.Data;
using Klinika.Models;
using Klinika.Tests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Klinika.Controllers
{
    public class DoctorsControllerTests : IDisposable
    {
        private readonly KlinikaDbContext _context;
        private readonly DoctorsController _controller;

        public DoctorsControllerTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _controller = new DoctorsController(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _controller?.Dispose();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfDoctors()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as IEnumerable<Doctor>;
            model.Should().NotBeNull();
            model.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewResult()
        {
            // Arrange
            var doctor = await _context.Doctors.FirstAsync();

            // Act
            var result = await _controller.Details(doctor.Id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as Doctor;
            model.Should().NotBeNull();
            model.Id.Should().Be(doctor.Id);
        }

        [Fact]
        public async Task Create_ValidDoctor_AddsToDatabase()
        {
            // Arrange
            var doctor = new Doctor
            {
                FirstName = "Петро",
                LastName = "Іваненко",
                Specialization = "Кардіолог",
                Qualification = "Вища категорія",
                ExperienceYears = 10,
                PhoneNumber = "+380501111111",
                Email = "petro@clinic.com",
                IsActive = true
            };

            // Act
            var result = await _controller.Create(doctor);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            
            var savedDoctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Email == doctor.Email);
            savedDoctor.Should().NotBeNull();
            savedDoctor.FirstName.Should().Be("Петро");
        }

        [Fact]
        public async Task Edit_ValidData_UpdatesDoctor()
        {
            // Arrange
            var doctor = await _context.Doctors.FirstAsync();
            doctor.Specialization = "Оновлена спеціалізація";

            // Act
            var result = await _controller.Edit(doctor.Id, doctor);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            
            var updated = await _context.Doctors.FindAsync(doctor.Id);
            updated?.Specialization.Should().Be("Оновлена спеціалізація");
        }

        [Fact]
        public async Task Delete_Get_WithValidId_ReturnsViewResult()
        {
            // Arrange
            var doctor = await _context.Doctors.FirstAsync();

            // Act
            var result = await _controller.Delete(doctor.Id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Doctor;
            model?.Id.Should().Be(doctor.Id);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesDoctor()
        {
            // Arrange
            var doctor = new Doctor
            {
                FirstName = "Для",
                LastName = "Видалення",
                Specialization = "Тест",
                Qualification = "Тест",
                PhoneNumber = "+380509999999",
                Email = "delete@test.com",
                IsActive = true
            };
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            var doctorId = doctor.Id;

            // Act
            var result = await _controller.DeleteConfirmed(doctorId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            
            var deleted = await _context.Doctors.FindAsync(doctorId);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task Details_LoadsRelatedAppointments()
        {
            // Arrange
            var doctor = await _context.Doctors.FirstAsync();
            var patient = await _context.Patients.FirstAsync();
            
            var appointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                AppointmentDateTime = DateTime.Now.AddDays(1),
                DurationMinutes = 30,
                Status = AppointmentStatus.Scheduled
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Details(doctor.Id);

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Doctor;
            model?.Appointments.Should().NotBeNull();
            model?.Appointments.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ReturnsViewResult()
        {
            // Arrange
            var doctor = await _context.Doctors.FirstAsync();

            // Act
            var result = await _controller.Edit(doctor.Id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as Doctor;
            model?.Id.Should().Be(doctor.Id);
        }

        [Fact]
        public async Task Edit_Get_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}