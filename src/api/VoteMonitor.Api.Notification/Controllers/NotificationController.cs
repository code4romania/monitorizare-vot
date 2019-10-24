using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core;
using AutoMapper;
using VoteMonitor.Api.Notification.Models;
using VotingIrregularities.Api.Options;
using Microsoft.Extensions.Configuration;
using VoteMonitor.Api.Notification.Commands;

namespace VoteMonitor.Api.Notification.Controllers
{
    [Route("api/v1/notification")]
    public class NotificationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public NotificationController(IMediator mediator, ILogger logger, IMapper mapper, IConfigurationRoot configuration)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;

            var firebaseOptions = new FirebaseServiceOptions();
            configuration.GetSection(nameof(FirebaseServiceOptions)).Bind(firebaseOptions);
        }

        [HttpPost]
        [Route("register")]
        public async Task<dynamic> RegisterTokenAsync(NotificationRegDataModel tokenRegistrationModel)
        {
            if(!tokenRegistrationModel.ObserverId.HasValue)
            {
                tokenRegistrationModel.ObserverId = this.GetIdObserver();
            }

            await _mediator.Send(_mapper.Map<NotificationRegDataCommand>(tokenRegistrationModel));

            return Task.FromResult(new {});
        }

        [HttpPost]
        [Route("send")]
        public async Task<dynamic> Send([FromBody]NotificationNewModel newNotificationModel)
        {
            var result = await _mediator.Send(_mapper.Map<NewNotificationCommand>(newNotificationModel));

            return Task.FromResult(result);
        }
    }
}