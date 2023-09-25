using MediatR;
using VoteMonitor.Api.Note.Models;

namespace VoteMonitor.Api.Note.Queries;

public class NoteQuery : IRequest<List<NoteModel>>
{
    public int? PollingStationId { get; set; }
    public int? ObserverId { get; set; }
    public int? IdQuestion { get; set; }
}
