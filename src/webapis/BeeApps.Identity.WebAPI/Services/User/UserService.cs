using System.Security.Authentication;
using BeeApps.Common.DTOs;
using BeeApps.Common.Exceptions;
using BeeApps.Common.Models;
using BeeApps.Common.Repositories;
using BeeApps.Common.WebAPI.Exceptions;
using BeeApps.Common.WebAPI.Options;
using Microsoft.Extensions.Options;

namespace BeeApps.Common.Services;

public class UserService : IUserService
{
    private readonly AuthOptions _authOptions;
    private readonly IHashService _hashService;
    private readonly ILogger<UserService> _logger;
    private readonly IMailService _mailService;
    private readonly Random _random;
    private readonly TokenOptions _tokenOptions;
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public UserService(ILogger<UserService> logger,
        IUserRepository userRepository,
        IHashService hashService,
        ITokenService tokenService,
        IMailService mailService,
        IOptions<TokenOptions> tokenOptions,
        IOptions<AuthOptions> authOptions)
    {
        _logger = logger;
        _userRepository = userRepository;
        _hashService = hashService;
        _tokenService = tokenService;
        _mailService = mailService;
        _tokenOptions = tokenOptions.Value;
        _authOptions = authOptions.Value;
        _random = new Random();
    }

    public async Task Create(UserCreateRequestDTO dto)
    {
        // Username uniqueness
        if (!await _userRepository.IsUniqueUsername(dto.Username))
            throw new NotUniqueInDatabaseException(nameof(dto.Username));

        // Email uniqueness
        if (!await _userRepository.IsUniqueEmail(dto.Email))
            throw new NotUniqueInDatabaseException(nameof(dto.Email));

        // Hash password
        var saltPassword = _hashService.GetSalt(256 / 8);
        var hashedPassword = _hashService.Hash(dto.Password, saltPassword);

        // Generate validation link
        var validationToken = getRandomString(12);

        // Insert to database
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = hashedPassword,
            PasswordSalt = Convert.ToBase64String(saltPassword),
            ValidationToken = _authOptions.RequiresValidation ? validationToken : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Send mail to user asking for email validation
        if (_authOptions.RequiresValidation)
            sendValidateEmail(user);

        await _userRepository.Insert(user);

        _logger.Log(LogLevel.Information, "User {Email} / {Username} created", user.Email, user.Username);
    }

    public async Task ValidateEmail(string token)
    {
        // Find user with this validation token
        var user = await _userRepository.GetByToken(token);

        // Validate user
        user.ValidationToken = null;
        await _userRepository.Update(user);
    }

    public async Task ResendEmailValidation(UserAuthRequestDTO dto)
    {
        try
        {
            await Authenticate(dto);
        }
        catch (EmailNotValidatedException)
        {
            // Find user with this email
            var user = await _userRepository.GetByEmail(dto.Email);
            
            // Generate new validation link            
            user.ValidationToken = getRandomString(12);
            
            await _userRepository.Update(user);
            
            if (_authOptions.RequiresValidation)
                sendValidateEmail(user);
        }
    }

    public async Task<UserAuthResponseDTO> Authenticate(UserAuthRequestDTO dto)
    {
        // Find user with this email
        var user = await _userRepository.GetByEmail(dto.Email);

        // Check password with salt from database
        var hashedPasswordFromDb = _hashService.Hash(dto.Password, user.PasswordSalt);
        if (!user.Password.Equals(hashedPasswordFromDb))
            throw new AuthenticationException();

        // Check if email is validated
        if (user.ValidationToken != null)
            throw new EmailNotValidatedException();

        var (accessToken, refreshToken) = await generateTokens(user);

        _logger.Log(LogLevel.Information, "User {Email} authenticated", user.Email);

        return new UserAuthResponseDTO(user, accessToken, refreshToken);
    }

    public async Task SignOff(UserSignOffRequestDTO dto)
    {
        // Retrieve user id from token
        var userId = await _tokenService.GetUserId(dto.RefreshToken);

        // Retrieve user from database
        var user = await _userRepository.GetById(userId);

        // Remove refresh token
        user.RefreshToken = null;
        user.RefreshTokenSalt = null;
        await _userRepository.Update(user);
    }

    public async Task<UserAuthResponseDTO> RefreshToken(UserRefreshTokenRequestDTO dto)
    {
        // Try to get user id from token
        int tokenUserId;
        try
        {
            tokenUserId = await _tokenService.GetUserId(dto.RefreshToken);
        }
        catch
        {
            throw new InvalidTokenException();
        }

        // Retrieve user from database
        var user = await _userRepository.GetById(tokenUserId);
        if (user.RefreshToken == null || user.RefreshTokenSalt == null)
            throw new NoDataFoundException();

        // Hash token from dto 
        var hashedTokenFromDTO = _hashService.Hash(dto.RefreshToken, user.RefreshTokenSalt);

        // Compare token from dto to database
        if (hashedTokenFromDTO != user.RefreshToken)
            throw new InvalidTokenException();

        // Refresh tokens
        var (accessToken, refreshToken) = await generateTokens(user);

        _logger.Log(LogLevel.Information, "Tokens refreshed for user {Email}", user.Email);

        return new UserAuthResponseDTO(user, accessToken, refreshToken);
    }

    private async Task<(Token, Token)> generateTokens(User user)
    {
        // Generate tokens
        var accessToken = await _tokenService.Generate(user, _tokenOptions.AccessLifetimeInMinutes);
        var refreshToken = await _tokenService.Generate(user, _tokenOptions.RefreshLifetimeInMinutes);

        // Hash refresh token to store in database
        var saltRefreshToken = _hashService.GetSalt(256 / 8);
        var hashedRefreshToken = _hashService.Hash(refreshToken.Value, saltRefreshToken);

        // Store refresh token and its salt in database
        user.RefreshToken = hashedRefreshToken;
        user.RefreshTokenSalt = Convert.ToBase64String(saltRefreshToken);
        await _userRepository.Update(user);

        _logger.Log(LogLevel.Information, "Tokens generated for user {Email}", user.Email);

        return (accessToken, refreshToken);
    }

    private string getRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Range(1, length).Select(_ => chars[_random.Next(chars.Length)]).ToArray());
    }

    private async void sendValidateEmail(User user)
    {
        await _mailService.Send(new Mail(
            user.Email,
            user.Username,
            "Confirm your email",
            $"To confirm this email address for your new account at beeapps.org, <a href=\"{_authOptions.ValidateEmailURL}/{user.ValidationToken}\">click here</a>.")
        );

        _logger.Log(LogLevel.Information, "Email validation sent to {Email}", user.Email);
    }
}