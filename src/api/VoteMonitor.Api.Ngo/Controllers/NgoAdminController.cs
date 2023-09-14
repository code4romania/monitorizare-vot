using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Ngo.Commands;
using VoteMonitor.Api.Ngo.Models;
using VoteMonitor.Api.Ngo.Queries;

namespace VoteMonitor.Api.Ngo.Controllers;

[Route("api/v1/ngo")]
[Authorize("NgoAdmin")]
public class NgoAdminController : Controller
{
    private readonly IMediator _mediator;

    private int UserNgoId => this.GetIdOngOrDefault(-1);


    public NgoAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{ngoId}/ngoadmin/")]
    [ProducesResponseType(typeof(List<NgoModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllNgosAsync([FromRoute] int ngoId)
    {
        if(UserNgoId != ngoId)
        {
            return Problem(detail: "Cannot edit that are not in your Ngo", statusCode: StatusCodes.Status400BadRequest);
        }

        var ngosListResult = await _mediator.Send(new GetAllNgoAdmins(ngoId));

        if (ngosListResult.IsFailure)
        {
            return Problem(detail: ngosListResult.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(ngosListResult.Value);
    }

    [HttpGet]
    [Route("{ngoId}/ngoadmin/{adminId}")]
    [ProducesResponseType(typeof(NgoModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNgoByIdAsync([FromRoute] int ngoId, [FromRoute] int adminId)
    {
        if (UserNgoId != ngoId)
        {
            return Problem(detail: "Cannot get admins that are not in your Ngo", statusCode: StatusCodes.Status400BadRequest);
        }

        var ngoDetailsResult = await _mediator.Send(new GetNgoAdminDetails(ngoId, adminId));

        if (ngoDetailsResult.IsFailure)
        {
            return Problem(detail: ngoDetailsResult.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(ngoDetailsResult.Value);
    }

    [HttpPost]
    [Route("{idNgo}/ngoadmin/{ngoAdminId}")]
    [ProducesResponseType(typeof(List<NgoModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateNgoAdminAsync([FromRoute] int idNgo, [FromRoute] int ngoAdminId, [FromBody] CreateUpdateNgoAdminModel model)
    {
        if (UserNgoId != idNgo)
        {
            return Problem(detail: "Cannot edit admins that are not in your Ngo", statusCode: StatusCodes.Status400BadRequest);
        }

        var result = await _mediator.Send(new UpdateNgoAdmin(idNgo, ngoAdminId, model));

        if (result.IsFailure)
        {
            return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok();
    }

    [HttpPost]
    [Route("{idNgo}/ngoadmin")]
    [ProducesResponseType(typeof(List<NgoModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateNgoAdminAsync([FromRoute] int idNgo, [FromBody] CreateUpdateNgoAdminModel model)
    {
        if (UserNgoId != idNgo)
        {
            return Problem(detail: "Cannot create admins that are not in your Ngo", statusCode: StatusCodes.Status400BadRequest);
        }

        var result = await _mediator.Send(new CreateNgoAdmin(idNgo, model));

        if (result.IsFailure)
        {
            return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok();
    }

    [HttpDelete]
    [Route("{idNgo}/ngoadmin/{ngoAdminId}")]
    [ProducesResponseType(typeof(List<NgoModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteNgoAdminAsync([FromRoute] int idNgo, [FromRoute] int ngoAdminId)
    {
        if (UserNgoId != idNgo)
        {
            return Problem(detail: "Cannot delete admins that are not in your Ngo", statusCode: StatusCodes.Status400BadRequest);
        }

        var result = await _mediator.Send(new DeleteNgoAdmin(idNgo, ngoAdminId));

        if (result.IsFailure)
        {
            return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok();
    }
}
