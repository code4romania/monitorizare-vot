using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Controllers;

[Route("api/v2/polling-station-info")]
public class PollingStationInfoController : Controller
{
    private readonly IMediator _mediator;

    public PollingStationInfoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize("Observer")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePollingStationInfo([FromBody] CreatePollingStationInfoModel request)
    {
        var pollingStationRequest = new CheckPollingStationExists(request.PollingStationId);

        var foundPollingStation = await _mediator.Send(pollingStationRequest);
        if (!foundPollingStation)
        {
            return NotFound(request.PollingStationId);
        }

        var command = new CreatePollingStationInfo(this.GetIdObserver(), request.PollingStationId, request.CountyCode, request.ObserverLeaveTime, request.ObserverArrivalTime, request.IsPollingStationPresidentFemale);
        await _mediator.Send(command);
        return Accepted();
    }

    [HttpPut("{id}")]
    [Authorize("Observer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePollingStationInfo([FromRoute] int id, [FromBody] EditPollingStationInfo request)
    {
        var pollingStationRequest = new CheckPollingStationExists(id);

        var foundPollingStation = await _mediator.Send(pollingStationRequest);
        if (!foundPollingStation)
        {
            return NotFound(id);
        }

        var command = new UpdatePollingStationInfo(this.GetIdObserver(), id, request.ObserverLeaveTime.Value);
        await _mediator.Send(command);

        return Ok();
    }
}
