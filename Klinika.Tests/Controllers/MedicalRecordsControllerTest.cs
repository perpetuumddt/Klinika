using FluentAssertions;
using Klinika.Data;
using Klinika.Models;
using Klinika.Tests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Klinika.Controllers
{
    public class MedicalRecordsControllerTests : IDisposable
    {
        private readonly KlinikaDbContext _context;
        private readonly MedicalRecordsController _controller;

        public MedicalRecordsControllerTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _controller = new MedicalRecordsController(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsView()
        {
            var record = await _context.MedicalRecords.FirstAsync();

            var result = await _controller.Details(record.Id);

            result.Should().BeOfType<ViewResult>();
            var viewResult = (ViewResult)result;
            var model = viewResult.Model as MedicalRecord;
            model.Should().NotBeNull();
            model!.Id.Should().Be(record.Id);
        }

        [Fact]
        public async Task Create_ValidModel_AddsToDb()
        {
            var patient = await _context.Patients.FirstAsync();
            var doctor = await _context.Doctors.FirstAsync();

            var newRecord = new MedicalRecord
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                Diagnosis = "Тестовий діагноз",
                Treatment = "Тестове лікування",
                RecordDate = DateTime.Today
            };

            var result = await _controller.Create(newRecord);

            result.Should().BeOfType<RedirectToActionResult>();

            var saved = await _context.MedicalRecords
                .FirstOrDefaultAsync(r => r.PatientId == patient.Id && r.Diagnosis == "Тестовий діагноз");
            saved.Should().NotBeNull();
        }

        [Fact]
        public async Task Edit_Post_Valid_Updates()
        {
            var record = await _context.MedicalRecords.FirstAsync();
            record.Diagnosis = "Оновлений";

            var result = await _controller.Edit(record.Id, record);

            result.Should().BeOfType<RedirectToActionResult>();

            var updated = await _context.MedicalRecords.FindAsync(record.Id);
            updated!.Diagnosis.Should().Be("Оновлений");
        }

        [Fact]
        public async Task DeleteConfirmed_Removes()
        {
            var record = await _context.MedicalRecords.FirstAsync();
            var id = record.Id;

            var result = await _controller.DeleteConfirmed(id);

            result.Should().BeOfType<RedirectToActionResult>();

            var deleted = await _context.MedicalRecords.FindAsync(id);
            deleted.Should().BeNull();
        }
    }
}
