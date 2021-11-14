using AutoMapper;
using Microsoft.Extensions.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Api.Notification.Models;
using VoteMonitor.Api.Notification.Queries;

namespace VoteMonitor.Api.Notification.Controllers
{
    [Route("api/v1/notification")]
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public NotificationController(IMediator mediator, ILogger<NotificationController> logger, IMapper mapper, IConfiguration configuration)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        [Authorize("Observer")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> RegisterTokenAsync(NotificationRegistrationDataModel tokenRegistrationModel)
        {
            if (!tokenRegistrationModel.ObserverId.HasValue)
            {
                tokenRegistrationModel.ObserverId = this.GetIdObserver();
            }

            await _mediator.Send(_mapper.Map<NotificationRegistrationDataCommand>(tokenRegistrationModel));

            _logger.LogInformation($"Observer {tokenRegistrationModel.ObserverId} registered for notifications");

            return Accepted();
        }

        [HttpPost]
        [Authorize("Organizer")]
        [Route("send")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Send([FromBody] SendNotificationModel sendNotificationModel)
        {
            var command = _mapper.Map<SendNotificationCommand>(sendNotificationModel);
            command.SenderAdminId = this.GetNgoAdminId();

            var result = await _mediator.Send(command);

            return Accepted(result);
        }

        [HttpPost]
        [Authorize("Organizer")]
        [Route("send/all")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> SendToAll([FromBody] SendNotificationToAllModel model)
        {
            var command = _mapper.Map<SendNotificationToAllCommand>(model);
            command.SenderAdminId = this.GetNgoAdminId();

            var result = await _mediator.Send(command);

            return Accepted(result);
        }

        [HttpGet]
        [Authorize("NgoAdmin")]
        [Route("get/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery]PagingModel model)
        {
            var ngoId = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var isOrganizer = this.GetOrganizatorOrDefault(false);
            if (!isOrganizer && ngoId == _configuration.GetValue<int>("DefaultIdOng"))
                return BadRequest();

            var query = new NotificationListQuery
            {
                NgoId = ngoId != _configuration.GetValue<int>("DefaultIdOng") ? ngoId : (int?)null,
                IsOrganizer = isOrganizer,
                Page = model.Page,
                PageSize = model.PageSize
            };
            var command = _mapper.Map<NotificationListCommand>(query);

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
