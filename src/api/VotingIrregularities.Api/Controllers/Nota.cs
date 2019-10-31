using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Models;
using AutoMapper;
using VoteMonitor.Api.Core;
using VotingIrregularities.Domain.NotaAggregate;
using System;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Core.Commands;
using System.Collections.Generic;
using System.Linq;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/note")]
    [Obsolete("use api/v2/note")]

    public class Nota : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public Nota(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
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
        /// <param name="nota"></param>
        /// <returns></returns>
        [HttpPost("ataseaza"),]
        public async Task<dynamic> Upload(IFormFile file, [FromForm]ModelNota nota)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest);

            // TODO[DH] use a pipeline instead of separate Send commands
            // daca nota este asociata sectiei
            var idSectie = await _mediator.Send(_mapper.Map<ModelSectieQuery>(nota));
            if (idSectie < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            var command = _mapper.Map<AdaugaNotaCommand>(nota);
            var uploadCommand = new UploadFileCommand() { Files = new List<IFormFile>() { file }, UploadType = UploadType.Notes };
            var fileAddress = await _mediator.Send(uploadCommand);

            command.IdObservator = this.GetIdObserver();
            command.CaleFisierAtasat = fileAddress.First();
            command.IdSectieDeVotare = idSectie;

            var result = await _mediator.Send(command);

            if (result < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            return await Task.FromResult(new { FileAdress = fileAddress, nota });
        }
    }
}
