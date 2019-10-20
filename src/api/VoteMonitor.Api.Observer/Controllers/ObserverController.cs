using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core;
using AutoMapper;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Commands;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Observer.Queries;
using Microsoft.Extensions.Configuration;
using VoteMonitor.Api.Core.Commands;

namespace VoteMonitor.Api.Observer.Controllers {
    [Route("api/v1/observer")]
    public class ObserverController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ObserverController(IMediator mediator, ILogger logger, IMapper mapper, IConfigurationRoot configuration)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        [Produces(type: typeof(List<ObserverModel>))]
        public async Task<ApiListResponse<ObserverModel>> GetObservers(ObserverListQuery query)
        {
            var ongId = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            
            var command = _mapper.Map<ObserverListCommand>(query);
            command.IdNgo = ongId;
            
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpPost]
        [Route("import")]
        public async Task<int> Import(IFormFile file, [FromForm] int ongId)
        {
            if (ongId <= 0) {
                ongId = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            }

            var fileAddress = await _mediator.Send(
                new UploadFileCommand { 
                    File = file, 
                    UploadType = UploadType.Observers 
                });
            
            var counter = await _mediator.Send(new ImportObserversRequest { 
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
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> NewObserver(NewObserverModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newObsCommand = _mapper.Map<NewObserverCommand>(model);
            newObsCommand.IdNgo = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
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
        public async Task<IActionResult> EditObserver([FromBody]EditObserverModel model) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _mediator.Send(_mapper.Map<EditObserverCommand>(model));

            return Ok(id > 0);
        }

        /// <summary>
        /// Deletes an observer.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Boolean indicating whether or not the observer was deleted successfully</returns>
        [HttpDelete]
        [Produces(type: typeof(bool))]
        public async Task<IActionResult> DeleteObserver(int id) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(_mapper.Map<DeleteObserverCommand>(new DeleteObserverModel { IdObserver = id }));

            return Ok(result);
        }


        [HttpPost]
        [Route("reset")]
        public async Task<IAsyncResult> Reset([FromForm] string action, [FromForm] string phoneNumber)
        {
            if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(phoneNumber))
                return Task.FromResult(BadRequest());

            if (string.Equals(action, ControllerExtensions.DEVICE_RESET))
            {
                var result = await _mediator.Send(new ResetDeviceCommand(this.GetIdOngOrDefault(0), phoneNumber));
                if (result == -1)
                    return Task.FromResult(NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + phoneNumber));
                else
                    return Task.FromResult(Ok(result));
            }

            if (string.Equals(action, ControllerExtensions.PASSWORD_RESET))
            {
                var result = await _mediator.Send(new ResetPasswordCommand(this.GetIdOngOrDefault(0), phoneNumber));
                if (string.IsNullOrEmpty(result))
                    return Task.FromResult(NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + phoneNumber));
                else
                    return Task.FromResult(Ok(result));
            }

            return Task.FromResult(UnprocessableEntity());
        }

        [HttpPost]
        [Route("generate")]
        public async Task<IAsyncResult> GenerateObservers([FromForm] int count)
        {
            if (!ControllerExtensions.ValidateGenerateObserversNumber(count))
                return Task.FromResult(new BadRequestObjectResult("Incorrect parameter supplied, please check that paramter is between boundaries: "
                    + ControllerExtensions.LOWER_OBS_VALUE + " - " + ControllerExtensions.UPPER_OBS_VALUE));

            ObserverGenerateCommand command = new ObserverGenerateCommand(count,
                ControllerExtensions.GetIdOngOrDefault(this, 0));

            var result = await _mediator.Send(command);

            return Task.FromResult(Ok(result));
        }
    }
}
