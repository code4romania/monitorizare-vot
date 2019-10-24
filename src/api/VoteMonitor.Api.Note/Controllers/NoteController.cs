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
using VoteMonitor.Api.Core.Commands;

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
        public async Task<List<NoteModel>> Get(NoteQuery filter)
        {
            if (!filter.IdObserver.HasValue)
                filter.IdObserver = this.GetIdObserver();

            return await _mediator.Send(filter);
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
        /// <param name="file"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<dynamic> Upload(IFormFile file, [FromForm]UploadNoteModel note)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest);

            // TODO[DH] use a pipeline instead of separate Send commands
            // daca nota este asociata sectiei
            var idSectie = await _mediator.Send(_mapper.Map<PollingStationQuery>(note));
            if (idSectie < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            var command = _mapper.Map<AddNoteCommand>(note);
            var fileAddress = await _mediator.Send(new UploadFileCommand { File = file, UploadType = UploadType.Notes });

            command.IdObserver = int.Parse(User.Claims.First(c => c.Type == ClaimsHelper.ObserverIdProperty).Value);
            command.AttachementPath = fileAddress;
            command.IdPollingStation = idSectie;

            var result = await _mediator.Send(command);

            if (result < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            return await Task.FromResult(new { FileAdress = fileAddress, note });
        }
    }
}