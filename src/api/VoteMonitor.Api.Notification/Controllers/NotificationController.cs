using Microsoft.Extensions.Configuration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Api.Notification.Models;

namespace VoteMonitor.Api.Notification.Controllers;

[Route("api/v1/notification")]
[ApiController]
public class NotificationController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationController> _logger;
    private readonly int _defaultIdOng;
    private readonly bool _defaultOrganizator;

    public NotificationController(IMediator mediator, ILogger<NotificationController> logger, IConfiguration configuration)
    {
        _mediator = mediator;
        _logger = logger;
        _defaultIdOng = configuration.GetValue<int>("DefaultIdOng");
        _defaultOrganizator = configuration.GetValue<bool>("DefaultOrganizator");
    }

    [HttpPost]
    [Route("register")]
    [Authorize("Observer")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> RegisterTokenAsync([FromBody] NotificationRegistrationDataModel request)
    {
        var observerId = request.ObserverId ?? this.GetIdObserver();

        await _mediator.Send(new NotificationRegistrationDataCommand(observerId, request.ChannelName, request.Token));

        _logger.LogInformation("Observer {observerId} registered for notifications", observerId);

        return Accepted();
    }

    [HttpPost]
    [Authorize("Organizer")]
    [Route("send")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Send([FromBody] SendNotificationModel request)
    {
        var command = new SendNotificationCommand(request.Recipients, this.GetNgoAdminId(), request.Channel, request.From, request.Title, request.Message);

        var result = await _mediator.Send(command);

        return Accepted(result);
    }

    [HttpPost]
    [Authorize("Organizer")]
    [Route("send/all")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SendToAll([FromBody] SendNotificationToAllModel request)
    {
        var command = new SendNotificationToAllCommand(this.GetNgoAdminId(), request.Channel, request.From, request.Title, request.Message);

        var result = await _mediator.Send(command);

        return Accepted(result);
    }

    [HttpGet]
    [Authorize("Organizer")]
    [Route("get/all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] PagingModel model)
    {

        var ngoId = this.GetIdOngOrDefault(_defaultIdOng);
        var isOrganizer = this.GetOrganizatorOrDefault(false);
        if (!isOrganizer && ngoId == _defaultIdOng)
            return BadRequest();

        var query = new NotificationListCommand
        {
            NgoId = ngoId != _defaultIdOng ? ngoId : (int?)null,
            IsOrganizer = isOrganizer,
            Page = model.Page,
            PageSize = model.PageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
