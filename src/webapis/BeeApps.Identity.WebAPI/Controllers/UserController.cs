using System.Security.Authentication;
using BeeApps.Common.DTOs;
using BeeApps.Common.Exceptions;
using BeeApps.Common.Filters;
using BeeApps.Common.Services;
using BeeApps.Common.WebAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace BeeApps.Common.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public UserController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("users")]
    public async Task<IActionResult> Create(UserCreateRequestDTO dto)
    {
        try
        {
            await _userService.Create(dto);
        }
        catch (NotUniqueInDatabaseException e)
        {
            // Do not return any error to not leak any email from database 
            if (e.Field == "Email")
                return StatusCode(StatusCodes.Status201Created);

            ModelState.AddModelError(e.Field, e.Message);
            return ValidationProblem();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }

        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("users/validate-email")]
    public async Task<IActionResult> ValidateEmail(string token)
    {
        try
        {
            await _userService.ValidateEmail(token);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return BadRequest();
        }
        catch (NoDataFoundException e)
        {
            return BadRequest();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
    
    [HttpPost("users/resend-email")]
    public async Task<IActionResult> ResendEmailValidation(UserAuthRequestDTO dto)
    {
        try
        {
            await _userService.ResendEmailValidation(dto);
            return Ok();
        }
        catch (NoDataFoundException e)
        {
            return Unauthorized();
        }
        catch (AuthenticationException e)
        {
            return Unauthorized();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpPost("users/authenticate")]
    public async Task<IActionResult> Authenticate(UserAuthRequestDTO dto)
    {
        try
        {
            var result = await _userService.Authenticate(dto);
            return Ok(result);
        }
        catch (NoDataFoundException e)
        {
            return Unauthorized();
        }
        catch (AuthenticationException e)
        {
            return Unauthorized();
        }
        catch (EmailNotValidatedException e)
        {
            return new StatusCodeResult(403);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpPost("users/signoff")]
    [ValidTokenFilter]
    public async Task<IActionResult> SignOff(UserSignOffRequestDTO dto)
    {
        try
        {
            await _userService.SignOff(dto);
            return Ok();
        }
        catch (NoDataFoundException e)
        {
            return Unauthorized();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpPost("users/validate-token")]
    public async Task<IActionResult> ValidateToken(UserValidateTokenRequestDTO dto)
    {
        try
        {
            var result = await _tokenService.Validate(dto.Token);

            if (result)
                return Ok();

            return Unauthorized();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }

    [HttpPost("users/refresh-token")]
    public async Task<IActionResult> RefreshTokens(UserRefreshTokenRequestDTO dto)
    {
        try
        {
            var result = await _userService.RefreshToken(dto);
            return Ok(result);
        }
        catch (NoDataFoundException e)
        {
            return Unauthorized();
        }
        catch (InvalidTokenException e)
        {
            return Unauthorized();
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
}