using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.Core.Commands;

[Obsolete("Will be removed when ui will use multiple files upload")]
public record UploadFileCommand(IFormFile File, UploadType UploadType) : IRequest<string>;
