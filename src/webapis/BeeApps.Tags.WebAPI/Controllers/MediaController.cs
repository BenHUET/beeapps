using BeeApps.Common.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BeeApps.Tags.WebAPI.Controllers;

[ApiController]
public class MediaController
{
    [HttpPost("users")]
    [ValidTokenFilter]
    public async Task<IActionResult> Create()
    {
        return new OkResult();
    }
}