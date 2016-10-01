using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/nota")]
    public class Nota : Controller
    {
        /// <summary>
        /// Se apeleaza aceasta ruta cand observatorul adauga sau modifica nota asociata unei intrebari.
        /// </summary>
        /// <param name="nota"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Inregistreaza([FromBody] ModelNota nota)
        {
            // TODO[DH] se salveaza efectiv
            await Task.FromResult(0);
        }


        /// <summary>
        /// Aceasta ruta este folosita cand observatorul incarca o imagine sau un clip in cadrul unei note.
        /// TODO: de adaugat in request header detaliile userului, sectiei si a intrebarii. Ex:
        /// codJudet:BU 
        /// numarSectie:3243
        /// idObservator: 3022
        /// idIntrebare: 201
        /// API-ul va returna adresa publica a fisierului unde este salvat.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("ataseaza")]
        public async Task<dynamic> Upload(IFormFile file)
        {
            return await Task.FromResult(new { FileAdress = file?.FileName});
        }

    }
}
