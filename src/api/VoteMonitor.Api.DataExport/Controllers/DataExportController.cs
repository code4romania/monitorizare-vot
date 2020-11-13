using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using VoteMonitor.Api.DataExport.FileGenerator;
using VoteMonitor.Api.DataExport.Queries;

namespace VoteMonitor.Api.DataExport.Controllers
{
    [Route("api/v1/export")]
    public class DataExportController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DataExportController> _logger;

        public DataExportController(IMediator mediator, ILogger<DataExportController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Exports all data (which data?) into a excel file
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [Authorize("Organizer")]
        public async Task<IActionResult> GetMyData(int? idNgo, int? idObserver, int? pollingStationNumber, string county, DateTime? from, DateTime? to)
        {
            var filter = new GetDataForExport
            {
                NgoId = idNgo,
                ObserverId = idObserver,
                PollingStationNumber = pollingStationNumber,
                County = county,
                From = from,
                To = to
            };

            var csvFileBytes = default(byte[]);
            try
            {
                var data = await _mediator.Send(filter);
                csvFileBytes = await _mediator.Send(new GenerateCSVFile(data));
            }
            catch (Exception e)
            {
                _logger.LogError(e, nameof(GetMyData));
            }
            

            if (csvFileBytes == null || csvFileBytes.Length == 0)
            {
                return NotFound();
            }

            return File(
                fileContents: csvFileBytes,
                contentType: CsvUtility.CSV_MEDIA_TYPE,
                fileDownloadName: "data.csv"
            );
        }
    }
}