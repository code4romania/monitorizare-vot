using AutoMapper;
using Microsoft.Extensions.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Api.Notification.Models;
using VoteMonitor.Api.Notification.Queries;

namespace VoteMonitor.Api.Notification.Controllers
{
    [Route("api/v1/notification")]
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
        public async Task<dynamic> RegisterTokenAsync(NotificationRegistrationDataModel tokenRegistrationModel)
        {
            if (!tokenRegistrationModel.ObserverId.HasValue)
            {
                tokenRegistrationModel.ObserverId = this.GetIdObserver();
            }

            await _mediator.Send(_mapper.Map<NotificationRegistrationDataCommand>(tokenRegistrationModel));

            _logger.LogInformation($"Observer {tokenRegistrationModel.ObserverId} registered for notifications");

            return Task.FromResult(new { });
        }

        [HttpPost]
        [Authorize("Organizer")]
        [Route("send")]
        public async Task<dynamic> Send([FromBody]NotificationNewModel newNotificationModel)
        {
            NewNotificationCommand command = _mapper.Map<NewNotificationCommand>(newNotificationModel);
            command.SenderAdminId = this.GetNgoAdminId();
            var result = await _mediator.Send(command);

            return Task.FromResult(result);
        }

        [HttpPost]
        [Authorize("Organizer")]
        [Route("send/all")]
        public async Task<dynamic> SendToAll([FromBody]NotificationForAllNewModel model)
        {
            var command = new SendNotificationToAll(this.GetNgoAdminId(), model.Channel, model.From, model.Title, model.Message);

            var result = await _mediator.Send(command);

            return Task.FromResult(result);
        }

        [HttpGet]
        [Authorize("NgoAdmin")]
        [Route("get/all")]
        public async Task<IActionResult> GetAll(PagingModel query)
        {
            var idNgo = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));
            var organizer = this.GetOrganizatorOrDefault(false);
            if (!organizer && idNgo == _configuration.GetValue<int>("DefaultIdOng"))
                return BadRequest();

            var command = _mapper.Map<NotificationListCommand>(new NotificationListQuery {
                IdNgo = idNgo != _configuration.GetValue<int>("DefaultIdOng") ? idNgo : (int?)null,
                Organizer = organizer,
                Page = query.Page,
                PageSize = query.PageSize
            });

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}