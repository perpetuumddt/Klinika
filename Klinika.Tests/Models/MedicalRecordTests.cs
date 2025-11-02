using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Klinika.Models;
using Xunit;

namespace Klinika.Tests.Models
{
    public class MedicalRecordTests
    {
        [Fact]
        public void MedicalRecord_WithValidData_PassesValidation()
        {
            // Arrange
            var record = new MedicalRecord
            {
                PatientId = 1,
                DoctorId = 1,
                RecordDate = DateTime.Now,
                Diagnosis = "Тестовий діагноз",
                Symptoms = "Головний біль, температура",
                Treatment = "Парацетамол",
                Prescriptions = "Пити 2 рази на день",
                Recommendations = "Відпочинок",
                Notes = "Пацієнт виглядає добре"
            };

            var validationContext = new ValidationContext(record);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeTrue();
            validationResults.Should().BeEmpty();
        }

        [Fact]
        public void MedicalRecord_WithoutPatient_FailsValidation()
        {
            // Arrange
            var record = new MedicalRecord
            {
                DoctorId = 1,
                RecordDate = DateTime.Now
            };

            var validationContext = new ValidationContext(record);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().Contain(v => v.MemberNames.Contains("PatientId"));
        }

        [Fact]
        public void MedicalRecord_WithoutDoctor_FailsValidation()
        {
            // Arrange
            var record = new MedicalRecord
            {
                PatientId = 1,
                RecordDate = DateTime.Now
            };

            var validationContext = new ValidationContext(record);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().Contain(v => v.MemberNames.Contains("DoctorId"));
        }

        [Fact]
        public void MedicalRecord_WithoutRecordDate_FailsValidation()
        {
            // Arrange
            var record = new MedicalRecord
            {
                PatientId = 1,
                DoctorId = 1,
                RecordDate = null
            };

            var validationContext = new ValidationContext(record);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().Contain(v => v.MemberNames.Contains("RecordDate"));
        }

        [Fact]
        public void MedicalRecord_Diagnosis_LongerThan1000_FailsValidation()
        {
            // Arrange
            var record = new MedicalRecord
            {
                PatientId = 1,
                DoctorId = 1,
                RecordDate = DateTime.Now,
                Diagnosis = new string('A', 1001)
            };

            var validationContext = new ValidationContext(record);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().Contain(v => v.MemberNames.Contains("Diagnosis"));
        }

        [Fact]
        public void MedicalRecord_Can_Have_Null_NextVisitDate()
        {
            // Arrange
            var record = new MedicalRecord
            {
                PatientId = 1,
                DoctorId = 1,
                RecordDate = DateTime.Now,
                NextVisitDate = null
            };

            var validationContext = new ValidationContext(record);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeTrue();
        }
    }
}
