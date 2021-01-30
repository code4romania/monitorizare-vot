using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Controllers
{
    [Route("api/v2/polling-station-info")]
    public class PollingStationInfoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public PollingStationInfoController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize("Observer")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePollingStationInfo([FromBody] CreatePollingStationInfoModel pollingStationInfoModel)
        {
            var pollingStationRequest = new CheckPollingStationExists
            {
                PollingStationId = pollingStationInfoModel.PollingStationId
            };

            var foundPollingStation = await _mediator.Send(pollingStationRequest);
            if (!foundPollingStation)
            {
                return NotFound(pollingStationInfoModel.PollingStationId);
            }
            
            var request = _mapper.Map<CreatePollingStationInfo>(pollingStationInfoModel);
            request.ObserverId = this.GetIdObserver();
            await _mediator.Send(request);
            return Accepted();
        }

        [HttpPut("{id}")]
        [Authorize("Observer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePollingStationInfo([FromRoute]int id, [FromBody]EditPollingStationInfo pollingStationInfo)
        {
            var pollingStationRequest = new CheckPollingStationExists
            {
                PollingStationId = id
            };

            var foundPollingStation = await _mediator.Send(pollingStationRequest);
            if (!foundPollingStation)
            {
                return NotFound(id);
            }

            var request = _mapper.Map<UpdatePollingStationInfo>(pollingStationInfo);
            request.ObserverId = this.GetIdObserver();
            request.PollingStationId = id;

            await _mediator.Send(request);

            return Ok();
        }
    }
}
