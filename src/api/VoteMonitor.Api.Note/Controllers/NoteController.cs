using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Note.Models;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Note.Commands;
using System.Collections.Generic;
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
        public async Task<List<NoteModel>> Get(NoteQuery filter) => await _mediator.Send(filter);

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
        /// <param name="file"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(List<IFormFile> files, [FromForm]NoteModel note)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // TODO[DH] use a pipeline instead of separate Send commands
            // daca nota este asociata sectiei
            var idSectie = await _mediator.Send(_mapper.Map<PollingStationQuery>(note));
            if (idSectie < 0)
                return NotFound();

            var command = _mapper.Map<AddNoteCommand>(note);
            var fileAddresses = await _mediator.Send(new UploadFileCommand { Files = files });

            command.IdObserver = int.Parse(User.Claims.First(c => c.Type == ClaimsHelper.ObserverIdProperty).Value);
            command.AttachementPaths = fileAddresses;
            command.IdPollingStation = idSectie;

            var result = await _mediator.Send(command);

            if (result < 0)
                return NotFound();

            return Ok(new { fileAddresses, note });
        }
    }
}