using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.County.Commands;

namespace VoteMonitor.Api.County.Controllers;


[ApiController]
[Route("api/v1/locations-cache")]
public class CacheController: ControllerBase
{
    private readonly IMediator _mediator;

    public CacheController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize("Organizer")]
    [HttpGet("prefill")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> PrefillCache()
    {
        await _mediator.Send(new PrefillCache());
        return NoContent();
    }
}
