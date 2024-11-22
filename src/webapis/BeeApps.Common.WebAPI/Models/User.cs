using System.ComponentModel.DataAnnotations;

namespace BeeApps.Common.Models;

public class User
{
    [Required] public int Id { get; set; }

    [Required] public string Username { get; set; }

    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }

    [Required] public string PasswordSalt { get; set; }

    public string? RefreshToken { get; set; }

    public string? RefreshTokenSalt { get; set; }

    public string? ValidationToken { get; set; }

    [Required] public DateTime CreatedAt { get; set; }

    [Required] public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; }
}