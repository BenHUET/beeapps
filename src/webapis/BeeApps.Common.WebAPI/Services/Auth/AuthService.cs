using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BeeApps.Common.DTOs;
using BeeApps.Common.WebAPI.Options;
using Microsoft.Extensions.Options;

namespace BeeApps.Common.Services;

public class AuthService : IAuthService
{
    private readonly AuthOptions _authOptions;
    private readonly HttpClient _httpClient;

    public AuthService(IOptions<AuthOptions> authOptions, IHttpClientFactory httpClientFactory)
    {
        _authOptions = authOptions.Value;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<bool> Authorize(string token)
    {
        var dto = new UserValidateTokenRequestDTO { Token = token };

        var payload = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        payload.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync(_authOptions.ValidateTokenURL, payload);

        return response.IsSuccessStatusCode;
    }
}