using MediatR;
using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Note.Commands;

[Obsolete("Will be removed when ui will use multiple files upload")]
public class AddNoteCommand : IRequest<int>
{
    public int IdObserver { get; set; }
    public int IdPollingStation { get; set; }
    public int? IdQuestion { get; set; }
    public string Text { get; set; }
    public UploadedFileModel Attachement { get; set; }
}
