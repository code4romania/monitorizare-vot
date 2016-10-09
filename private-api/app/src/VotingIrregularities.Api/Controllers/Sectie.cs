using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VotingIrregularities.Api.Extensions;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.SectieAggregate;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/sectie")]
    public class Sectie : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediatr;

        public Sectie(IMediator mediatr, IMapper mapper)
        {
            _mapper = mapper;
            _mediatr = mediatr;
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
            command.IdObservator = 1;

            var result = await _mediatr.SendAsync(command);

            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }
    }
}
