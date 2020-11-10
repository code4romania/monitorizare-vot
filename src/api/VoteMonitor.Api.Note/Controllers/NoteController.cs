using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [Produces(type: typeof(List<NoteModel>))]
        public async Task<IActionResult> GetNotes(NoteQuery filter)
        {
            if (filter.IdQuestion.HasValue && !filter.IdPollingStation.HasValue)
                return BadRequest($"If the {nameof(filter.IdQuestion)} param is provided then the {nameof(filter.IdPollingStation)} param is required !");

            if (!filter.IdObserver.HasValue)
            {
                filter.IdObserver = this.GetIdObserver();
            }

            return Ok(await _mediator.Send(filter));
        }

        [HttpPost]
        [Authorize("Observer")]
        [Produces(type: typeof(UploadNoteResultV2))]
        public async Task<IActionResult> Upload([FromForm] UploadNoteModelV2 note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO[DH] use a pipeline instead of separate Send commands
            // daca nota este asociata sectiei
            var pollingStationId = await _mediator.Send(_mapper.Map<PollingStationQuery>(note));

            if (pollingStationId < 0)
            {
                return NotFound();
            }

            var command = _mapper.Map<AddNoteCommandV2>(note);

            command.IdObserver = this.GetIdObserver();
            command.IdPollingStation = pollingStationId;

            if (note.Files != null && note.Files.Any())
            {
                var files = await _mediator.Send(new UploadFileCommandV2 { Files = note.Files, UploadType = UploadType.Notes });
                command.AttachmentPaths = files;
            }

            var result = await _mediator.Send(command);

            if (result < 0)
            {
                return NotFound();
            }

            return Ok(new UploadNoteResultV2 { FilesAddress = command.AttachmentPaths, Note = note });
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
        [Obsolete("Will be removed when ui will use multiple files upload")]
        [Produces(type: typeof(UploadNoteResult))]
        public async Task<IActionResult> UploadOld([FromForm] UploadNoteModel note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO[DH] use a pipeline instead of separate Send commands
            // daca nota este asociata sectiei
            var idSectie = await _mediator.Send(_mapper.Map<PollingStationQuery>(note));

            if (idSectie < 0)
            {
                return NotFound();
            }

            var command = _mapper.Map<AddNoteCommand>(note);

            command.IdObserver = int.Parse(User.Claims.First(c => c.Type == ClaimsHelper.ObserverIdProperty).Value);
            command.IdPollingStation = idSectie;

            if (note.File != null)
            {
                var fileAddress = await _mediator.Send(new UploadFileCommand { File = note.File, UploadType = UploadType.Notes });
                command.AttachementPath = fileAddress;
            }

            var result = await _mediator.Send(command);

            if (result < 0)
            {
                return NotFound();
            }

            return Ok(new UploadNoteResult{ FileAddress = command.AttachementPath, Note= note });
        }
    }
}