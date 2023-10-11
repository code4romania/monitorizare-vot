using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.Core.Commands;

namespace VoteMonitor.Api.Core.Controllers;

[ApiController]
[Route("api/v1/cache")]
public class CacheController: Controller
{
    private readonly IMediator _mediator;

    public CacheController(IMediator mediator)
    {
        _mediator = mediator;
    }
    

    [Authorize("Organizer")]
    [HttpPost("clear")]
    public async Task<IActionResult> ClearCacheAsync()
    {
        await _mediator.Send(new ClearCache());
        return Ok();
    }
}
