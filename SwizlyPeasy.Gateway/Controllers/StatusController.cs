using Microsoft.AspNetCore.Mvc;
using SwizlyPeasy.Common.Dtos.Status;
using SwizlyPeasy.Gateway.Services;

namespace SwizlyPeasy.Gateway.Controllers;

[ApiController]
public class StatusController(IStatusService statusService) : ControllerBase
{
    private readonly IStatusService _statusService = statusService ?? throw new ArgumentNullException(nameof(statusService));

    [HttpGet("")]
    public async Task<ActionResult<StatusDto>> GetStatus()
    {
        var status = await _statusService.GetGatewayStatus();
        return Ok(status);
    }
}