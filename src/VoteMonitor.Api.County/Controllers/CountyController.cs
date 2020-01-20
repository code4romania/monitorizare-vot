using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using VoteMonitor.Api.County.Models;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using VoteMonitor.Api.County.Commands;
using VoteMonitor.Api.County.Queries;

namespace VoteMonitor.Api.County.Controllers
{
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
        [Produces(typeof(byte[]))]
        public async Task<IActionResult> ExportToCsvAsync()
        {
            var dataResult = await _mediator.Send(new GetCountiesForExport());

            if (dataResult.IsFailure)
            {
                return BadRequest(dataResult.Error);
            }

            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer))
            {
                csvWriter.Configuration.HasHeaderRecord = true;
                csvWriter.Configuration.AutoMap<CountyCsvModel>();

                csvWriter.WriteRecords(dataResult.Value);

                writer.Flush();
                return File(mem.ToArray(), "application/octet-stream", "counties.csv");
            }
        }

        [HttpPost]
        [Route("import")]
        [Authorize("Organizer")]
        public async Task<IActionResult> ImportAsync(CountiesUploadModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _mediator.Send(new CreateOrUpdateCounties(request.CsvFile));
            if (response.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new ValidationErrorModel { Message = response.Error });
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCountiesAsync()
        {
            var response = await _mediator.Send(new GetAllCounties());
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(new ValidationErrorModel { Message = response.Error });
        }

        [HttpGet("{countyId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCountyAsync(int countyId)
        {
            var response = await _mediator.Send(new GetCounty(countyId));
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(new ValidationErrorModel { Message = response.Error });
        }


        [HttpPost("{countyId}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCountyAsync(int countyId, [FromBody] CountyModel county)
        {
            var response = await _mediator.Send(new UpdateCounty(countyId, county));
            if (response.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new ValidationErrorModel { Message = response.Error });
        }
    }
}
