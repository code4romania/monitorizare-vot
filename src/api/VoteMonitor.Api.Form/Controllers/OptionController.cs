using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Api.Form.Models.Options;
using VoteMonitor.Api.Form.Queries;

namespace VoteMonitor.Api.Form.Controllers;

[ApiController]
[Route("api/v1/option")]
public class OptionController : Controller
{
    private readonly IMediator _mediator;

    public OptionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<OptionModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<List<OptionModel>> GetAll()
    {
        var options = await _mediator.Send(new FetchAllOptionsQuery());
        var mappedResult = options.Select(ToModel).ToList();

        return mappedResult;
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(OptionModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<OptionModel> GetByOptionId([FromRoute] int id)
    {
        var optionDto = await _mediator.Send(new GetOptionByIdQuery(id));
        var result = ToModel(optionDto);

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
        var optionDto = await _mediator.Send(new AddOptionCommand(model.Text, model.Hint, model.IsFreeText));

        var result = ToModel(optionDto);

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
        var result = await _mediator.Send(new UpdateOptionCommand(model.Id,model.Text, model.Hint, model.IsFreeText));

        return this.ResultAsync(result < 0 ? HttpStatusCode.NotFound : HttpStatusCode.OK);
    }

    private static OptionModel ToModel(OptionDTO optionDto)
    {
        return new OptionModel
        {
            Id = optionDto.Id,
            Hint = optionDto.Hint,
            Text = optionDto.Text,
            IsFreeText = optionDto.IsFreeText
        };
    }
}
