using System.ComponentModel.DataAnnotations;

namespace Klinika.Models
{
    public class Doctor
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

        [Required(ErrorMessage = "Спеціалізація обов'язкова")]
        [Display(Name = "Спеціалізація")]
        public string Specialization { get; set; } = string.Empty;

        [Required(ErrorMessage = "Кваліфікація обов'язкова")]
        [Display(Name = "Кваліфікація")]
        public string Qualification { get; set; } = string.Empty;

        [Display(Name = "Досвід роботи (років)")]
        [Range(0, 50, ErrorMessage = "Досвід роботи повинен бути від 0 до 50 років")]
        public int ExperienceYears { get; set; }

        [Required(ErrorMessage = "Телефон обов'язковий")]
        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Невірний формат телефону")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string? Email { get; set; }

        [Display(Name = "Кабінет")]
        public string? OfficeNumber { get; set; }

        [Display(Name = "Робочі години")]
        public string? WorkingHours { get; set; }

        [Display(Name = "Дата найму")]
        public DateTime HireDate { get; set; } = DateTime.Now;

        [Display(Name = "Активний")]
        public bool IsActive { get; set; } = true;

        
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        [Display(Name = "Повне ім'я")]
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        [Display(Name = "Повне ім'я з посадою")]
        public string FullNameWithTitle => $"{FullName}, {Specialization}";
    }
}
