using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Core.Options;
using Microsoft.Extensions.Options;

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
        private readonly ApplicationCacheOptions _cacheOptions;
        private readonly IMediator _mediator;

        public Formular(IMediator mediator, IOptions<ApplicationCacheOptions> cacheOptions)
        {
			_cacheOptions = cacheOptions.Value;
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
			// for now all of the forms are retrieved
            var formsAsDict = new Dictionary<string, int>();
            (await _mediator.Send(new FormVersionQuery(null))).ForEach(form => formsAsDict.Add(form.Code, form.CurrentVersion));

            return Ok(new { Versiune = formsAsDict });
        }

        /// <summary>
        /// Returns an array of forms. This method will not apply filtering
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFormsAsync()
            => Ok(new ModelVersiune { Formulare = await _mediator.Send(new FormVersionQuery(null)) });

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
				CacheHours = _cacheOptions.Hours,
				CacheMinutes = _cacheOptions.Minutes,
				CacheSeconds = _cacheOptions.Seconds
			});

            return result;
        }
    }
}