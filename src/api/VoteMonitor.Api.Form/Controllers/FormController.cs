﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Queries;

namespace VoteMonitor.Api.Form.Controllers {
    /// <inheritdoc />
    /// <summary>
    /// Ruta Formular ofera suport pentru toate operatiile legate de formularele completate de observatori
    /// </summary>

    [Route("api/v1/form")]
    public class FormController : Controller
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IMediator _mediator;

        public FormController(IMediator mediator, IConfigurationRoot configuration)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<int> AddForm([FromBody]FormDTO newForm) {
            FormDTO result = await _mediator.Send(new AddFormQuery { Form = newForm });
            return result.Id;
        }
        /// <summary>
        /// Returneaza versiunea tuturor formularelor sub forma unui array. 
        /// Daca versiunea returnata difera de cea din aplicatie, atunci trebuie incarcat formularul din nou 
        /// </summary>
        /// <returns></returns>
        [HttpGet("versions")]
        [Produces(typeof(Dictionary<string,int>))]
        public async Task<IActionResult> GetFormVersions()
        {
            var formsAsDict = new Dictionary<string, int>();
            (await _mediator.Send(new FormVersionQuery())).ForEach(form => formsAsDict.Add(form.Code, form.CurrentVersion));

            return Ok(new { Versions = formsAsDict });
        }

        /// <summary>
        /// Returns an array of forms
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFormsAsync()
            => Ok(new { FormVersions = await _mediator.Send(new FormVersionQuery()) });

        /// <summary>
        /// Se interogheaza ultima versiunea a formularului pentru observatori si se primeste definitia lui. 
        /// In definitia unui formular nu intra intrebarile standard (ora sosirii, etc). 
        /// Acestea se considera implicite pe fiecare formular.
        /// </summary>
        /// <param name="formId">Id-ul formularului pentru care trebuie preluata definitia</param>
        /// <returns></returns>
        [HttpGet("{formId}")]
        public async Task<IEnumerable<FormSectionDTO>> GetFormAsync(string formId)
        {
            var result = await _mediator.Send(new FormQuestionQuery {
                FormCode = formId,
                CacheHours = _configuration.GetValue<int>("DefaultCacheHours"),
                CacheMinutes = _configuration.GetValue<int>("DefaultCacheMinutes"),
                CacheSeconds = _configuration.GetValue<int>("DefaultCacheSeconds")
            });

            return result;
        }
    }
}