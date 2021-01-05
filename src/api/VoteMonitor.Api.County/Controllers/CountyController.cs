using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using VoteMonitor.Api.County.Models;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.County.Commands;
using VoteMonitor.Api.County.Queries;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.County.Controllers
{
    [Route("api/v1/county")]
    public class CountyController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<CountyController> _localizer;

        public CountyController(IMediator mediator, IStringLocalizer<CountyController> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
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

            return BadRequest(new ErrorModel { Message = response.Error });
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
        public async Task<IActionResult> GetCountyAsync(int countyId)
        {
            var response = await _mediator.Send(new GetCounty(countyId));
            if (response.IsSuccess)
            {
                return Ok(response.Value);
            }

            return BadRequest(new ErrorModel { Message = response.Error });
        }

        [HttpPost("{countyId}")]
        [Authorize("Organizer")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCountyAsync(int countyId, [FromBody] UpdateCountyModel county)
        {
            var response = await _mediator.Send(new UpdateCounty(countyId, county));
            if (response.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new ErrorModel { Message = response.Error });
        }

        /// <summary>
        /// This method is used for test. will be removed after this approch is approved;
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpGet("test-localization")]
        public IActionResult TestLocalization([Required] string name)
        {
            string text = string.Format(_localizer["hello"].Value, name);
            string noValue = _localizer["no-translation"].Value;
            string onlyEnglish = _localizer["only-english"].Value;

            return Ok(new
            {
                formattedText = text,
                noValue,
                onlyEnglish
            });
        }
    }
}
