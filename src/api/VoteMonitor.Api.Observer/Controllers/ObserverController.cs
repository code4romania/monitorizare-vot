﻿using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.Core;
using AutoMapper;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Commands;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Observer.Queries;
using Microsoft.Extensions.Options;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Observer.Controllers
{
    [Route("api/v1/observer")]
    public class ObserverController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly DefaultNgoOptions _defaultNgoOptions;

        private int NgoId => this.GetIdOngOrDefault(_defaultNgoOptions.DefaultNgoId);

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

            var organizer = this.GetOrganizatorOrDefault(false);
            command.IdNgo = organizer ? -1 : NgoId;

            var result = await _mediator.Send(command);
            return result;
        }

        [HttpGet]
        [Produces(type: typeof(List<ObserverModel>))]
        [Route("active")]
        public async Task<List<ObserverModel>> GetActiveObservers(ActiveObserverFilter query)
        {
            var command = _mapper.Map<ActiveObserversQuery>(query);

            var organizer = this.GetOrganizatorOrDefault(false);
            command.IdNgo = organizer ? -1 : NgoId;

            var result = await _mediator.Send(command);
            return result;
        }

        [HttpGet]
        [Produces(type: typeof(int))]
        [Route("count")]
        public async Task<int> GetTotalObserverCount()
        {
            var result = await _mediator.Send(new ObserverCountCommand { IdNgo = NgoId });
            return result;
        }

        [HttpPost]
        [Route("import")]
        [Produces(type: typeof(int))]
        public async Task<int> Import(IFormFile file, [FromForm] int ongId)
        {
            if (ongId <= 0)
                ongId = NgoId;

            await _mediator.Send(
                new UploadFileCommand
                {
                    File = file,
                    UploadType = UploadType.Observers
                });

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
        [Produces(type: typeof(int))]
        public async Task<IActionResult> NewObserver(NewObserverModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> EditObserver([FromBody]EditObserverModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _mediator.Send(_mapper.Map<EditObserverCommand>(model));

            return Ok(id > 0);
        }

        /// <summary>
        /// Deletes an observer.
        /// </summary>
        /// <param name="id">The Observer id</param>
        /// <returns>Boolean indicating whether or not the observer was deleted successfully</returns>
        [HttpDelete]
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> DeleteObserver(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(_mapper.Map<DeleteObserverCommand>(new DeleteObserverModel { IdObserver = id }));

            return Ok(result);
        }

        [HttpPost]
        [Route("reset")]
        public async Task<IActionResult> Reset([FromBody]ResetModel model)
        {
            if (string.IsNullOrEmpty(model.Action) || string.IsNullOrEmpty(model.PhoneNumber))
                return BadRequest();

            if (string.Equals(model.Action, ControllerExtensions.DEVICE_RESET))
            {
                var result = await _mediator.Send(new ResetDeviceCommand(NgoId, model.PhoneNumber));
                if (result == -1)
                    return NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + model.PhoneNumber);
                else
                    return Ok(result);
            }

            if (string.Equals(model.Action, ControllerExtensions.PASSWORD_RESET))
            {
                var result = await _mediator.Send(new ResetPasswordCommand(NgoId, model.PhoneNumber, model.Pin));
                if (result == false)
                    return NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + model.PhoneNumber);

                return Ok();
            }

            return UnprocessableEntity();
        }

        [HttpPost]
        [Route("generate")]
        [Produces(type: typeof(List<GeneratedObserver>))]
        public async Task<IActionResult> GenerateObservers([FromForm] int count)
        {
            if (!ControllerExtensions.ValidateGenerateObserversNumber(count))
                return BadRequest("Incorrect parameter supplied, please check that paramter is between boundaries: "
                    + ControllerExtensions.LOWER_OBS_VALUE + " - " + ControllerExtensions.UPPER_OBS_VALUE);

            var command = new ObserverGenerateCommand(count, NgoId);

            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}