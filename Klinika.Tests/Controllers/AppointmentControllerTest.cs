using FluentAssertions;
using Klinika.Controllers;
using Klinika.Data;
using Klinika.Models;
using Klinika.Tests.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Klinika.Tests.Controllers
{
    public class AppointmentControllerTests : IDisposable
    {
        private readonly KlinikaDbContext _context;
        private readonly AppointmentsController _controller;

        public AppointmentControllerTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _controller = new AppointmentsController(_context);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithList()
        {
            var result = await _controller.Index();

            result.Should().BeOfType<ViewResult>();
            var viewResult = (ViewResult)result;
            var model = viewResult.Model as IEnumerable<Appointment>;
            model.Should().NotBeNull();
            model.Should().HaveCountGreaterThan(0);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsView()
        {
            var appointment = await _context.Appointments.FirstAsync();

            var result = await _controller.Details(appointment.Id);

            result.Should().BeOfType<ViewResult>();
            var viewResult = (ViewResult)result;
            var model = viewResult.Model as Appointment;
            model.Should().NotBeNull();
            model!.Id.Should().Be(appointment.Id);
        }

        [Fact]
        public async Task Create_Post_ValidModel_Redirects()
        {
            var patient = await _context.Patients.FirstAsync();
            var doctor = await _context.Doctors.FirstAsync();

            var newAppointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                AppointmentDateTime = DateTime.Now.AddDays(2),
                DurationMinutes = 30,
                Status = AppointmentStatus.Scheduled
            };

            var result = await _controller.Create(newAppointment);

            result.Should().BeOfType<RedirectToActionResult>();

            var saved = await _context.Appointments
                .FirstOrDefaultAsync(a => a.PatientId == patient.Id &&
                                          a.DoctorId == doctor.Id &&
                                          a.DurationMinutes == 30);
            saved.Should().NotBeNull();
        }

        [Fact]
        public async Task Edit_Post_ValidData_Updates()
        {
            var appointment = await _context.Appointments.FirstAsync();
            appointment.DurationMinutes = 45;

            var result = await _controller.Edit(appointment.Id, appointment);

            result.Should().BeOfType<RedirectToActionResult>();

            var updated = await _context.Appointments.FindAsync(appointment.Id);
            updated!.DurationMinutes.Should().Be(45);
        }

        [Fact]
        public async Task DeleteConfirmed_Removes()
        {
            var appointment = await _context.Appointments.FirstAsync();
            var id = appointment.Id;

            var result = await _controller.DeleteConfirmed(id);

            result.Should().BeOfType<RedirectToActionResult>();

            var deleted = await _context.Appointments.FindAsync(id);
            deleted.Should().BeNull();
        }
    }
}
