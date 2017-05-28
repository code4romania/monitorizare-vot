using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VotingIrregularities.Api.Extensions;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.SectieAggregate;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/sectie")]
    public class Sectie : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public Sectie(IMediator mediator, IMapper mapper)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// Se apeleaza aceast metoda cand observatorul salveaza informatiile legate de ora sosirii. ora plecarii, zona urbana, info despre presedintele BESV.
        /// Aceste informatii sunt insotite de id-ul sectiei de votare.
        /// </summary>
        /// <param name="dateSectie">Informatii despre sectia de votare si observatorul alocat ei</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IAsyncResult> Inregistreaza([FromBody] ModelDateSectie dateSectie)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);

            var command = _mapper.Map<InregistreazaSectieCommand>(dateSectie);

            // TODO[DH] get the actual IdObservator from token
            command.IdObservator = int.Parse(User.Claims.First(c => c.Type == "IdObservator").Value);

            var result = await _mediator.Send(command);

            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }

        /// <summary>
        /// Se apeleaza aceasta metoda cand se actualizeaza informatiile legate de ora plecarii.
        /// Aceste informatii sunt insotite de id-ul sectiei de votare.
        /// </summary>
        /// <param name="dateSectie">Numar sectie de votare, cod judet, ora plecarii</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IAsyncResult> Actualizeaza([FromBody] ModelActualizareDateSectie dateSectie)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);

            int idSectie = await _mediator.Send(_mapper.Map<ModelSectieQuery>(dateSectie));
            if (idSectie < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            var command = _mapper.Map<ActualizeazaSectieCommand>(dateSectie);

            // TODO get the actual IdObservator from token
            command.IdObservator = int.Parse(User.Claims.First(c => c.Type == "IdObservator").Value);
            command.IdSectieDeVotare = idSectie;

            var result = await _mediator.Send(command);

            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }
    }
}
