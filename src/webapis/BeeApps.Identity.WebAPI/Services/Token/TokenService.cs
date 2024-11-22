using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BeeApps.Common.Models;
using BeeApps.Common.WebAPI.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BeeApps.Common.Services;

public class TokenService : ITokenService
{
    private readonly TokenOptions _tokenOptions;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public TokenService(IOptions<TokenOptions> tokenOptions,
        IOptions<TokenValidationParameters> tokenValidationParameters)
    {
        _tokenOptions = tokenOptions.Value;
        _tokenValidationParameters = tokenValidationParameters.Value;
    }

    public async Task<Token> Generate(User user, int lifetimeInMinutes)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.Key));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("u", user.Id.ToString()),
                new Claim("p", Permission.ToShortInline(user.Permissions))
            }),
            Expires = DateTime.UtcNow.AddMinutes(lifetimeInMinutes),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        return new Token
        {
            Value = tokenHandler.WriteToken(securityToken),
            Expiration = tokenDescriptor.Expires.Value
        };
    }

    public async Task<bool> Validate(string token)
    {
        var result = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, _tokenValidationParameters);
        return result.IsValid;
    }

    public async Task<int> GetUserId(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.ValidateToken(token, _tokenValidationParameters, out var securityToken);
        var userId = int.Parse(((JwtSecurityToken)securityToken).Claims.First(c => c.Type == "u").Value);
        return userId;
    }
}