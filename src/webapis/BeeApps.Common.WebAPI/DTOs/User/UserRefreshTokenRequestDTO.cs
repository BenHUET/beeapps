using System.ComponentModel.DataAnnotations;

namespace BeeApps.Common.DTOs;

public class UserRefreshTokenRequestDTO
{
    [Required] public string RefreshToken { get; set; }
}