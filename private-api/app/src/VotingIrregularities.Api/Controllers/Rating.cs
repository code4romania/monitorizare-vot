using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Extensions;
using VotingIrregularities.Api.Models;
using AutoMapper;
using VotingIrregularities.Domain.RatingAggregate;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/note")]
    public class Rating : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public Rating(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Aceasta ruta este folosita cand observatorul incarca o imagine sau un clip in cadrul unei note.
        /// Fisierului atasat i se da contenttype = Content-Type: multipart/form-data
        /// Celalalte proprietati sunt de tip form-data
        /// CountyCode:BU 
        /// SectionNumber:3243
        /// QuestionId: 201
        /// RatingText: "asdfasdasdasdas"
        /// API-ul va returna adresa publica a fisierului unde este salvat si obiectul trimis prin formdata
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("ataseaza")]
        public async Task<dynamic> Upload(IFormFile file, [FromForm]RatingModel nota)
        {
            if (!ModelState.IsValid)
                return this.ResultAsync(HttpStatusCode.BadRequest);

            // TODO[DH] use a pipeline instead of separate Send commands
            // daca nota este asociata sectiei
            int sectionId = await _mediator.Send(_mapper.Map<SectionModelQuery>(nota));
            if (sectionId < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            var command = _mapper.Map<AddRatingCommand>(nota);
            var fileAddress = await _mediator.Send(new FileModel { File = file });

            // TODO[DH] get the actual ObserverId from token
            command.ObserverId = int.Parse(User.Claims.First(c => c.Type == "ObserverId").Value);
            command.AttachedFilePath = fileAddress;
            command.VotingSectionId = sectionId;

            var result = await _mediator.Send(command);

            if (result < 0)
                return this.ResultAsync(HttpStatusCode.NotFound);

            return await Task.FromResult(new { FileAdress = fileAddress, nota = nota });
        }
    }
}
