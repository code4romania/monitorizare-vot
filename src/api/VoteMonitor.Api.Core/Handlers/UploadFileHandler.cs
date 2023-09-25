using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Core.Services;

namespace VoteMonitor.Api.Core.Handlers;

public class UploadFileHandler : IRequestHandler<UploadFileCommandV2, UploadedFileModel[]>,
    IRequestHandler<UploadFileCommand, UploadedFileModel>
{
    private readonly IFileService _fileService;
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public UploadFileHandler(IFileService fileService, FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileService = fileService;
        _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
    }

    /// <summary>
    ///  Uploads a list of files in blob storage
    /// </summary>
    /// <returns>The url of the blob</returns>
    public async Task<UploadedFileModel[]> Handle(UploadFileCommandV2 message, CancellationToken cancellationToken)
    {
        if (message.Files != null && message.Files.Any())
        {
            var uploadTasks = new List<Task<UploadedFileModel>>();

            foreach (var file in message.Files)
            {
                var uploadTask = UploadFileAsync(file, message.UploadType);

                uploadTasks.Add(uploadTask);
            }

            UploadedFileModel[] paths = await Task.WhenAll(uploadTasks);

            return paths;
        }

        return Array.Empty<UploadedFileModel>();
    }



    [Obsolete("Will be removed when ui will use multiple files upload")]
    public async Task<UploadedFileModel> Handle(UploadFileCommand message, CancellationToken cancellationToken)
    {
        if (message.File != null)
        {
            var fileExtension = Path.GetExtension(message.File.FileName);
            _fileExtensionContentTypeProvider.TryGetContentType(fileExtension, out var contentType);

            return await UploadFileAsync(message.File, message.UploadType);
        }

        return null;
    }

    private async Task<UploadedFileModel> UploadFileAsync(IFormFile file, UploadType uploadType)
    {
        var fileExtension = Path.GetExtension(file.FileName);
        _fileExtensionContentTypeProvider.TryGetContentType(fileExtension, out var contentType);

        var path = await _fileService.UploadFromStreamAsync(file.OpenReadStream(),
            contentType ?? file.ContentType,
            fileExtension,
            uploadType);

        return path;
    }
}
