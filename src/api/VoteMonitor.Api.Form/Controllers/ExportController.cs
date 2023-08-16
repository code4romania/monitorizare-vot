using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Queries;

namespace VoteMonitor.Api.Form.Controllers
{
    [Route("api/v1/export")]
    public class ExportController : Controller
    {
        private readonly IMediator _mediator;

        public ExportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("forms")]
        [Authorize("Organizer")]
        public async Task<IReadOnlyList<FormResponseModel>> ExportFormsAsync()
        {
            var filledInForms = await _mediator.Send(new GetFormsQuery());
            return filledInForms;
        }

        [HttpPost("filled-forms")]
        [Authorize("Organizer")]
        public async Task<IReadOnlyList<FilledFormResponseModel>> ExportFilledFormsAsync()
        {
            var filledInForms = await _mediator.Send(new GetFilledFormsQuery());
            return filledInForms;
        }
    }
}
