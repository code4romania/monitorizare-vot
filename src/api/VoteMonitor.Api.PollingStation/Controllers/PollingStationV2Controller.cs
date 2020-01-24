using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public PollingStationV2Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces(typeof(IEnumerable<Models.PollingStation>))]
        public async Task<IActionResult> GetAllPollingStations()
        {
            var result = await _mediator.Send(new GetAllPollingStations());
            return Ok(result);
        }
    }
}
