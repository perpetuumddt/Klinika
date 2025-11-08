using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace Klinika.Models.Identity;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Прізвище обов'язкове")]
    [Display(Name = "Прізвище")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ім'я обов'язкове")]
    [Display(Name = "Ім'я")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "По батькові")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Email обов'язковий")]
    [EmailAddress(ErrorMessage = "Невірний формат email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон обов'язковий")]
    [Phone(ErrorMessage = "Невірний формат телефону")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Телефон повинен бути у форматі +380XXXXXXXXX")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обов'язковий")]
    [StringLength(16, MinimumLength = 8, ErrorMessage = "Пароль повинен містити від 8 до 16 символів")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Підтвердження паролю обов'язкове")]
    [Compare("Password", ErrorMessage = "Паролі не співпадають")]
    [DataType(DataType.Password)]
    [Display(Name = "Підтвердження паролю")]
    public string ConfirmPassword { get; set; } = string.Empty;
}