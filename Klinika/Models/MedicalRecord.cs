using System.ComponentModel.DataAnnotations;

namespace Klinika.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Пацієнт обов'язковий")]
        [Range(1, int.MaxValue, ErrorMessage = "Пацієнт обов'язковий")]
        [Display(Name = "Пацієнт")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Лікар обов'язковий")]
        [Range(1, int.MaxValue, ErrorMessage = "Лікар обов'язковий")]
        [Display(Name = "Лікар")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Дата запису обов'язкова")]
        [Display(Name = "Дата запису")]
        [DataType(DataType.Date)]
        public DateTime? RecordDate { get; set; }   // <- без дефолта!

        [Display(Name = "Діагноз")]
        [StringLength(1000, ErrorMessage = "Не більше 1000 символів")]
        public string? Diagnosis { get; set; }

        [Display(Name = "Симптоми")]
        [StringLength(1000, ErrorMessage = "Не більше 1000 символів")]
        public string? Symptoms { get; set; }

        [Display(Name = "Лікування")]
        [StringLength(1000, ErrorMessage = "Не більше 1000 символів")]
        public string? Treatment { get; set; }

        [Display(Name = "Призначення")]
        [StringLength(1000, ErrorMessage = "Не більше 1000 символів")]
        public string? Prescriptions { get; set; }

        [Display(Name = "Рекомендації")]
        [StringLength(1000, ErrorMessage = "Не більше 1000 символів")]
        public string? Recommendations { get; set; }

        [Display(Name = "Примітки")]
        [StringLength(1000, ErrorMessage = "Не більше 1000 символів")]
        public string? Notes { get; set; }

        [Display(Name = "Наступний візит")]
        [DataType(DataType.Date)]
        public DateTime? NextVisitDate { get; set; }

        public virtual Patient? Patient { get; set; } = null!;
        public virtual Doctor? Doctor { get; set; } = null!;
    }
}