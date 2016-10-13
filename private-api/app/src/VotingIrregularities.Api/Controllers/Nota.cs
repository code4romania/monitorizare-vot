using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Extensions;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/note")]
    public class Nota : Controller
    {
        private readonly IMediator _mediator;

        public Nota(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Aceasta ruta este folosita cand observatorul incarca o imagine sau un clip in cadrul unei note.
        /// Fisierului atasat i se da contenttype = Content-Type: multipart/form-data
        /// Celalalte proprietati sunt de tip form-data
        /// CodJudet:BU 
        /// NumarSectie:3243
        /// IdIntrebare: 201
        /// API-ul va returna adresa publica a fisierului unde este salvat si obiectul trimis prin formdata
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("ataseaza")]
        public async Task<dynamic> Upload(IFormFile file, [FromForm]ModelNota nota)
        {
            return await Task.FromResult(new { FileAdress = file?.FileName, note=nota});

            //// daca nota este asociata sectiei
            //if (!ModelState.IsValid)
            //    return this.ResultAsync(HttpStatusCode.BadRequest);

            //// TODO[DH] use a pipeline instead of separate Send commands
            //var command = await _mediator.SendAsync(new ModelNoteBulk(note));

            //// TODO[DH] get the actual IdObservator from token
            //command.IdObservator = 1;

            //var result = await _mediator.SendAsync(command);

            //return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }


    }
}
