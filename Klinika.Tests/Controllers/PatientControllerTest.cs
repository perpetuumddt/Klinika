using FluentAssertions;
using Klinika.Data;
using Klinika.Models;
using Klinika.Tests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Klinika.Controllers
{
    public class PatientsControllerTests : IDisposable
    {
        private readonly KlinikaDbContext _context;
        private readonly PatientsController _controller;

        public PatientsControllerTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _controller = new PatientsController(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
            _controller?.Dispose();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfPatients()
        {
            // Act
            var result = await _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as IEnumerable<Patient>;
            model.Should().NotBeNull();
            model.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewResult()
        {
            // Arrange
            var patient = await _context.Patients.FirstAsync();

            // Act
            var result = await _controller.Details(patient.Id);

            // Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result as ViewResult;
            var model = viewResult.Model as Patient;
            model.Should().NotBeNull();
            model.Id.Should().Be(patient.Id);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Details(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
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
        public async Task Create_ValidPatient_AddsToDatabase()
        {
            // Arrange
            var patient = new Patient
            {
                FirstName = "Тест",
                LastName = "Тестовий",
                DateOfBirth = new DateTime(1995, 1, 1),
                Gender = "Чоловіча",
                PhoneNumber = "+380681234567",
                Email = "test@example.com"
            };

            // Act
            var result = await _controller.Create(patient);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            
            var savedPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PhoneNumber == patient.PhoneNumber);
            savedPatient.Should().NotBeNull();
            savedPatient.FirstName.Should().Be("Тест");
        }

        [Fact]
        public async Task Edit_ValidData_UpdatesPatient()
        {
            // Arrange
            var patient = await _context.Patients.FirstAsync();
            patient.FirstName = "Оновлене Ім'я";

            // Act
            var result = await _controller.Edit(patient.Id, patient);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            
            var updated = await _context.Patients.FindAsync(patient.Id);
            updated.FirstName.Should().Be("Оновлене Ім'я");
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesPatient()
        {
            // Arrange
            var patient = new Patient
            {
                FirstName = "Тест",
                LastName = "Видалення",
                DateOfBirth = DateTime.Now.AddYears(-30),
                Gender = "Чоловіча",
                PhoneNumber = "+380997777777"
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            var patientId = patient.Id;

            // Act
            var result = await _controller.DeleteConfirmed(patientId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>();
            
            var deleted = await _context.Patients.FindAsync(patientId);
            deleted.Should().BeNull();
        }
        
    }
}