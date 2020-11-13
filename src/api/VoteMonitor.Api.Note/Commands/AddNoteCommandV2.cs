using MediatR;

namespace VoteMonitor.Api.Note.Commands
{
    public class AddNoteCommandV2 : IRequest<int>
    {
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public int? IdQuestion { get; set; }
        public string Text { get; set; }
        public string[] AttachmentPaths { get; set; }
    }
}
