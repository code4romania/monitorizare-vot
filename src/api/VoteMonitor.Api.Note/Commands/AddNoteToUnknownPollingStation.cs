using MediatR;
using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Note.Commands;

public record AddNoteToUnknownPollingStation : IRequest<int>
{
    public int ObserverId { get;  }
    public string CountyCode { get; }
    public string MunicipalityCode { get; }
    public int PollingStationNumber { get; }
    public int? QuestionId { get;  }
    public string Text { get;  }
    public UploadedFileModel[] Attachments { get;  }

    public AddNoteToUnknownPollingStation(int observerId, string countyCode, string municipalityCode, int pollingStationNumber,  int? questionId, string text, UploadedFileModel[] attachments = null)
    {
        ObserverId = observerId;
        CountyCode = countyCode;
        MunicipalityCode = municipalityCode;
        PollingStationNumber = pollingStationNumber;
        QuestionId = questionId;
        Text = text;
        Attachments = attachments ?? Array.Empty<UploadedFileModel>();
    }
}
