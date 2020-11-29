using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Queries;

namespace VoteMonitor.Api.Form.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Form Controller offers support for CRUD Operations on the forms completed by observers.
    /// </summary>

    [Route("api/v1/form")]
    public class FormController : Controller
    {
        private readonly ApplicationCacheOptions _cacheOptions;
        private readonly IMediator _mediator;

        public FormController(IMediator mediator, IOptions<ApplicationCacheOptions> cacheOptions)
        {
            _cacheOptions = cacheOptions.Value;
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize("Organizer")]
        public async Task<int> AddForm([FromBody] FormDTO newForm)
        {
            var result = await _mediator.Send(new AddFormCommand { Form = newForm });
            return result.Id;
        }

        [HttpPut]
        [Authorize("Organizer")]
        public async Task<ActionResult> UpdateForm([FromBody] FormDTO newForm)
        {
            var formExists = await _mediator.Send(new GetFormExistsByIdQuery() { Id = newForm.Id });
            if (!formExists)
            {
                return NotFound($"The form with id {newForm.Id} was not found.");
            }

            var result = await _mediator.Send(new UpdateFormCommand { Id = newForm.Id, Form = newForm });
            return Ok(result.Id);
        }
        /// <summary>
        /// Returneaza versiunea tuturor formularelor sub forma unui array. 
        /// Daca versiunea returnata difera de cea din aplicatie, atunci trebuie incarcat formularul din nou 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("versions")]
        [Produces(typeof(Dictionary<string, int>))]
        public async Task<IActionResult> GetFormVersions()
        {
            var formsAsDict = new Dictionary<string, int>();
            (await _mediator.Send(new FormVersionQuery(null, null))).ForEach(form => formsAsDict.Add(form.Code, form.CurrentVersion));

            return Ok(new { Versions = formsAsDict });
        }

        /// <summary>
        /// Returns an array of forms
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFormsAsync(bool? diaspora, bool? draft)
            => Ok(new FormVersionsModel { FormVersions = await _mediator.Send(new FormVersionQuery(diaspora, draft)) });

        /// <summary>
        /// Se interogheaza ultima versiunea a formularului pentru observatori si se primeste definitia lui. 
        /// In definitia unui formular nu intra intrebarile standard (ora sosirii, etc). 
        /// Acestea se considera implicite pe fiecare formular.
        /// </summary>
        /// <param name="formId">Id-ul formularului pentru care trebuie preluata definitia</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{formId}")]
        public async Task<IEnumerable<FormSectionDTO>> GetFormAsync(int formId)
        {
            var result = await _mediator.Send(new FormQuestionQuery
            {
                FormId = formId,
                CacheHours = _cacheOptions.Hours,
                CacheMinutes = _cacheOptions.Minutes,
                CacheSeconds = _cacheOptions.Seconds
            });

            return result;
        }

        [HttpDelete]
        [Authorize("Organizer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteForm(int formId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(new DeleteFormCommand { FormId = formId });

            if (result.IsFailure)
            {
                switch (result.Error)
                {
                    case DeleteFormErrorType.FormHasAnswers:
                        return BadRequest("Could not delete with form that has answers.");
                    case DeleteFormErrorType.FormNotFound:
                        return BadRequest("Could not find form with requested id.");
                    case DeleteFormErrorType.FormNotDraft:
                        return BadRequest("Could not delete a non draft form.");
                    default:
                        return BadRequest("An error occured when deleting form.");
                }
            }

            return Ok();
        }

        [HttpDelete("section/{sectionId}")]
        [Authorize("Organizer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSection(int sectionId)
        {
            var deleted = await _mediator.Send(new DeleteSectionCommand { SectionId = sectionId });

            if (!deleted)
            {
                return BadRequest("The section could not be deleted. Make sure the section exists and it doesn't already have saved answers.");
            }

            return Ok();
        }
    }
}