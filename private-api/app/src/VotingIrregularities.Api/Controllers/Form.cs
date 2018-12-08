using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VotingIrregularities.Api.Models;
using Microsoft.Extensions.Configuration;

namespace VotingIrregularities.Api.Controllers
{
    /// <summary>
    /// Ruta Form ofera suport pentru toate operatiile legate de formularele completate de observatori
    /// </summary>
    [Route("api/v1/formular")]
    public class Form : Controller
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IMediator _mediator;

        public Form(IMediator mediator, IConfigurationRoot configuration)
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
        public async Task<VersionModel> Version()
        {
            return new VersionModel { Version = await _mediator.Send(new FormModel.VersionQuery()) };
        }

        /// <summary>
        /// Se interogheaza ultima versiunea a formularului pentru observatori si se primeste definitia lui. 
        /// In definitia unui formular nu intra intrebarile standard (ora sosirii, etc). 
        /// Acestea se considera implicite pe fiecare formular.
        /// </summary>
        /// <param name="formId">Id-ul formularului pentru care trebuie preluata definitia</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<SectionModel>> GetFormById(string formId)
        {
            var result = await _mediator.Send(new FormModel.QuestionsQuery {
                FormCode = formId,
                CacheHours = _configuration.GetValue<int>("DefaultCacheHours"),
                CacheMinutes = _configuration.GetValue<int>("DefaultCacheMinutes"),
                CacheSeconds = _configuration.GetValue<int>("DefaultCacheSeconds")
            });

            return result;
        }
    }
}