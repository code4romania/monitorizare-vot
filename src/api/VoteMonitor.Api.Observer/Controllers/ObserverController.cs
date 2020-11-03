using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Queries;

namespace VoteMonitor.Api.Observer.Controllers
{
    [Route("api/v1/observer")]
    public class ObserverController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly DefaultNgoOptions _defaultNgoOptions;

        private int NgoId => this.GetIdOngOrDefault(_defaultNgoOptions.DefaultNgoId);
        private bool IsOrganizer => this.GetOrganizatorOrDefault(false);

        public ObserverController(IMediator mediator, IMapper mapper, IOptions<DefaultNgoOptions> defaultNgoOptions)
        {
            _mediator = mediator;
            _mapper = mapper;
            _defaultNgoOptions = defaultNgoOptions.Value;
        }

        [HttpGet]
        [Produces(type: typeof(ApiListResponse<ObserverModel>))]
        public async Task<ApiListResponse<ObserverModel>> GetObservers(ObserverListQuery query)
        {
            var command = _mapper.Map<ObserverListCommand>(query);

            command.IdNgo = IsOrganizer ? -1 : NgoId;

            var result = await _mediator.Send(command);
            return result;
        }

        [HttpGet]
        [Authorize("NgoAdmin")]
        [Produces(type: typeof(List<ObserverModel>))]
        [Route("active")]
        public async Task<List<ObserverModel>> GetActiveObservers(ActiveObserverFilter query)
        {
            var command = _mapper.Map<ActiveObserversQuery>(query);

            command.IdNgo = IsOrganizer ? -1 : NgoId;

            var result = await _mediator.Send(command);
            return result;
        }

        [HttpGet]
        [Produces(type: typeof(int))]
        [Route("count")]
        public async Task<int> GetTotalObserverCount()
        {
            var result = await _mediator.Send(new ObserverCountCommand(NgoId, IsOrganizer));
            return result;
        }

        [HttpPost]
        [Authorize("Organizer")]
        [Route("import")]
        [Produces(type: typeof(int))]
        public async Task<int> Import(IFormFile file, [FromForm] int ongId)
        {
            if (ongId <= 0)
            {
                ongId = NgoId;
            }

            var counter = await _mediator.Send(new ImportObserversRequest
            {
                File = file,
                IdOng = ongId
            });

            return counter;
        }

        /// <summary>
        ///  Adds an observer.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Boolean indicating whether or not the observer was added successfully.</returns>
        [HttpPost]
        [Authorize("Organizer")]
        [Produces(type: typeof(int))]
        public async Task<IActionResult> NewObserver(NewObserverModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newObsCommand = _mapper.Map<NewObserverCommand>(model);
            newObsCommand.IdNgo = NgoId;
            var newId = await _mediator.Send(newObsCommand);

            return Ok(newId);
        }

        /// <summary>
        /// Edits Observer information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Boolean indicating whether or not the observer was changed successfully</returns>
        [HttpPut]
        [Authorize("Organizer")]
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> EditObserver([FromBody] EditObserverModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(string.IsNullOrWhiteSpace(model.Name)
                && string.IsNullOrWhiteSpace(model.Phone)
                && string.IsNullOrWhiteSpace(model.Pin))
            {
                return BadRequest("Invalid request");
            }

            var isActionAllowed = await IsActionAllowed(model.IdObserver);
            if (!isActionAllowed)
            {
                return Problem("Action not allowed", statusCode: (int)HttpStatusCode.BadRequest);
            }

            var id = await _mediator.Send(_mapper.Map<EditObserverCommand>(model));

            return Ok(id > 0);
        }

        /// <summary>
        /// Deletes an observer.
        /// </summary>
        /// <param name="id">The Observer id</param>
        /// <returns>Boolean indicating whether or not the observer was deleted successfully</returns>
        [HttpDelete]
        [Authorize("Organizer")]
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> DeleteObserver(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isActionAllowed = await IsActionAllowed(id);
            if (!isActionAllowed)
            {
                return Problem("Action not allowed", statusCode: (int)HttpStatusCode.BadRequest);
            }

            var result = await _mediator.Send(new DeleteObserverCommand(id));

            return Ok(result);
        }

        private async Task<bool> IsActionAllowed(int? id)
        {
            if (IsOrganizer)
            {
                return true;
            }

            var observerRequest = new GetObserverDetails(NgoId, id.Value);

            var observer = await _mediator.Send(observerRequest);
            if (observer == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes mobile device Id associated with Observer of given Id.
        /// </summary>
        /// <param name="id">The Observer id</param>
        /// <returns>Boolean indicating whether or not the mobile device Id was removed successfully</returns>
        [HttpPost]
        [Route("removeDeviceId")]
        [Authorize("NgoAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveObserverDeviceId(int id)
        {
            var isActionAllowed = await IsActionAllowed(id);
            if (!isActionAllowed)
            {
                return Problem("Action not allowed", statusCode: (int)HttpStatusCode.BadRequest);
            }

            var request = _mapper.Map<RemoveDeviceIdCommand>(new RemoveDeviceIdModel { Id = id });
            await _mediator.Send(request);

            return Ok();
        }

        [HttpPost]
        [Route("reset")]
        [Authorize("Organizer")]
        public async Task<IActionResult> Reset([FromBody] ResetModel model)
        {
            if (string.IsNullOrEmpty(model.Action) || string.IsNullOrEmpty(model.PhoneNumber))
            {
                return BadRequest();
            }

            if (string.Equals(model.Action, ControllerExtensions.DEVICE_RESET))
            {
                var result = await _mediator.Send(new ResetDeviceCommand
                {
                    IdNgo = NgoId,
                    PhoneNumber = model.PhoneNumber,
                    Organizer = this.GetOrganizatorOrDefault(false)
                });
                if (result == -1)
                {
                    return NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + model.PhoneNumber);
                }
                else
                {
                    return Ok(result);
                }
            }

            if (string.Equals(model.Action, ControllerExtensions.PASSWORD_RESET))
            {
                var result = await _mediator.Send(new ResetPasswordCommand
                {
                    IdNgo = NgoId,
                    PhoneNumber = model.PhoneNumber,
                    Pin = model.Pin,
                    Organizer = this.GetOrganizatorOrDefault(false)
                });
                if (result == false)
                {
                    return NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + model.PhoneNumber);
                }

                return Ok();
            }

            return UnprocessableEntity();
        }

        [HttpPost]
        [Authorize("Organizer")]
        [Route("generate")]
        [Produces(type: typeof(List<GeneratedObserver>))]
        public async Task<IActionResult> GenerateObservers([FromForm] int count)
        {
            if (!ControllerExtensions.ValidateGenerateObserversNumber(count))
            {
                return BadRequest("Incorrect parameter supplied, please check that parameter is between boundaries: "
                    + ControllerExtensions.LOWER_OBS_VALUE + " - " + ControllerExtensions.UPPER_OBS_VALUE);
            }

            var command = new ObserverGenerateCommand(count, NgoId);

            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}