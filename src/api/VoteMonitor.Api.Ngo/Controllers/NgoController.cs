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
    [Authorize("Organizer")]
    public class NgoController : Controller
    {
        private readonly IMediator _mediator;

        public NgoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all ngos 
        /// </summary>
        /// <returns>A list of ngos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<NgoModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllNgosAsync()
        {
            var ngosListResult = await _mediator.Send(new GetAllNgos());

            if (ngosListResult.IsFailure)
            {
                return Problem(detail: ngosListResult.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok(ngosListResult.Value);
        }
        /// <summary>
        /// Gets details about a specific ngo
        /// </summary>
        /// <param name="id">Id of a ngo</param>
        /// <returns>Details about ngo or error message</returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(NgoModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetNgoByIdAsync(int id)
        {
            var ngoDetailsResult = await _mediator.Send(new GetNgoDetails(id));

            if (ngoDetailsResult.IsFailure)
            {
                return Problem(detail: ngoDetailsResult.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok(ngoDetailsResult.Value);
        }

        /// <summary>
        /// Update ngo
        /// </summary>
        /// <param name="id">Id of ngo to be updated</param>
        /// <param name="ngo">New ngo data</param>
        /// <returns> 200 code response if update was successful or an error </returns>
        [HttpPost]
        [Route("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> UpdateNgoAsync([FromRoute] int id, [FromBody] CreateUpdateNgoModel ngo)
        {
            var result = await _mediator.Send(new UpdateNgo(id, ngo));

            if (result.IsFailure)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok();
        }
        /// <summary>
        /// Adds a new Ngo
        /// </summary>
        /// <param name="ngo">Ngo details</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateNgoAsync([FromBody] CreateUpdateNgoModel ngo)
        {
            var result = await _mediator.Send(new CreateNgo(ngo));

            if (result.IsFailure)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteNgoAsync([FromRoute] int id)
        {
            var result = await _mediator.Send(new DeleteNgo(id));

            if (result.IsFailure)
            {
                return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok();
        }

        [HttpPatch]
        [Route("{id}/status")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateStatusAsync([FromRoute] int id, [FromBody] PatchNgoStatusModel newStatus)
        {
            var ngoDetailsResult = await _mediator.Send(new SetNgoStatusFlag(id, newStatus.IsActive));

            if (ngoDetailsResult.IsFailure)
            {
                return Problem(detail: ngoDetailsResult.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok();
        }

        [HttpPatch]
        [Route("{id}/organizer")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateIsOrganizerAsync([FromRoute] int id, [FromBody] PatchNgoOrganizerModel newStatus)
        {
            var ngoDetailsResult = await _mediator.Send(new SetNgoOrganizerFlag(id, newStatus.IsOrganizer));

            if (ngoDetailsResult.IsFailure)
            {
                return Problem(detail: ngoDetailsResult.Error, statusCode: StatusCodes.Status400BadRequest);
            }

            return Ok();
        }
    }
}
