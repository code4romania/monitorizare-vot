using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Models;
using Microsoft.Extensions.Configuration;
using System;
using VoteMonitor.Api.Form.Models;

namespace VotingIrregularities.Api.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Ruta Formular ofera suport pentru toate operatiile legate de formularele completate de observatori
    /// </summary>
    [Route("api/v1/formular")]
    [Obsolete("use /form instead")]
    public class Formular : Controller
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IMediator _mediator;

        public Formular(IMediator mediator, IConfigurationRoot configuration)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        /// <summary>
        /// Returneaza versiunea tuturor formularelor sub forma unui array. 
        /// Daca versiunea returnata difera de cea din aplicatie, atunci trebuie incarcat formularul din nou 
        /// </summary>
        /// <returns></returns>
        [HttpGet("versiune")]
        public async Task<IActionResult> GetFormVersions()
        {
            var formsAsDict = new Dictionary<string, int>();
            (await _mediator.Send(new FormVersionQuery())).ForEach(form => formsAsDict.Add(form.Code, form.CurrentVersion));

            return Ok(new { Versiune = formsAsDict });
        }

        /// <summary>
        /// Returns an array of forms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFormsAsync()
            => Ok(new ModelVersiune { Formulare = await _mediator.Send(new FormVersionQuery()) });

        /// <summary>
        /// Se interogheaza ultima versiunea a formularului pentru observatori si se primeste definitia lui. 
        /// In definitia unui formular nu intra intrebarile standard (ora sosirii, etc). 
        /// Acestea se considera implicite pe fiecare formular.
        /// </summary>
        /// <param name="idformular">Id-ul formularului pentru care trebuie preluata definitia</param>
        /// <returns></returns>
        [HttpGet("{idformular}")]
        public async Task<IEnumerable<ModelSectiune>> GetFormAsync(string idformular)
        {
            var result = await _mediator.Send(new FormQuestionsQuery {
                CodFormular = idformular,
                CacheHours = _configuration.GetValue<int>("DefaultCacheHours"),
                CacheMinutes = _configuration.GetValue<int>("DefaultCacheMinutes"),
                CacheSeconds = _configuration.GetValue<int>("DefaultCacheSeconds")
            });

            return result;
        }
    }
}