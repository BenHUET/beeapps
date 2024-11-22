using System.ComponentModel.DataAnnotations;

namespace BeeApps.Common.DTOs;

public class UserAuthRequestDTO
{
    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }
}