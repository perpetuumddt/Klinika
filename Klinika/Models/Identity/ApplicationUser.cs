using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Klinika.Models.Identity;

public class ApplicationUser : IdentityUser<int>
{
    [Required]
    [StringLength(500)]
    public string FullName { get; set; }
    
    [Required]
    [Phone]
    [RegularExpression(@"^\+380\d{9}$")] // Ukrainian format
    public string PhoneNumberUA { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsActive { get; set; } = true;
}