using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Klinika.Models;
using Xunit;

namespace Klinika.Tests.Models
{
    public class DoctorTests
    {
        [Fact]
        public void Doctor_WithValidData_PassesValidation()
        {
            var doctor = new Doctor
            {
                FirstName = "Олена",
                LastName = "Коваль",
                MiddleName = "Петрівна",
                Specialization = "Терапевт",
                Qualification = "Вища категорія",
                ExperienceYears = 10,
                PhoneNumber = "+380501234567",
                Email = "olena@example.com",
                OfficeNumber = "101",
                WorkingHours = "9:00-18:00",
                IsActive = true
            };

            var ctx = new ValidationContext(doctor);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(doctor, ctx, results, validateAllProperties: true);

            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void Doctor_WithoutFirstName_FailsValidation()
        {
            var doctor = new Doctor
            {
                FirstName = "",
                LastName = "Коваль",
                Specialization = "Терапевт",
                Qualification = "Вища категорія",
                PhoneNumber = "+380501234567"
            };

            var ctx = new ValidationContext(doctor);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(doctor, ctx, results, true);

            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(Doctor.FirstName)));
        }

        [Fact]
        public void Doctor_WithoutLastName_FailsValidation()
        {
            var doctor = new Doctor
            {
                FirstName = "Олена",
                LastName = "",
                Specialization = "Терапевт",
                Qualification = "Вища категорія",
                PhoneNumber = "+380501234567"
            };

            var ctx = new ValidationContext(doctor);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(doctor, ctx, results, true);

            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(Doctor.LastName)));
        }

        [Fact]
        public void Doctor_WithoutSpecialization_FailsValidation()
        {
            var doctor = new Doctor
            {
                FirstName = "Олена",
                LastName = "Коваль",
                Specialization = "",
                Qualification = "Вища категорія",
                PhoneNumber = "+380501234567"
            };

            var ctx = new ValidationContext(doctor);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(doctor, ctx, results, true);

            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(Doctor.Specialization)));
        }

        [Fact]
        public void Doctor_WithTooBigExperience_FailsValidation()
        {
            var doctor = new Doctor
            {
                FirstName = "Олена",
                LastName = "Коваль",
                Specialization = "Терапевт",
                Qualification = "Вища категорія",
                ExperienceYears = 100,
                PhoneNumber = "+380501234567"
            };

            var ctx = new ValidationContext(doctor);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(doctor, ctx, results, true);

            isValid.Should().BeFalse();
            results.Should().Contain(r => r.MemberNames.Contains(nameof(Doctor.ExperienceYears)));
        }

        [Fact]
        public void FullName_ReturnsCorrectFormat_WithMiddleName()
        {
            var doctor = new Doctor
            {
                FirstName = "Олена",
                LastName = "Коваль",
                MiddleName = "Петрівна",
                Specialization = "Терапевт",
                Qualification = "Вища категорія",
                PhoneNumber = "+380501234567"
            };

            doctor.FullName.Should().Be("Коваль Олена Петрівна");
        }

        [Fact]
        public void FullName_ReturnsCorrectFormat_WithoutMiddleName()
        {
            var doctor = new Doctor
            {
                FirstName = "Олена",
                LastName = "Коваль",
                MiddleName = null,
                Specialization = "Терапевт",
                Qualification = "Вища категорія",
                PhoneNumber = "+380501234567"
            };

            doctor.FullName.Should().Be("Коваль Олена");
        }

        [Fact]
        public void FullNameWithTitle_ReturnsCorrectFormat()
        {
            var doctor = new Doctor
            {
                FirstName = "Олена",
                LastName = "Коваль",
                MiddleName = "Петрівна",
                Specialization = "Терапевт",
                Qualification = "Вища категорія",
                PhoneNumber = "+380501234567"
            };

            doctor.FullNameWithTitle.Should().Be("Коваль Олена Петрівна, Терапевт");
        }

        [Fact]
        public void FullNameWithTitle_WithoutMiddleName_ReturnsCorrectFormat()
        {
            var doctor = new Doctor
            {
                FirstName = "Олена",
                LastName = "Коваль",
                Specialization = "Терапевт",
                Qualification = "Вища категорія",
                PhoneNumber = "+380501234567"
            };

            doctor.FullNameWithTitle.Should().Be("Коваль Олена, Терапевт");
        }
    }
}
