using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VotingIrregularities.Api.Extensions;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.SectieAggregate;
using VotingIrregularities.Api.Helpers;
using VotingIrregularities.Api.Queries;
using VotingIrregularities.Domain.Models;
using System.Collections.Generic;
using VotingIrregularities.Api.Models.PollingStation;

namespace VotingIrregularities.Api.Controllers
{
    /// <summary>
    /// Controller responsible for interacting with the polling stations - PollingStationInfo 
    /// </summary>
    [Route("api/v1/polling-station")]
    public class PollingStationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public PollingStationController(IMediator mediator, IMapper mapper)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// This method gets called when the observer saves the info regarding the arrival time, leave time, urban area, BESV president
        /// These info come together with the polling station id.
        /// </summary>
        /// <param name="pollingStationInfo">Info about the polling station and its' allocated observer</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IAsyncResult> Register([FromBody] AddPollingStationInfo pollingStationInfo)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);

            var command = _mapper.Map<RegisterPollingStationCommand>(pollingStationInfo);

            // TODO[DH] get the actual IdObservator from token
            command.IdObserver = int.Parse(User.Claims.First(c => c.Type == ClaimsHelper._observerIdProperty).Value);

            var result = await _mediator.Send(command);

            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }

        /// <summary>
        /// This method gets called when updating information about the leave time.
        /// These info come together with the polling station id.
        /// </summary>
        /// <param name="pollingStationInfo">Polling station id, county code, leave time</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IAsyncResult> Update([FromBody] UpdatePollingStationInfo pollingStationInfo)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);

            int idSectie = await _mediator.Send(_mapper.Map<PollingStationQuery>(pollingStationInfo));
            if (idSectie < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            var command = _mapper.Map<UpdatePollingSectionCommand>(pollingStationInfo);

            // TODO get the actual IdObservator from token
            command.IdObserver = int.Parse(User.Claims.First(c => c.Type == "IdObservator").Value);
            command.IdPollingStation = idSectie;

            var result = await _mediator.Send(command);

            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }

        /// <summary>
        /// Gets the polling stations' allocated per county
        /// </summary>
        /// <returns>{ "countyCode": "numberOfPollingStationsAssigned", ... }</returns>
        [HttpGet]
        [Produces(typeof(IEnumerable<CountyPollingStationLimit>))]
        public async Task<IActionResult> PollingStationsLimits()
        {
            var result = await _mediator.Send(new PollingStationsAssignmentQuery());
            return Ok(result);
        }
    }
}
