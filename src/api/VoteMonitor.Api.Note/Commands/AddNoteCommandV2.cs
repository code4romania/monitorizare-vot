using MediatR;
using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Note.Commands;

public record AddNoteCommandV2 : IRequest<int>
{
    public int ObserverId { get;  }
    public int PollingStationId { get;  }
    public int? QuestionId { get;  }
    public string Text { get;  }
    public UploadedFileModel[] Attachments { get;  }

    public AddNoteCommandV2(int observerId, int pollingStationId, int? questionId, string text, UploadedFileModel[] attachments = null)
    {
        ObserverId = observerId;
        PollingStationId = pollingStationId;
        QuestionId = questionId;
        Text = text;
        Attachments = attachments ?? Array.Empty<UploadedFileModel>();
    }
}
