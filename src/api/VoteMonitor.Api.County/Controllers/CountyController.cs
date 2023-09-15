using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.County.Models;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.County.Commands;
using VoteMonitor.Api.County.Queries;
using System.Globalization;

namespace VoteMonitor.Api.County.Controllers;

[ApiController]
[Route("api/v1/county")]
public class CountyController : Controller
{
    private readonly IMediator _mediator;
    public CountyController(IMediator mediator)
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
        var dataResult = await _mediator.Send(new GetCountiesForExport());

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
            return File(mem.ToArray(), "application/octet-stream", "counties.csv");
        }
    }

    [HttpPost]
    [Route("import")]
    [Authorize("Organizer")]
    public async Task<IActionResult> ImportAsync([FromForm] CountiesUploadRequest request)
    {
        var response = await _mediator.Send(new CreateOrUpdateCounties(request.CsvFile));
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
                new CountyCsvModel
                {
                    Id = 1,
                    Code = "CE 1",
                    Name = "County example 1",
                    Diaspora = false,
                    Order = 0
                }, 
                new CountyCsvModel
                {
                    Id = 2,
                    Code = "CE 2",
                    Name = "County example 1",
                    Diaspora = true,
                    Order = 1
                },

            });
            writer.Flush();
            return File(mem.ToArray(), "application/octet-stream", "observers-import-template.csv");
        }
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<CountyModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllCountiesAsync()
    {
        var response = await _mediator.Send(new GetAllCounties());
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }

        return BadRequest(new ErrorModel { Message = response.Error });
    }

    [HttpGet("{countyId}")]
    [Authorize("NgoAdmin")]
    [ProducesResponseType(typeof(CountyModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCountyAsync([FromRoute] int countyId)
    {
        var response = await _mediator.Send(new GetCountyById(countyId));
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }

        return BadRequest(new ErrorModel { Message = response.Error });
    }

    [HttpPost("{countyId}")]
    [Authorize("NgoAdmin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateCountyAsync([FromRoute]int countyId, [FromBody] UpdateCountyRequest county)
    {
        var response = await _mediator.Send(new UpdateCounty(countyId, county));
        if (response.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(new ErrorModel { Message = response.Error });
    }

    [Authorize]
    [HttpGet("{countyCode}/municipalities")]
    [ProducesResponseType(typeof(List<MunicipalityModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllMunicipalitiesAsync([FromRoute] string countyCode)
    {
        var response = await _mediator.Send(new GetAllMunicipalitiesByCountyCode(countyCode));
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }

        return BadRequest(new ErrorModel { Message = response.Error });
    }
}
