using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.Form.Commands;

public enum DeleteFormErrorType
{
    FormNotFound,
    FormHasAnswers,
    FormNotDraft,
    ErrorOccurred
}

public record DeleteFormCommand(int FormId) : IRequest<Result<bool, DeleteFormErrorType>>;
