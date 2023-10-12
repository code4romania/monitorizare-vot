using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.DataExport.FileGenerator;
using VoteMonitor.Api.DataExport.Queries;

namespace VoteMonitor.Api.DataExport.Controllers;

[Route("api/v2/export")]
public class DataExportV2Controller : Controller
{
    private readonly IMediator _mediator;

    public DataExportV2Controller(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Exports all data in excel file
    /// </summary>
    /// <returns></returns>
    [HttpGet("all")]
    [Authorize("Organizer")]
    public async Task<IActionResult> GetAllData()
    {

        var data = await _mediator.Send(new GetExcelDbCommand());


        return File(
            fileContents: data,
            contentType: ExcelUtility.EXCEL_MEDIA_TYPE,
            fileDownloadName: "data.xlsx"
        );
    }
}
