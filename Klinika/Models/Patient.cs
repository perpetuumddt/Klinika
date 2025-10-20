using System.ComponentModel.DataAnnotations;

namespace Klinika.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове")]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Прізвище обов'язкове")]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "По батькові")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Дата народження обов'язкова")]
        [Display(Name = "Дата народження")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Стать обов'язкова")]
        [Display(Name = "Стать")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Телефон обов'язковий")]
        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Невірний формат телефону")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string? Email { get; set; }

        [Display(Name = "Адреса")]
        public string? Address { get; set; }

        [Display(Name = "Поліс ОМС")]
        public string? InsuranceNumber { get; set; }

        [Display(Name = "Дата реєстрації")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        [Display(Name = "Повне ім'я")]
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}
