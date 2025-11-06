using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace Klinika.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ім'я обов'язкове")]
    [StringLength(50, ErrorMessage = "Ім'я користувача не повинно перевищувати 50 символів")]
    [Display(Name = "Ім'я користувача")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Повне ім'я обов'язкове")]
    [StringLength(500, ErrorMessage = "Повне ім'я не повинно перевищувати 500 символів")]
    [Display(Name = "Повне ім'я")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email обов'язковий")]
    [EmailAddress(ErrorMessage = "Невірний формат email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Телефон обов'язковий")]
    [Phone(ErrorMessage = "Невірний формат телефону")]
    [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Телефон повинен бути у форматі +380XXXXXXXXX")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обов'язковий")]
    [StringLength(16, MinimumLength = 8, ErrorMessage = "Пароль повинен містити від 8 до 16 символів")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,16}$", 
        ErrorMessage = "Пароль повинен містити: 1 велику літеру, 1 малу літеру, 1 цифру, 1 спеціальний символ")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Підтвердження паролю обов'язкове")]
    [Compare("Password", ErrorMessage = "Паролі не співпадають")]
    [DataType(DataType.Password)]
    [Display(Name = "Підтвердження паролю")]
    public string ConfirmPassword { get; set; } = string.Empty;
}