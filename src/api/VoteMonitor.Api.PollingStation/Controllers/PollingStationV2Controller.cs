using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [Produces(typeof(IEnumerable<Models.PollingStation>))]
        public async Task<IActionResult> GetAllPollingStations([FromQuery]PollingStationsFilter pollingStationsFilter)
        {
            var request = _mapper.Map<GetPollingStations>(pollingStationsFilter);

            var result = await _mediator.Send(request);
            
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Produces(typeof(Models.PollingStation))]
        public async Task<IActionResult> CreatePollingStation([FromBody]Models.PollingStation pollingStation)
        {
            var result = await _mediator.Send(new CreatePollingStation(pollingStation));
            return Ok(result);
        }
    }
}
