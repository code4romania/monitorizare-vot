using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Api.Location.Models;
using VoteMonitor.Api.Location.Models.ResultValues;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Location.Services;

namespace VoteMonitor.Api.Location.Controllers
{
    /// <summary>
    /// Controller responsible for interacting with the polling stations - PollingStationInfo 
    /// </summary>
    [Route("api/v1/polling-station")]
    public class PollingStationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IFileLoader _fileLoader;

        public PollingStationController(IMediator mediator, IMapper mapper, IFileLoader loader)
        {
            _mapper = mapper;
            _mediator = mediator;
            _fileLoader = loader;
        }

        /// <summary>
        /// This method gets called when the observer saves the info regarding the arrival time, leave time, urban area, BESV president
        /// These info come together with the polling station id.
        /// </summary>
        /// <param name="pollingStationInfo">Info about the polling station and its' allocated observer</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IAsyncResult> Register([FromBody] AddPollingStationInfo pollingStationInfo)
        {
            if (!ModelState.IsValid)
            {
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);
            }

            var command = _mapper.Map<RegisterPollingStationCommand>(pollingStationInfo);

            // TODO[DH] get the actual IdObservator from token
            command.IdObserver = int.Parse(User.Claims.First(c => c.Type == ClaimsHelper.ObserverIdProperty).Value);

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
        [Authorize]
        public async Task<IAsyncResult> Update([FromBody] UpdatePollingStationInfo pollingStationInfo)
        {
            if (!ModelState.IsValid)
            {
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);
            }

            var idSectie = await _mediator.Send(_mapper.Map<PollingStationQuery>(pollingStationInfo));
            if (idSectie < 0)
            {
                return this.ResultAsync(HttpStatusCode.NotFound);
            }

            var command = _mapper.Map<UpdatePollingSectionCommand>(pollingStationInfo);

            command.IdObserver = this.GetIdObserver();
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
        public async Task<IActionResult> PollingStationsLimits(bool? diaspora)
        {
            var result = await _mediator.Send(new PollingStationsAssignmentQuery(diaspora));
            return Ok(result);
        }

        [HttpPost("import")]
        [Authorize("Organizer")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportFormatFile(IFormFile file)
        {
            if (!_fileLoader.ValidateFile(file))
            {
                return UnprocessableEntity();
            }

            var result = await _mediator.Send(new PollingStationCommand(_fileLoader.ImportFileAsync(file).Result));

            if (result.Success)
            {
                return Ok();
            }
            else if (result.Error.ErrorCode == PollingStationImportErrorCode.CountyNotFound)
            {
                return BadRequest(result.Error.Exception.Message);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpGet]
        [Route("excelHeaderFile")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ExcelHeaderFile()
        {
            return File(_fileLoader.ExportHeaderInformation(), "application/octet-stream", "pollingStationHeader.xlsx");
        }
    }
}
