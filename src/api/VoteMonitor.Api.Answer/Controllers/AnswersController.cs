using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Api.Answer.Queries;
using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Answer.Controllers
{
    [Route("api/v1/answers")]
    public class AnswersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public AnswersController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Returns a list of polling stations where observers from the given NGO have submitted answers
        /// to the questions marked as IsFlagged=IsUrgent, ordered by ModifiedDate descending
        /// </summary>
        /// <param name="model">SectionAnswersRequest</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiListResponse<AnswerQueryDto>> Get(SectionAnswersRequest model)
        {
            var organizer = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));
            var ngoId = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));

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
        public async Task<List<QuestionDto<FilledInAnswerDto>>> Get(int pollingStationId, int observerId)
        {
            return await _mediator.Send(new FilledInAnswersQuery
            {
                ObserverId = observerId,
                PollingStationId = pollingStationId
            });
        }

        /// <summary>
        /// Returns the polling station information filled in by the given observer at the given polling station
        /// </summary>
        /// <param name="model"> "PollingStationId" - Id of the given polling station
        /// "ObserverId" - Id of the observer
        /// </param>
        [HttpGet("pollingStationInfo")]
        public async Task<PollingStationInfoDto> GetObserverAnswers(ObserverAnswersRequest model)
        {
            return await _mediator.Send(new FormAnswersQuery
            {
                ObserverId = model.ObserverId,
                PollingStationId = model.PollingStationNumber
            });
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO[DH] use a pipeline instead of separate Send commands
            var command = await _mediator.Send(new Commands.BulkAnswers(answerModel.Answers));

            command.ObserverId = this.GetIdObserver();

            var result = await _mediator.Send(command);

            if (result < 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
