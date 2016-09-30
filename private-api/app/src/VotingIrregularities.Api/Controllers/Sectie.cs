using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Controllers
{
    [Route("api/v1/sectie")]
    public class Sectie : Controller
    {
        /// <summary>
        /// Se apeleaza aceast metoda cand observatorul salveaza informatiile legate de ora sosirii. ora plecarii, zona urbana, info despre presedintele BESV.
        /// Aceste informatii sunt insotite de id-ul sectiei de votare.
        /// </summary>
        /// <param name="dateSectie">Informatii despre sectia de votare si observatorul alocat ei</param>
        /// <returns></returns>
        [HttpPost()]
        public async Task Inregistreaza([FromBody] ModelDateSectie dateSectie)
        {
            // TODO[DH] se salveaza efectiv
            await Task.FromResult(0);
        }
    }
}
