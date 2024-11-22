using System.ComponentModel.DataAnnotations;

namespace BeeApps.Common.DTOs;

public class UserCreateRequestDTO
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]{4,16}$",
        ErrorMessage =
            "Username must have 4 to 16 alphanumeric characters")]
    public string Username { get; set; }

    [Required]
    [RegularExpression(
        @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~\\])[A-Za-z\d !""#$%&'()*+,-./:;<=>?@[\]^_`{|}~\\]{8,64}$",
        ErrorMessage = "Password must have 8 to 64 alphanumeric characters, at least one letter, one number and one special character"
    )]
    public string Password { get; set; }

    [Required] 
    [EmailAddress] 
    public string Email { get; set; }
}