using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.Location.Commands;

namespace VoteMonitor.Api.Location.Controllers;

[ApiController]
[Route("api/v1/polling-stations-cache")]
public class CacheController : ControllerBase
{
    private readonly IMediator _mediator;

    public CacheController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("prefill")]
    [Authorize("Organizer")]
    public async Task<IActionResult> PrefillCache()
    {
        await _mediator.Send(new PrefillCache());
        return NoContent();
    }
}
