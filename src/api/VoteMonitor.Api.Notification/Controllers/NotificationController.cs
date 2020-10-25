using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Api.Notification.Models;

namespace VoteMonitor.Api.Notification.Controllers
{
    [Route("api/v1/notification")]
    public class NotificationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationController> _logger;
        private readonly IMapper _mapper;

        public NotificationController(IMediator mediator, ILogger<NotificationController> logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
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
    }
}