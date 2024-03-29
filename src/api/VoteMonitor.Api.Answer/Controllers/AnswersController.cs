using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Api.Answer.Queries;
using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Answer.Controllers;

[ApiController]
[Route("api/v1/answers")]
[Authorize]
public class AnswersController : Controller
{
    private readonly IMediator _mediator;
    private readonly bool _defaultOrganizator;
    private readonly int _defaultIdOng;

    public AnswersController(IMediator mediator, IConfiguration configuration)
    {
        _mediator = mediator;

        _defaultOrganizator = configuration.GetValue<bool>("DefaultOrganizator");
        _defaultIdOng = configuration.GetValue<int>("DefaultIdOng");
    }


    /// <summary>
    /// Returns a list of polling stations where observers from the given NGO have submitted answers
    /// to the questions marked as IsFlagged=IsUrgent, ordered by ModifiedDate descending
    /// </summary>
    /// <param name="model">SectionAnswersRequest</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiListResponse<AnswerQueryDto>> Get([FromQuery] SectionAnswersRequest model)
    {
        var organizer = this.GetOrganizatorOrDefault(_defaultOrganizator);
        var ngoId = this.GetIdOngOrDefault(_defaultIdOng);

        return await _mediator.Send(new AnswersQuery
        {
            NgoId = ngoId,
            Organizer = organizer,
            Page = model.Page,
            PageSize = model.PageSize,
            Urgent = model.IsUrgent,
            County = model.County,
            PollingStationNumber = model.PollingStationNumber,
            ObserverId = model.ObserverId,
            ObserverPhoneNumber = model.ObserverPhoneNumber
        });
    }

    /// <summary>
    /// Returns answers given by the specified observer at the specified polling station
    /// </summary>
    [HttpGet("filledIn")]
    public async Task<List<QuestionDto<FilledInAnswerDto>>> Get([FromQuery] int pollingStationId, [FromQuery] int observerId)
    {
        return await _mediator.Send(new FilledInAnswersQuery(observerId, pollingStationId));
    }

    /// <summary>
    /// Returns the polling station information filled in by the given observer at the given polling station
    /// </summary>
    /// <param name="model"> "PollingStationId" - Id of the given polling station
    /// "ObserverId" - Id of the observer
    /// </param>
    [HttpGet("pollingStationInfo")]
    public async Task<PollingStationInfoDto> GetObserverAnswers([FromQuery] ObserverAnswersRequest model)
    {
        return await _mediator.Send(new FormAnswersQuery(model.ObserverId, model.PollingStationId));
    }

    /// <summary>
    /// Saves the answers to one or more questions, at a given polling station
    /// An answer can have multiple options (OptionId) and potentially a free text (Value).
    /// </summary>
    /// <param name="answerModel">Polling station, list of options and the associated text of an option when
    /// <code>IsFreeText = true</code></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize("Observer")]
    public async Task<IActionResult> PostAnswer([FromBody] BulkAnswersRequest answerModel)
    {
        // TODO[DH] use a pipeline instead of separate Send commands
        var command = await _mediator.Send(new Commands.BulkAnswers(this.GetIdObserver(), answerModel.Answers));

        var result = await _mediator.Send(command);

        if (result < 0)
        {
            return NotFound();
        }

        return Ok();
    }
}
