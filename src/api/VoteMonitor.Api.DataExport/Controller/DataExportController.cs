using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VoteMonitor.Api.DataExport.Queries;

namespace VoteMonitor.Api.DataExport.Controller
{
    [Route("api/v1/export")]
    public class DataExportController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IMediator _mediator;

        public DataExportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Exports all data (which data?) into a excel file
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        [Authorize("NgoAdmin")]
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

            var data = await _mediator.Send(filter);
            var excelFileBytes = await _mediator.Send(new GenerateExcelFile(data));

            if (excelFileBytes == null || excelFileBytes.Length == 0)
            {
                return NotFound();
            }

            return File(
                fileContents: excelFileBytes,
                contentType: Utility.EXCEL_MEDIA_TYPE,
                fileDownloadName: "data.xlsx"
            );
        }
    }
}