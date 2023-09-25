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
[Route("api/v1/province")]
public class ProvinceController : Controller
{
    private readonly IMediator _mediator;
    public ProvinceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("csvFormat")]
    [Authorize("Organizer")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportToCsvAsync()
    {
        var dataResult = await _mediator.Send(new GetProvincesForExport());

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
    public async Task<IActionResult> ImportAsync([FromForm] ProvincesUploadRequest request)
    {
        var response = await _mediator.Send(new CreateOrUpdateProvinces(request.CsvFile));
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
                new ProvinceCsvModel
                {
                    Id = 1,
                    Code = "CE 1",
                    Name = "Province example 1",
                    Order = 0
                },
                new ProvinceCsvModel
                {
                    Id = 2,
                    Code = "CE 2",
                    Name = "Province example 1",
                    Order = 1
                },

            });
            writer.Flush();
            return File(mem.ToArray(), "application/octet-stream", "province-import-template.csv");
        }
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<ProvinceModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllProvincesAsync()
    {
        var response = await _mediator.Send(new GetAllProvinces());
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }

        return BadRequest(new ErrorModel { Message = response.Error });
    }

    [HttpGet("{provinceId}")]
    [Authorize("NgoAdmin")]
    [ProducesResponseType(typeof(ProvinceModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProvinceAsync([FromRoute] int provinceId)
    {
        var response = await _mediator.Send(new GetProvinceById(provinceId));
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }

        return BadRequest(new ErrorModel { Message = response.Error });
    }

    [HttpPost("{provinceId}")]
    [Authorize("NgoAdmin")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProvinceAsync([FromRoute] int provinceId, [FromBody] UpdateProvinceRequest province)
    {
        var response = await _mediator.Send(new UpdateProvince(provinceId, province));
        if (response.IsSuccess)
        {
            return Ok();
        }

        return BadRequest(new ErrorModel { Message = response.Error });
    }

    [Authorize]
    [HttpGet("{provinceCode}/counties")]
    [ProducesResponseType(typeof(List<MunicipalityModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllMunicipalitiesAsync([FromRoute] string provinceCode)
    {
        var response = await _mediator.Send(new GetAllCountiesByProvinceCode(provinceCode));
        if (response.IsSuccess)
        {
            return Ok(response.Value);
        }

        return BadRequest(new ErrorModel { Message = response.Error });
    }
}
