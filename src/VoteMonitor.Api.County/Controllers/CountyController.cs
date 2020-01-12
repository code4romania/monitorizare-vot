using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VoteMonitor.Api.County.Models;
using VoteMonitor.Api.Observer.Queries;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

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
        [AllowAnonymous]
        [Produces(typeof(byte[]))]
        public async Task<IActionResult> ExportToCsvAsync()
        {
            // add authorization
            List<CountyCsvModel> data = await _mediator.Send(new GetCountiesForExport());
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer))
            {
                csvWriter.Configuration.HasHeaderRecord = true;
                csvWriter.Configuration.AutoMap<CountyCsvModel>();

                csvWriter.WriteRecords(data);

                writer.Flush();
                return File(mem.ToArray(), "application/octet-stream", "counties.csv");
            }
        }

        [HttpPost]
        [Route("import")]
        [AllowAnonymous]
        public async Task ImportAsync(IFormFile file)
        {
            // add authorization

        }
    }
}
