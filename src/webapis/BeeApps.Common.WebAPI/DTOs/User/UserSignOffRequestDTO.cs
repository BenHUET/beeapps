using System.ComponentModel.DataAnnotations;

namespace BeeApps.Common.DTOs;

public class UserSignOffRequestDTO
{
    [Required] public string RefreshToken { get; set; }
}