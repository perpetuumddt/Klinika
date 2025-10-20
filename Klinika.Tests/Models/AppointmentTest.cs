using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Klinika.Models;

namespace Klinika.Tests.Models
{
    public class AppointmentTests
    {
        [Fact]
        public void Appointment_WithValidData_PassesValidation()
        {
            // Arrange
            var appointment = new Appointment
            {
                Id = 1,
                PatientId = 1,
                DoctorId = 1,
                AppointmentDateTime = DateTime.Now.AddDays(1),
                DurationMinutes = 30,
                AppointmentType = "Консультація",
                Status = AppointmentStatus.Scheduled,
                CreatedDate = DateTime.Now
            };

            var validationContext = new ValidationContext(appointment);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                appointment, 
                validationContext, 
                validationResults, 
                true
            );

            // Assert
            isValid.Should().BeTrue();
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void Appointment_WithoutPatientId_FailsValidation()
        {
            // Arrange
            var appointment = new Appointment
            {
                PatientId = 0, // Невалідне значення
                DoctorId = 1,
                AppointmentDateTime = DateTime.Now.AddDays(1),
                DurationMinutes = 30
            };

            var validationContext = new ValidationContext(appointment);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                appointment, 
                validationContext, 
                validationResults, 
                true
            );

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().Contain(v => 
                v.MemberNames.Contains("PatientId"));
        }

        [Fact]
        public void Appointment_WithoutDoctorId_FailsValidation()
        {
            // Arrange
            var appointment = new Appointment
            {
                PatientId = 1,
                DoctorId = 0, // Невалідне значення
                AppointmentDateTime = DateTime.Now.AddDays(1),
                DurationMinutes = 30
            };

            var validationContext = new ValidationContext(appointment);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                appointment, 
                validationContext, 
                validationResults, 
                true
            );

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void Complaints_ExceedingMaxLength_FailsValidation()
        {
            // Arrange
            var appointment = new Appointment
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentDateTime = DateTime.Now.AddDays(1),
                DurationMinutes = 30,
                Complaints = new string('A', 1001) // Більше 1000 символів
            };

            var validationContext = new ValidationContext(appointment);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                appointment, 
                validationContext, 
                validationResults, 
                true
            );

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().Contain(v => 
                v.MemberNames.Contains("Complaints"));
        }

        [Theory]
        [InlineData(AppointmentStatus.Scheduled)]
        [InlineData(AppointmentStatus.Confirmed)]
        [InlineData(AppointmentStatus.Completed)]
        [InlineData(AppointmentStatus.Cancelled)]
        [InlineData(AppointmentStatus.NotVisited)]
        public void Status_AllEnumValues_AreValid(AppointmentStatus status)
        {
            // Arrange & Act
            var appointment = new Appointment
            {
                Status = status
            };

            // Assert
            appointment.Status.Should().Be(status);
            Enum.IsDefined(typeof(AppointmentStatus), status).Should().BeTrue();
        }

        [Fact]
        public void Appointment_DefaultValues_AreSetCorrectly()
        {
            // Arrange & Act
            var appointment = new Appointment();

            // Assert
            appointment.Status.Should().Be(AppointmentStatus.Scheduled);
            appointment.DurationMinutes.Should().Be(30);
            appointment.AppointmentDateTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
    }
}