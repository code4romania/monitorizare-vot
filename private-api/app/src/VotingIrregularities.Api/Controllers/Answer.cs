using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Api.Extensions;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Controllers
{
    /// <summary>
    /// Ruta unde se inregistreaza raspunsurile
    /// </summary>
    [Route("/api/v1/answer")]
    public class Answer : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public Answer(IMediator mediator, IMapper mapper, ILogger logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Aici se inregistreaza raspunsul dat de observator la una sau mai multe intrebari, pentru o sectie de votare.
        /// Raspunsul (SelectedOptionsModel) poate avea mai multe optiuni (OptionId) si potential un text (Value).
        /// </summary>
        /// <param name="answer">Sectia de votare, lista de optiuni si textul asociat unei optiuni care se completeaza cand 
        /// optiunea <code>TextMustBeInserted = true</code></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IAsyncResult> SendAnswer([FromBody] AnswerModelWrapper answer)
        {

            if (!ModelState.IsValid)
            {
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);
            }

            // TODO[DH] use a pipeline instead of separate Send commands
            var command = await _mediator.Send(new AnswersBulk(answer.Answer));

            // TODO[DH] get the actual ObserverId from token
            command.ObserverId = int.Parse(User.Claims.First(c => c.Type == "ObserverId").Value);

            var result = await _mediator.Send(command);

            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }
    }
}
