﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Note.Commands;
using VoteMonitor.Api.Note.Models;
using VoteMonitor.Api.Note.Queries;

namespace VoteMonitor.Api.Note.Controllers
{
    [Route("api/v2/note")]
    public class NoteController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public NoteController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> Get(NoteQuery filter)
        {
            if (filter.IdQuestion.HasValue && !filter.IdPollingStation.HasValue)
                return BadRequest($"If the {nameof(filter.IdQuestion)} param is provided then the {nameof(filter.IdPollingStation)} param is required !");

            if (!filter.IdObserver.HasValue)
            {
                filter.IdObserver = this.GetIdObserver();
            }

            return Ok(await _mediator.Send(filter));
        }
        /// <summary>
        /// Aceasta ruta este folosita cand observatorul incarca o imagine sau un clip in cadrul unei note.
        /// Fisierului atasat i se da contenttype = Content-Type: multipart/form-data
        /// Celalalte proprietati sunt de tip form-data
        /// CodJudet:BU 
        /// NumarSectie:3243
        /// IdIntrebare: 201
        /// TextNota: "asdfasdasdasdas"
        /// API-ul va returna adresa publica a fisierului unde este salvat si obiectul trimis prin formdata
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        [Authorize("Observer")]
        public async Task<dynamic> Upload([FromForm] UploadNoteModel note)
        {
            if (!ModelState.IsValid)
            {
                return this.ResultAsync(HttpStatusCode.BadRequest);
            }

            // TODO[DH] use a pipeline instead of separate Send commands
            // daca nota este asociata sectiei
            var pollingStationId = await _mediator.Send(_mapper.Map<PollingStationQuery>(note));

            if (pollingStationId < 0)
            {
                return this.ResultAsync(HttpStatusCode.NotFound);
            }

            var command = _mapper.Map<AddNoteCommand>(note);

            command.IdObserver = this.GetIdObserver();
            command.IdPollingStation = pollingStationId;

            if (note.Files != null && note.Files.Any())
            {
                var files = await _mediator.Send(new UploadFileCommand { Files = note.Files, UploadType = UploadType.Notes });
                command.AttachementPaths = files;
            }

            var result = await _mediator.Send(command);

            if (result < 0)
            {
                return this.ResultAsync(HttpStatusCode.NotFound);
            }

            return await Task.FromResult(new { FilesAddress = command.AttachementPaths, note });
        }
    }
}