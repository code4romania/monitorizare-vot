using MediatR;
using Microsoft.AspNetCore.Mvc;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using VoteMonitor.Api.County.Commands;
using VoteMonitor.Api.County.Models;
using VoteMonitor.Api.County.Queries;

namespace VoteMonitor.Api.County.Controllers;

[ApiController]
[Route("api/v1/municipality")]
public class MunicipalityController : Controller
{
    private readonly IMediator _mediator;
    public MunicipalityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("csvFormat")]
    [Authorize("Organizer")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportToCsvAsync()
    {
        var dataResult = await _mediator.Send(new GetMunicipalitiesForExport());

        if (dataResult.IsFailure)
        {
            return BadRequest(dataResult.Error);
        }

        using (var mem = new MemoryStream())
        using (var writer = new StreamWriter(mem))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(dataResult.Value);

            writer.Flush();
            return File(mem.ToArray(), "application/octet-stream", "municipalities.csv");
        }
    }

    [HttpPost]
    [Route("import")]
    [Authorize("Organizer")]
    public async Task<IActionResult> ImportAsync([FromForm] MunicipalitiesUploadRequest request)
    {
        var response = await _mediator.Send(new CreateOrUpdateMunicipalities(request.CsvFile));
        if (response.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(new ErrorModel { Message = response.Error });
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
                new MunicipalityCsvModel
                {
                    Id = 1,
                    CountyCode = "County1",
                    Code = "Municipality 1",
                    Name = "Municipality example 1",
                    Order = 0
                },
                new MunicipalityCsvModel
                {
                    Id = 2,
                    CountyCode = "County1",
                    Code = "Municipality 2",
                    Name = "Municipality example 2",
                    Order = 2
                },

            });
            writer.Flush();
            return File(mem.ToArray(), "application/octet-stream", "municipalities-import-template.csv");
        }
    }

    [HttpGet("{municipalityId}")]
    [Authorize("NgoAdmin")]
    [ProducesResponseType(typeof(MunicipalityModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMunicipalityAsync([FromRoute] int municipalityId)
    {
        var response = await _mediator.Send(new GetMunicipalityById(municipalityId));
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }

        return BadRequest(new ErrorModel { Message = response.Error });
    }

    [HttpPost("{municipalityId}")]
    [Authorize("NgoAdmin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMunicipalityAsync([FromRoute] int municipalityId, [FromBody] UpdateMunicipalityRequest request)
    {
        var response = await _mediator.Send(new UpdateMunicipality(municipalityId, request));
        if (response.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(new ErrorModel { Message = response.Error });
    }
}
