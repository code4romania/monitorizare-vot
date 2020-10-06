using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// CRUD operations on Questions
    /// </summary>

    [Route("api/v1/question")]
    public class QuestionController : Controller
    {
        private IMediator _mediator;

        public QuestionController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("all")]
        public Task<List<QuestionDTO>> GetAll()
        {
            return null;
        }
        [HttpPost]
        [Authorize("Organizer")]
        public Task<int> NewQuestion()
        {
            return null;
        }
        [HttpDelete]
        public Task DeleteQuestion(int id)
        {
            return null;
        }
    }
}