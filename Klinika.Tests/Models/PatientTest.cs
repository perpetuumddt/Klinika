using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;

namespace Klinika.Models
{
    public class PatientTests
    {
        [Fact]
        public void Patient_WithValidData_PassesValidation()
        {
            // Arrange
            var patient = new Patient
            {
                FirstName = "Іван",
                LastName = "Петренко",
                MiddleName = "Олександрович",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = "Чоловіча",
                PhoneNumber = "+380501234567",
                Email = "ivan@example.com",
                Address = "Київ, вул. Хрещатик, 1"
            };

            var validationContext = new ValidationContext(patient);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                patient, 
                validationContext, 
                validationResults, 
                true
            );

            // Assert
            isValid.Should().BeTrue();
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void Patient_WithoutFirstName_FailsValidation()
        {
            // Arrange
            var patient = new Patient
            {
                FirstName = "", // Порожнє обов'язкове поле
                LastName = "Петренко",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = "Чоловіча",
                PhoneNumber = "+380501234567"
            };

            var validationContext = new ValidationContext(patient);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                patient, 
                validationContext, 
                validationResults, 
                true
            );

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().Contain(v => 
                v.MemberNames.Contains("FirstName"));
        }

        [Theory]
        [InlineData("valid@example.com")]
        [InlineData("user.name@example.com")]
        [InlineData("user+tag@example.co.uk")]
        public void Patient_WithValidEmail_PassesValidation(string email)
        {
            // Arrange
            var patient = new Patient
            {
                FirstName = "Іван",
                LastName = "Петренко",
                DateOfBirth = new DateTime(1990, 5, 15),
                Gender = "Чоловіча",
                PhoneNumber = "+380501234567",
                Email = email
            };

            var validationContext = new ValidationContext(patient);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(
                patient, 
                validationContext, 
                validationResults, 
                true
            );

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void FullName_ReturnsCorrectFormat()
        {
            // Arrange
            var patient = new Patient
            {
                FirstName = "Іван",
                LastName = "Петренко",
                MiddleName = "Олександрович"
            };

            // Act
            var fullName = patient.FullName;

            // Assert
            fullName.Should().Be("Петренко Іван Олександрович");
        }

        [Fact]
        public void FullName_WithoutMiddleName_ReturnsCorrectFormat()
        {
            // Arrange
            var patient = new Patient
            {
                FirstName = "Іван",
                LastName = "Петренко",
                MiddleName = null
            };

            // Act
            var fullName = patient.FullName;

            // Assert
            fullName.Should().Be("Петренко Іван");
        }

        [Fact]
        public void Patient_DefaultRegistrationDate_IsSetToNow()
        {
            // Arrange & Act
            var patient = new Patient();

            // Assert
            patient.RegistrationDate.Should().BeCloseTo(
                DateTime.Now, 
                TimeSpan.FromSeconds(1)
            );
        }
    }
}