using BeeApps.Common.Models;

namespace BeeApps.Common.DTOs;

public class UserAuthResponseDTO
{
    public UserAuthResponseDTO(User user, Token accessToken, Token refreshToken)
    {
        Username = user.Username;
        Email = user.Email;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public string Username { get; }
    public string Email { get; }
    public Token AccessToken { get; }
    public Token RefreshToken { get; }
}