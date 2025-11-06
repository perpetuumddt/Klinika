using System.ComponentModel.DataAnnotations;

namespace Klinika.Models;

public class ApplicationUser
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Username { get; set; } // Unique
    
    [Required]
    [StringLength(500)]
    public string FullName { get; set; }
    
    [Required]
    public string PasswordHash { get; set; }
    
    [Required]
    [Phone]
    [RegularExpression(@"^\+380\d{9}$")] // Ukrainian format
    public string Phone { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; } // Email format
    
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; }
}