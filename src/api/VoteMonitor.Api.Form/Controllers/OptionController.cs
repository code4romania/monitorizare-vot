using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Models.Options;
using VoteMonitor.Api.Form.Queries;

namespace VoteMonitor.Api.Form.Controllers
{
    [Route("api/v1/option")]
    public class OptionController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public OptionController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<OptionModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<List<OptionModel>> GetAll()
        {
            var options = await _mediator.Send(new FetchAllOptionsCommand());
            var mappedResult = options.Select(dto => _mapper.Map<OptionModel>(dto)).ToList();

            return mappedResult;
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(OptionModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        public async Task<OptionModel> GetByOptionId([FromRoute] int id)
        {
            var optionDto = await _mediator.Send(new GetOptionByIdCommand(id));
            var result = _mapper.Map<OptionModel>(optionDto);
            return result;
        }

        [HttpPost("create")]
        [Authorize("Organizer")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateOptionModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dto = _mapper.Map<OptionDto>(model);
            var optionDto = await _mediator.Send(new AddOptionCommand(dto));

            var result = _mapper.Map<OptionModel>(optionDto);

            return Ok(result);

        }

        [HttpPut("update")]
        [Authorize("Organizer")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IAsyncResult> Update([FromBody] OptionModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.ResultAsync(HttpStatusCode.BadRequest, ModelState);
            }

            var dto = _mapper.Map<OptionDto>(model);
            var result = await _mediator.Send(new UpdateOptionCommand(dto));


            return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
        }

    }
}