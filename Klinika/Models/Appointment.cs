using System.ComponentModel.DataAnnotations;

namespace Klinika.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Пацієнт обов'язковий")]
        [Display(Name = "Пацієнт")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Лікар обов'язковий")]
        [Display(Name = "Лікар")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Дата і час запису обов'язкові")]
        [Display(Name = "Дата і час")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDateTime { get; set; } = DateTime.Now;

        [Display(Name = "Тривалість (хв.)")]
        public int DurationMinutes { get; set; } = 30;

        [Display(Name = "Тип прийому")]
        public string? AppointmentType { get; set; }

        [Display(Name = "Скарги")]
        [StringLength(1000, ErrorMessage = "Скарги не повинні перевищувати 1000 символів")]
        public string? Complaints { get; set; }

        [Display(Name = "Статус")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        [Display(Name = "Коментар")]
        public string? Notes { get; set; }

        [Display(Name = "Дата створення")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Дата оновлення")]
        public DateTime? UpdatedDate { get; set; }

        
        public virtual Patient? Patient { get; set; }
        public virtual Doctor? Doctor { get; set; }
    }

    public enum AppointmentStatus
    {
        [Display(Name = "В обробці")]
        Scheduled = 0,
        [Display(Name = "Підтверджений")]
        Confirmed = 1,
        [Display(Name = "Виконаний")]
        Completed = 2,
        [Display(Name = "Відмінений")]
        Cancelled = 3,
        [Display(Name = "Не з'явився")]
        NotVisited = 4
    }
}
