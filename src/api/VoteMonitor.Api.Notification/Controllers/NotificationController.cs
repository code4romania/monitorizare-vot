using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Notification.Models;
using VoteMonitor.Api.Observer.Commands;
using VotingIrregularities.Api.Options;
using Microsoft.Extensions.Configuration;

namespace VoteMonitor.Api.Notification.Controllers
{
    [Route("api/v1/notification")]
    public class NotificationController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly FirebaseServiceOptions _firebaseOptions;
        private readonly IConfigurationRoot _configuration;

        public NotificationController(IMediator mediator, ILogger logger, IMapper mapper, IConfigurationRoot configuration)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;

            _configuration = configuration;
            _firebaseOptions = new FirebaseServiceOptions();
            _configuration.GetSection(nameof(FirebaseServiceOptions)).Bind(_firebaseOptions);
        }

        [HttpPost]
        [Route("register")]
        public async Task<dynamic> registerTokenAsync(NotificationRegDataModel tokenRegistrationModel)
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
        public async Task<dynamic> send([FromBody]NotificationNewModel newNotificationModel)
        {
            var result = await _mediator.Send(_mapper.Map<NewNotificationCommand>(newNotificationModel));

            return Task.FromResult(result);
        }
    }
}