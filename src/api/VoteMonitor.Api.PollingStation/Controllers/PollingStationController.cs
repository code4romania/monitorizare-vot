using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.PollingStation.Commands;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Controllers;

/// <summary>
/// Controller responsible for interacting with the polling stations - PollingStationInfo
/// </summary> 
[Route("api/v2/polling-station")]
public class PollingStationController : Controller
{
    private readonly IMediator _mediator;

    public PollingStationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// Retrieves all of the polling stations matching a certain filter.
    [HttpGet]
    [Authorize] // for now do not allow anonymous users.
    [Produces(typeof(IEnumerable<GetPollingStationModel>))]
    public async Task<IActionResult> GetAllPollingStations([FromQuery] PollingStationsFilterModel request)
    {
        var query = new GetPollingStations(request.CountyId, request.Page, request.PageSize);

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    /// Retrieves a polling station with the given ID.
    [HttpGet("{id}")]
    [Authorize("Organizer")]
    [Produces(typeof(GetPollingStationModel))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPollingStation([FromRoute] int id)
    {
        var request = new GetPollingStationById(id);

        var result = await _mediator.Send(request);
        if (result == null)
        {
            return NotFound(id);
        }

        return Ok(result);
    }

    /// Updates a polling station's details.
    [HttpPut("{id}")]
    [Authorize("Organizer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditPollingStation([FromRoute] int id, [FromBody] UpdatePollingStationModel request)
    {
        var command = new UpdatePollingStation(id, request.Address, request.Number);

        var updated = await _mediator.Send(command);
        if (updated.HasValue && !updated.Value)
        {
            return NotFound(id);
        }

        return Ok();
    }

    [HttpDelete("clearAll")]
    [Authorize("Organizer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClearAll([FromQuery] bool includeRelatedData = false)
    {
        var result = await _mediator.Send(new ClearAllPollingStationsCommand(includeRelatedData));

        if (result.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(result.Error);
    }
}
