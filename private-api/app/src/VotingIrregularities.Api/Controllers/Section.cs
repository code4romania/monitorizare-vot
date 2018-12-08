using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VotingIrregularities.Api.Extensions;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.SectionAggregate;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/sectie")]
    public class Section : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public Section(IMediator mediator, IMapper mapper)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary>
        /// Se apeleaza aceast metoda cand observatorul salveaza informatiile legate de ora sosirii. ora plecarii, zona urbana, info despre presedintele BESV.
        /// Aceste informatii sunt insotite de id-ul sectiei de votare.
        /// </summary>
        /// <param name="sectionData">Informatii despre sectia de votare si observatorul alocat ei</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IAsyncResult> Register([FromBody] SectionDataModel sectionData)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);

            var command = _mapper.Map<RegisterSectionCommand>(sectionData);

            // TODO[DH] get the actual ObserverId from token
            command.ObserverId = int.Parse(User.Claims.First(c => c.Type == "ObserverId").Value);

            var result = await _mediator.Send(command);

            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }

        /// <summary>
        /// Se apeleaza aceasta metoda cand se actualizeaza informatiile legate de ora plecarii.
        /// Aceste informatii sunt insotite de id-ul sectiei de votare.
        /// </summary>
        /// <param name="sectionData">Numar sectie de votare, cod judet, ora plecarii</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IAsyncResult> Update([FromBody] SectionDataUpdateModel sectionData)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);

            int idSectie = await _mediator.Send(_mapper.Map<SectionModelQuery>(sectionData));
            if (idSectie < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            var command = _mapper.Map<SectionUpdateCommand>(sectionData);

            // TODO get the actual ObserverId from token
            command.ObserverId = int.Parse(User.Claims.First(c => c.Type == "ObserverId").Value);
            command.VotingSectionId = idSectie;

            var result = await _mediator.Send(command);

            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }
    }
}
