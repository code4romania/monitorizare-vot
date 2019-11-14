using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.DataExport.Queries;

namespace VoteMonitor.Api.DataExport.Controller
{
    [Route("api/v1/export")]
    public class DataExportController :Microsoft.AspNetCore.Mvc.Controller
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
        public async Task<IActionResult> GetMyData()
        {
            var data = await _mediator.Send(new GetDataForExport());
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