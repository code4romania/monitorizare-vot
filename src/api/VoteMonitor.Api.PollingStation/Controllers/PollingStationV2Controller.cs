using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Controllers
{
    /// <summary>
    /// Controller responsible for interacting with the polling stations - PollingStationInfo 
    /// </summary>
    [Route("api/v2/polling-station")]
    public class PollingStationV2Controller : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PollingStationV2Controller(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces(typeof(IEnumerable<GetPollingStation>))]
        public async Task<IActionResult> GetAllPollingStations([FromQuery]PollingStationsFilter pollingStationsFilter)
        {
            var request = _mapper.Map<GetPollingStations>(pollingStationsFilter);

            var result = await _mediator.Send(request);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize("Organizer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditPollingStation([FromRoute]int id, [FromBody]Models.UpdatePollingStation pollingStation)
        {
            var request = _mapper.Map<Queries.UpdatePollingStation>(pollingStation);
            request.Id = id;

            var updated = await _mediator.Send(request);
            if (updated.HasValue && !updated.Value)
            {
                return NotFound(id);
            }

            return Ok();
        }
    }
}
