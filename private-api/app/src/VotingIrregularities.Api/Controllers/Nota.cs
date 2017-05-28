using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Extensions;
using VotingIrregularities.Api.Models;
using AutoMapper;
using VotingIrregularities.Domain.NotaAggregate;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/note")]
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
        /// <returns></returns>
        [HttpPost("ataseaza")]
        public async Task<dynamic> Upload(IFormFile file, [FromForm]ModelNota nota)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest);

            // TODO[DH] use a pipeline instead of separate Send commands
            // daca nota este asociata sectiei
            int idSectie = await _mediator.Send(_mapper.Map<ModelSectieQuery>(nota));
            if (idSectie < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            var command = _mapper.Map<AdaugaNotaCommand>(nota);
            var fileAddress = await _mediator.Send(new ModelFile { File = file });

            // TODO[DH] get the actual IdObservator from token
            command.IdObservator = int.Parse(User.Claims.First(c => c.Type == "IdObservator").Value);
            command.CaleFisierAtasat = fileAddress;
            command.IdSectieDeVotare = idSectie;

            var result = await _mediator.Send(command);

            if (result < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            return await Task.FromResult(new { FileAdress = fileAddress, nota = nota });
        }
    }
}
