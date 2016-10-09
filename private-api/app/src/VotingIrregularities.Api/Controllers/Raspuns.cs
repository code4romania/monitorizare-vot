using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VotingIrregularities.Api.Extensions;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.RaspunsAggregate.Commands;

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

        public Raspuns(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Aici se inregistreaza raspunsul dat de observator la una sau mai multe intrebari, pentru o sectie de votare.
        /// Raspunsul (ModelOptiuniSelectate) poate avea mai multe optiuni (IdOptiune) si potential un text (Value).
        /// </summary>
        /// <param name="raspuns">Sectia de votare, lista de optiuni si textul asociat unei optiuni care se completeaza cand 
        /// optiunea <code>SeIntroduceText = true</code></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IAsyncResult> CompleteazaRaspuns([FromBody] ModelRaspunsBulk[] raspuns)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest);

            // TODO[DH] use a pipeline instead of separate Send commands
            var command = await _mediator.SendAsync(new RaspunsuriBulk(raspuns));

            // TODO[DH] get the actual IdObservator from token
            command.IdObservator = 1;

            var result = await _mediator.SendAsync(command);

            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }
    }
}
