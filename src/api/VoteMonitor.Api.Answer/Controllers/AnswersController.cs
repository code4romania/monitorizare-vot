﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VoteMonitor.Api.Answer.Commands;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Api.Answer.Queries;
using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Answer.Controllers {
    [Route("api/v1/answers")]
    public class AnswersController : Controller {
        private readonly IMediator _mediator;
        private readonly IConfigurationRoot _configuration;

        public AnswersController(IMediator mediator, IConfigurationRoot configuration) {
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Returns a list of polling stations where observers from the given NGO have submitted answers
        /// to the questions marked as Flagged=Urgent, ordered by ModifiedDate descending
        /// </summary>
        /// <param name="model"> Detaliile de paginare (default Page=1, PageSize=20)
        /// Urgent (valoarea campului RaspunsFlag)
        /// </param>
        [HttpGet]
        public async Task<ApiListResponse<AnswerQueryDTO>> Get(SectionAnswersRequest model) {
            var organizator = this.GetOrganizatorOrDefault(_configuration.GetValue<bool>("DefaultOrganizator"));
            var idOng = this.GetIdOngOrDefault(_configuration.GetValue<int>("DefaultIdOng"));

            return await _mediator.Send(new AnswersQuery {
                IdONG = idOng,
                Organizer = organizator,
                Page = model.Page,
                PageSize = model.PageSize,
                Urgent = model.Urgent,
                County = model.County,
                PollingStationNumber = model.PollingStationNumber,
                ObserverId = model.ObserverId
            });
        }

        /// <summary>
        /// Returns answers given by the specified observer at the specified polling station
        /// </summary>
        /// <param name="model">
        /// "IdSectieDeVotare" - Id-ul sectiei unde s-au completat raspunsurile
        /// "IdObservator" - Id-ul observatorului care a dat raspunsurile
        /// </param>
        [HttpGet("filledIn")]
        public async Task<List<QuestionDTO<FilledInAnswerDTO>>> Get(ObserverAnswersRequest model) {
            return await _mediator.Send(new FilledInAnswersQuery {
                ObserverId = model.ObserverId,
                PollingStationId = model.PollingStationNumber
            });
        }

        /// <summary>
        /// Returns the polling station information filled in by the given observer at the given polling station
        /// </summary>
        /// <param name="model"> "IdSectieDeVotare" - Id-ul sectiei unde s-au completat raspunsurile
        /// "IdObservator" - Id-ul observatorului care a dat raspunsurile
        /// </param>
        [HttpGet("pollingStationInfo")]
        public async Task<PollingStationInfosDTO> GetRaspunsuriFormular(ObserverAnswersRequest model) {
            return await _mediator.Send(new FormAnswersQuery {
                ObserverId = model.ObserverId,
                PollingStationId = model.PollingStationNumber
            });
        }


        /// <summary>
        /// Aici se inregistreaza raspunsul dat de observator la una sau mai multe intrebari, pentru o sectie de votare.
        /// Raspunsul (ModelOptiuniSelectate) poate avea mai multe optiuni (IdOptiune) si potential un text (Value).
        /// </summary>
        /// <param name="raspuns">Sectia de votare, lista de optiuni si textul asociat unei optiuni care se completeaza cand 
        /// optiunea <code>SeIntroduceText = true</code></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize("Observer")]
        public async Task<IActionResult> PostAnswer([FromBody] AnswerModelWrapper raspuns) {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            // TODO[DH] use a pipeline instead of separate Send commands
            var command = await _mediator.Send(new BulkAnswers(raspuns.Answers));

            command.ObserverId = this.GetIdObserver();

            var result = await _mediator.Send(command);

            if (result < 0) return NotFound();

            return Ok();
        }
    }
}