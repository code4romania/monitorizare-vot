namespace VoteMonitor.Api.Core.Models;

public record UploadedFileModel
{
    public string FileName { get; init; }
    public string Path { get; init; }
}
