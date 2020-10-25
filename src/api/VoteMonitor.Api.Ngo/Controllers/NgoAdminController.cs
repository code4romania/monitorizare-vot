using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.Ngo.Commands;
using VoteMonitor.Api.Ngo.Models;
using VoteMonitor.Api.Ngo.Queries;

namespace VoteMonitor.Api.Ngo.Controllers
{
    [Route("api/v1/ngo")]
    public class NgoAdminController : Controller
    {
        private readonly IMediator _mediator;

        public NgoAdminController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        [Route("{idNgo}/ngoadmin/")]
        [Authorize("NgoAdmin")]
        [ProducesResponseType(typeof(List<NgoModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllNgosAsync(int idNgo)
        {
            var ngosListResult = await _mediator.Send(new GetAllNgoAdmins(idNgo));

            if (ngosListResult.IsFailure)
            {
                return Problem(detail: ngosListResult.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok(ngosListResult.Value);
        }

        [HttpGet]
        [Route("{idNgo}/ngoadmin/{ngoAdminId}")]
        [ProducesResponseType(typeof(NgoModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetNgoByIdAsync([FromRoute] int idNgo, [FromRoute] int ngoAdminId)
        {
            var ngoDetailsResult = await _mediator.Send(new GetNgoAdminDetails(idNgo, ngoAdminId));

            if (ngoDetailsResult.IsFailure)
            {
                return Problem(detail: ngoDetailsResult.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok(ngoDetailsResult.Value);
        }

        [HttpPost]
        [Route("{idNgo}/ngoadmin/{ngoAdminId}")]
        [Authorize("NgoAdmin")]
        [ProducesResponseType(typeof(List<NgoModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateNgoAdminAsync([FromRoute] int idNgo, [FromRoute] int ngoAdminId, [FromBody] CreateUpdateNgoAdminModel model)
        {
            var result = await _mediator.Send(new UpdateNgoAdmin(idNgo, ngoAdminId, model));

            if (result.IsFailure)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok();
        }

        [HttpPost]
        [Route("{idNgo}/ngoadmin")]
        [Authorize("NgoAdmin")]
        [ProducesResponseType(typeof(List<NgoModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateNgoAdminAsync([FromRoute] int idNgo, [FromBody] CreateUpdateNgoAdminModel model)
        {
            var result = await _mediator.Send(new CreateNgoAdmin(idNgo, model));

            if (result.IsFailure)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{idNgo}/ngoadmin/{ngoAdminId}")]
        [Authorize("NgoAdmin")]
        [ProducesResponseType(typeof(List<NgoModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteNgoAdminAsync([FromRoute] int idNgo, [FromRoute] int ngoAdminId)
        {
            var result = await _mediator.Send(new DeleteNgoAdmin(idNgo, ngoAdminId));

            if (result.IsFailure)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok();
        }
    }
}