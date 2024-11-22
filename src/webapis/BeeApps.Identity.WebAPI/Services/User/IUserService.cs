using BeeApps.Common.DTOs;

namespace BeeApps.Common.Services;

public interface IUserService
{
    public Task Create(UserCreateRequestDTO dto);
    public Task<UserAuthResponseDTO> Authenticate(UserAuthRequestDTO dto);
    public Task ValidateEmail(string token);
    public Task ResendEmailValidation(UserAuthRequestDTO dto);
    public Task SignOff(UserSignOffRequestDTO dto);
    public Task<UserAuthResponseDTO> RefreshToken(UserRefreshTokenRequestDTO dto);
}