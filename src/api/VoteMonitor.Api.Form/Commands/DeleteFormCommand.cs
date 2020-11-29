using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.Form.Commands
{
    public enum DeleteFormErrorType
    {
        FormNotFound,
        FormHasAnswers,
        FormNotDraft,
        ErrorOccured
    }

    public class DeleteFormCommand : IRequest<Result<bool, DeleteFormErrorType>>
    {
        public int FormId { get; set; }
    }
}
