using CsvHelper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Api.Location.Models;
using VoteMonitor.Api.Location.Models.ResultValues;
using VoteMonitor.Api.Location.Queries;

namespace VoteMonitor.Api.Location.Controllers;

/// <summary>
/// Controller responsible for interacting with the polling stations - PollingStationInfo 
/// </summary>
[ApiController]
[Route("api/v1/polling-station")]
public class PollingStationController : Controller
{
    private readonly IMediator _mediator;

    public PollingStationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// This method gets called when the observer saves the info regarding the arrival time, leave time, urban area, BESV president
    /// These info come together with the polling station id.
    /// </summary>
    /// <param name="request">Info about the polling station and its' allocated observer</param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Register([FromBody] AddPollingStationInfo request)
    {
        var command = new RegisterPollingStationCommand
        {
            CountyCode = request.CountyCode,
            MunicipalityCode = request.MunicipalityCode,
            ObserverArrivalTime = request.ObserverArrivalTime,

            NumberOfVotersOnTheList = request.NumberOfVotersOnTheList!.Value,
            NumberOfCommissionMembers = request.NumberOfCommissionMembers!.Value,
            NumberOfFemaleMembers = request.NumberOfFemaleMembers!.Value,
            MinPresentMembers = request.MinPresentMembers!.Value,
            ChairmanPresence = request.ChairmanPresence!.Value,
            SinglePollingStationOrCommission = request.SinglePollingStationOrCommission!.Value,
            AdequatePollingStationSize = request.AdequatePollingStationSize!.Value,
            IdObserver = int.Parse(User.Claims.First(c => c.Type == ClaimsHelper.ObserverIdProperty).Value),
            PollingStationNumber = request.PollingStationNumber
        };

        var result = await _mediator.Send(command);

        if (result < 0) return NotFound();

        return Ok();
    }

    /// <summary>
    /// This method gets called when updating information about the leave time.
    /// These info come together with the polling station id.
    /// </summary>
    /// <param name="request">Polling station id, county code, leave time</param>
    /// <returns></returns>
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] UpdatePollingStationInfo request)
    {
        var pollingStationId = await _mediator.Send(new GetPollingStationId(request.CountyCode, request.MunicipalityCode, request.PollingStationNumber));
        if (pollingStationId < 0)
        {
            return NotFound();
        }

        var command = new UpdatePollingSectionCommand
        {
            ObserverId = this.GetIdObserver(),
            PollingStationId = pollingStationId
        };

        var result = await _mediator.Send(command);
        if (result < 0)
        {
            return NotFound();
        }

        return Ok();
    }

    /// <summary>
    /// Gets the polling stations' allocated per county
    /// </summary>
    /// <returns>{ "countyCode": "numberOfPollingStationsAssigned", ... }</returns>
    [HttpGet]
    [Authorize("Organizer")]
    [Produces(typeof(IEnumerable<CountyPollingStationLimit>))]
    public async Task<IActionResult> PollingStationsLimits([FromQuery] bool? diaspora)
    {
        var result = await _mediator.Send(new PollingStationsAssignmentQuery(diaspora));
        return Ok(result);
    }

    [HttpPost("import")]
    [Authorize("Organizer")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportFormatFile([FromForm] PollingStationsUploadRequest request)
    {
        var result = await _mediator.Send(new ImportPollingStationsCommand(request.CsvFile));

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
    [Route("import-template")]
    [Authorize("Organizer")]
    public IActionResult DownloadImportTemplate()
    {
        using (var mem = new MemoryStream())
        using (var writer = new StreamWriter(mem))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(new[]
            {
                new PollingStationCsvModel
                {
                    Address = "Example address",
                    MunicipalityCode = "EC",
                    Number = 1
                },
                new PollingStationCsvModel
                {
                    Address = "Example address",
                    MunicipalityCode = "EC",
                    Number = 2
                }

            });
            writer.Flush();
            return File(mem.ToArray(), "application/octet-stream", "observers-import-template.csv");
        }
    }

}
