using CsvHelper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Queries;

namespace VoteMonitor.Api.Observer.Controllers;

[ApiController]
[Route("api/v1/observer")]
public class ObserverController : Controller
{
    private readonly IMediator _mediator;
    private readonly DefaultNgoOptions _defaultNgoOptions;

    private int NgoId => this.GetIdOngOrDefault(_defaultNgoOptions.DefaultNgoId);
    private bool IsOrganizer => this.GetOrganizatorOrDefault(false);

    public ObserverController(IMediator mediator, IOptions<DefaultNgoOptions> defaultNgoOptions)
    {
        _mediator = mediator;
        _defaultNgoOptions = defaultNgoOptions.Value;
    }

    [HttpGet]
    [Produces(type: typeof(ApiListResponse<ObserverModel>))]
    [Authorize] // for now do not allow anonymous users.
    public async Task<ApiListResponse<ObserverModel>> GetObservers([FromQuery] ObserverListQuery query)
    {
        var command = new ObserverListCommand()
        {
            Number = query.Number,
            Name = query.Name,
            Page = query.Page,
            PageSize = query.PageSize,
            NgoId = IsOrganizer ? -1 : NgoId
        };

        var result = await _mediator.Send(command);
        return result;
    }

    [HttpGet]
    [Authorize("NgoAdmin")]
    [Produces(type: typeof(List<ObserverModel>))]
    [Route("active")]
    public async Task<List<ObserverModel>> GetActiveObservers([FromQuery] ActiveObserverFilter filter)
    {
        var query = new ActiveObserversQuery(IsOrganizer ? -1 : NgoId, filter.CountyCodes, filter.CurrentlyCheckedIn);

        var result = await _mediator.Send(query);
        return result;
    }

    [HttpGet]
    [Produces(type: typeof(int))]
    [Authorize] // for now do not allow anonymous users.
    [Route("count")]
    public async Task<int> GetTotalObserverCount()
    {
        var result = await _mediator.Send(new ObserverCountCommand(NgoId, IsOrganizer));
        return result;
    }

    [HttpPost]
    [Authorize("Organizer")]
    [Route("import")]
    [Produces(type: typeof(int))]
    public async Task<int> Import(IFormFile file, [FromForm] int ngoId)
    {
        if (ngoId <= 0)
        {
            ngoId = NgoId;
        }

        var counter = await _mediator.Send(new ImportObserversRequest(ngoId, file));

        return counter;
    }

    [HttpGet]
    [Authorize("Organizer")]
    [Route("import-template")]
    public IActionResult DownloadImportTemplate()
    {
        using (var mem = new MemoryStream())
        using (var writer = new StreamWriter(mem))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(new[]
            {
                new ObserversImportModel
                {
                    Phone = "observer phone",
                    Pin = "observer pin",
                    Name = "observer name",
                }
            });
            writer.Flush();
            return File(mem.ToArray(), "application/octet-stream", "observers-import-template.csv");
        }
    }

    /// <summary>
    ///  Adds an observer.
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Boolean indicating whether or not the observer was added successfully.</returns>
    [HttpPost]
    [Authorize("Organizer")]
    [Produces(type: typeof(int))]
    public async Task<IActionResult> NewObserver([FromBody] NewObserverModel model)
    {
        var newObsCommand = new NewObserverCommand()
        {
            Name = model.Name,
            Pin = model.Pin,
            Phone = model.Phone,
            NgoId = NgoId
        };

        var newId = await _mediator.Send(newObsCommand);

        return Ok(newId);
    }

    /// <summary>
    /// Edits Observer information.
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Boolean indicating whether or not the observer was changed successfully</returns>
    [HttpPut]
    [Authorize("Organizer")]
    [Produces(type: typeof(bool))]
    public async Task<IActionResult> EditObserver([FromBody] EditObserverModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name)
           && string.IsNullOrWhiteSpace(model.Phone)
           && string.IsNullOrWhiteSpace(model.Pin))
        {
            return BadRequest("Invalid request");
        }

        var isActionAllowed = await IsActionAllowed(model.ObserverId!.Value);
        if (!isActionAllowed)
        {
            return Problem("Action not allowed", statusCode: (int)HttpStatusCode.BadRequest);
        }

        var id = await _mediator.Send(new EditObserverCommand()
        {
            Name = model.Name,
            Phone = model.Phone,
            Pin = model.Pin,
            ObserverId = model.ObserverId!.Value
        });

        return Ok(id > 0);
    }

    /// <summary>
    /// Deletes an observer.
    /// </summary>
    /// <param name="id">The Observer id</param>
    /// <returns>Boolean indicating whether or not the observer was deleted successfully</returns>
    [HttpDelete]
    [Authorize("NgoAdmin")]
    [Produces(type: typeof(bool))]
    public async Task<IActionResult> DeleteObserver(int id)
    {
        var isActionAllowed = await IsActionAllowed(id);
        if (!isActionAllowed)
        {
            return Problem("Action not allowed", statusCode: (int)HttpStatusCode.BadRequest);
        }

        var result = await _mediator.Send(new DeleteObserverCommand(id));

        return Ok(result);
    }

    private async Task<bool> IsActionAllowed(int observerId)
    {
        if (IsOrganizer)
        {
            return true;
        }

        var observerRequest = new GetObserverDetails(NgoId, observerId);

        var observer = await _mediator.Send(observerRequest);
        if (observer == null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Removes mobile device Id associated with Observer of given Id.
    /// </summary>
    /// <returns>Boolean indicating whether or not the mobile device Id was removed successfully</returns>
    [HttpPost]
    [Route("removeDeviceId")]
    [Authorize("NgoAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveObserverDeviceId([FromBody] RemoveDeviceIdModel request)
    {
        var isActionAllowed = await IsActionAllowed(request.Id);
        if (!isActionAllowed)
        {
            return Problem("Action not allowed", statusCode: (int)HttpStatusCode.BadRequest);
        }

        await _mediator.Send(new RemoveDeviceIdCommand(request.Id));

        return Ok();
    }

    [HttpPost]
    [Route("reset")]
    [Authorize("Organizer")]
    public async Task<IActionResult> Reset([FromBody] ResetModel model)
    {
        if (string.IsNullOrEmpty(model.Action) || string.IsNullOrEmpty(model.PhoneNumber))
        {
            return BadRequest();
        }

        if (string.Equals(model.Action, ControllerExtensions.DEVICE_RESET))
        {
            var result = await _mediator.Send(new ResetDeviceCommand
            {
                NgoId = NgoId,
                PhoneNumber = model.PhoneNumber,
                Organizer = this.GetOrganizatorOrDefault(false)
            });
            if (result == -1)
            {
                return NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + model.PhoneNumber);
            }
            else
            {
                return Ok(result);
            }
        }

        if (string.Equals(model.Action, ControllerExtensions.PASSWORD_RESET))
        {
            var result = await _mediator.Send(new ResetPasswordCommand
            {
                NgoId = NgoId,
                PhoneNumber = model.PhoneNumber,
                Pin = model.Pin,
                IsOrganizer = this.GetOrganizatorOrDefault(false)
            });
            if (result == false)
            {
                return NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + model.PhoneNumber);
            }

            return Ok();
        }

        return UnprocessableEntity();
    }

    [HttpPost]
    [Authorize("Organizer")]
    [Route("generate")]
    [Produces(type: typeof(List<GeneratedObserver>))]
    public async Task<IActionResult> GenerateObservers([FromForm] int count)
    {
        if (!ControllerExtensions.ValidateGenerateObserversNumber(count))
        {
            return BadRequest("Incorrect parameter supplied, please check that parameter is between boundaries: "
                              + ControllerExtensions.LOWER_OBS_VALUE + " - " + ControllerExtensions.UPPER_OBS_VALUE);
        }

        var command = new ObserverGenerateCommand(count, NgoId);

        var result = await _mediator.Send(command);

        return Ok(result);
    }
}
