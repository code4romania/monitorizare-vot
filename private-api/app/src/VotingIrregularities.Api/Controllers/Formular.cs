using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Models;
using Microsoft.Extensions.Configuration;

namespace VotingIrregularities.Api.Controllers
{
    /// <summary>
    /// Ruta Formular ofera suport pentru toate operatiile legate de formularele completate de observatori
    /// </summary>
    [Route("api/v1/formular")]
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
        public async Task<ModelVersiune> Versiune()
        {
            return new ModelVersiune { Versiune = await _mediator.Send(new ModelFormular.VersiuneQuery())};
        }

        /// <summary>
        /// Se interogheaza ultima versiunea a formularului pentru observatori si se primeste definitia lui. 
        /// In definitia unui formular nu intra intrebarile standard (ora sosirii, etc). 
        /// Acestea se considera implicite pe fiecare formular.
        /// </summary>
        /// <param name="idformular">Id-ul formularului pentru care trebuie preluata definitia</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ModelSectiune>> Citeste(string idformular)
        {
            var result = await _mediator.Send(new ModelFormular.IntrebariQuery {
                CodFormular = idformular,
                CacheHours = _configuration.GetValue<int>("DefaultCacheHours"),
                CacheMinutes = _configuration.GetValue<int>("DefaultCacheMinutes"),
                CacheSeconds = _configuration.GetValue<int>("DefaultCacheSeconds")
            });

            return result;
        }
    }
}