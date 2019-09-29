using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Controllers
{
    /// <summary>
    /// Ruta unde se inregistreaza raspunsurile
    /// </summary>
    [Route("/api/v1/raspuns")]
    public class Raspuns : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public Raspuns(IMediator mediator, IMapper mapper, ILogger logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Aici se inregistreaza raspunsul dat de observator la una sau mai multe intrebari, pentru o sectie de votare.
        /// Raspunsul (ModelOptiuniSelectate) poate avea mai multe optiuni (IdOptiune) si potential un text (Value).
        /// </summary>
        /// <param name="raspuns">Sectia de votare, lista de optiuni si textul asociat unei optiuni care se completeaza cand 
        /// optiunea <code>SeIntroduceText = true</code></param>
        /// <returns></returns>
        [HttpPost]
        [Obsolete("use /answers instead")]
        public async Task<IAsyncResult> CompleteazaRaspuns([FromBody] ModelRaspunsWrapper raspuns)
        {

            if (!ModelState.IsValid)
            {
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);
            }

            // TODO[DH] use a pipeline instead of separate Send commands
            var command = await _mediator.Send(new RaspunsuriBulk(raspuns.Raspuns));

            // TODO[DH] get the actual IdObservator from token
            command.IdObservator = int.Parse(User.Claims.First(c => c.Type == ClaimsHelper.ObserverIdProperty).Value);

            var result = await _mediator.Send(command);

            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }
    }
}
