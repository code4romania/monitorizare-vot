using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VotingIrregularities.Api.Models;

namespace VotingIrregularities.Api.Controllers
{
    /// <summary>
    /// Ruta unde se inregistreaza raspunsurile
    /// </summary>
    [Route("/api/v1/raspuns")]
    public class Raspuns : Controller
    {
        /// <summary>
        /// Aici se inregistreaza raspunsul dat de observator la una sau mai multe intrebari, pentru o sectie de votare.
        /// Raspunsul (ModelOptiuniSelectate) poate avea mai multe optiuni (IdOptiune) si potential un text (Value).
        /// </summary>
        /// <param name="raspuns">Sectia de votare, lista de optiuni si textul asociat unei optiuni care se completeaza cand 
        /// optiunea <code>SeIntroduceText = true</code></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task CompleteazaRaspuns([FromBody] ModelRaspuns[] raspuns)
        {
            // TODO[DH] se salveaza efectiv
            await Task.Delay(0);
        }
    }
}
