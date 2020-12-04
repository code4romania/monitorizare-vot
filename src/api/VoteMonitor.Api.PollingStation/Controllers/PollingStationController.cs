using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.PollingStation.Commands;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Controllers
{
    /// <summary>
    /// Controller responsible for interacting with the polling stations - PollingStationInfo
    /// </summary> 
    [Route("api/v2/polling-station")]
    public class PollingStationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PollingStationController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// Retrieves all of the polling stations matching a certain filter.
        [HttpGet]
        [Produces(typeof(IEnumerable<GetPollingStationModel>))]
        public async Task<IActionResult> GetAllPollingStations([FromQuery] PollingStationsFilterModel pollingStationsFilterModel)
        {
            var request = _mapper.Map<GetPollingStations>(pollingStationsFilterModel);

            var result = await _mediator.Send(request);

            return Ok(result);
        }

        /// Retrieves a polling station with the given ID.
        [HttpGet("{id}")]
        [Authorize("Organizer")]
        [Produces(typeof(GetPollingStationModel))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPollingStation(int id)
        {
            var request = new GetPollingStationById { PollingStationId = id };

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
        public async Task<IActionResult> EditPollingStation([FromRoute] int id, [FromBody] UpdatePollingStationModel pollingStationModel)
        {
            var request = _mapper.Map<UpdatePollingStation>(pollingStationModel);
            request.PollingStationId = id;

            var updated = await _mediator.Send(request);
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
}
