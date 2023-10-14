using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Note.Commands;
using VoteMonitor.Api.Note.Models;
using VoteMonitor.Api.Note.Queries;

namespace VoteMonitor.Api.Note.Controllers;

[ApiController]
[Route("api/v2/note")]
public class NoteController : Controller
{
    private readonly IMediator _mediator;

    public NoteController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet]
    [Authorize("NgoAdmin")]
    [Produces(type: typeof(List<NoteModel>))]
    public async Task<IActionResult> GetNotes([FromQuery] NoteQuery filter)
    {
        if (filter.IdQuestion.HasValue && !filter.PollingStationId.HasValue)
            return BadRequest($"If the {nameof(filter.IdQuestion)} param is provided then the {nameof(filter.PollingStationId)} param is required !");

        if (!filter.ObserverId.HasValue)
        {
            filter.ObserverId = this.GetIdObserver();
        }

        return Ok(await _mediator.Send(filter));
    }

    [HttpPost]
    [Authorize("Observer")]
    [Produces(type: typeof(UploadNoteResultV2))]
    public async Task<IActionResult> Upload([FromForm] UploadNoteModelV2 note)
    {
        // TODO[DH] use a pipeline instead of separate Send commands
        // daca nota este asociata sectiei
        var pollingStationId = await _mediator.Send(new GetPollingStationId(note.CountyCode, note.MunicipalityCode, note.PollingStationNumber));

        UploadedFileModel[] attachments = { };
        if (note.Files != null && note.Files.Any())
        {
            var files = await _mediator.Send(new UploadFileCommandV2(note.Files, UploadType.Notes));
            attachments = files;
        }

        if (pollingStationId > 0)
        {
            var command = new AddNoteCommandV2(this.GetIdObserver(), pollingStationId, note.QuestionId, note.Text, attachments);
            var result = await _mediator.Send(command);
            return ProcessUpload(result, note, command.Attachments.Select(x => x.Path).ToArray());
        }
        else
        {
            var command = new AddNoteToUnknownPollingStation(this.GetIdObserver(), note.CountyCode, note.MunicipalityCode, note.PollingStationNumber, note.QuestionId, note.Text, attachments);
            var result = await _mediator.Send(command);
            return ProcessUpload(result, note, command.Attachments.Select(x => x.Path).ToArray());
        }
    }

    private IActionResult ProcessUpload(int saveOperationResult, UploadNoteModelV2 note, string[] fileAddress)
    {
        if (saveOperationResult < 0)
        {
            return NotFound();
        }

        var model = new UploadNoteResultV2
        {
            FilesAddress = fileAddress,
            Note = new UploadNoteModelV2()
            {
                CountyCode = note.CountyCode,
                MunicipalityCode = note.MunicipalityCode,
                Text = note.Text,
                PollingStationNumber = note.PollingStationNumber,
                QuestionId = note.QuestionId
            }
        };

        return Ok(model);
    }


    /// <summary>
    /// Aceasta ruta este folosita cand observatorul incarca o imagine sau un clip in cadrul unei note.
    /// Fisierului atasat i se da contenttype = Content-Type: multipart/form-data
    /// Celalalte proprietati sunt de tip form-data
    /// CodJudet:BU 
    /// NumarSectie:3243
    /// IdIntrebare: 201
    /// TextNota: "asdfasdasdasdas"
    /// API-ul va returna adresa publica a fisierului unde este salvat si obiectul trimis prin formdata
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
    [HttpPost("upload")]
    [Authorize("Observer")]
    [Obsolete("Will be removed when ui will use multiple files upload")]
    [Produces(type: typeof(UploadNoteResult))]
    public async Task<IActionResult> UploadOld([FromForm] UploadNoteModel note)
    {
        // TODO[DH] use a pipeline instead of separate Send commands
        // daca nota este asociata sectiei
        var pollingStationId = await _mediator.Send(new GetPollingStationId(note.CountyCode, note.MunicipalityCode, note.PollingStationNumber));

        if (pollingStationId < 0)
        {
            return NotFound();
        }

        var command = new AddNoteCommand
        {
            IdObserver = int.Parse(User.Claims.First(c => c.Type == ClaimsHelper.ObserverIdProperty).Value),
            IdPollingStation = pollingStationId,
            Text = note.Text,
            IdQuestion = note.QuestionId
        };

        if (note.File != null)
        {
            var fileAddress = await _mediator.Send(new UploadFileCommand(note.File, UploadType.Notes));
            command.Attachement = fileAddress;
        }

        var result = await _mediator.Send(command);

        if (result < 0)
        {
            return NotFound();
        }

        return Ok(new UploadNoteResult
        {
            FileAddress = command.Attachement.Path,
            Note = new UploadNoteModel
            {
                CountyCode = note.CountyCode,
                MunicipalityCode = note.MunicipalityCode,
                Text = note.Text,
                PollingStationNumber = note.PollingStationNumber,
                QuestionId = note.QuestionId
            }
        });
    }
}
