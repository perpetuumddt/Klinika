using System.ComponentModel.DataAnnotations;

namespace Klinika.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ім'я користувача або email обов'язкові")]
    [Display(Name = "Ім'я користувача або Email")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обов'язковий")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Запам'ятати мене")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}