using MediatR;

namespace VoteMonitor.Api.DataExport.Queries;

public record GetExcelDbCommand : IRequest<byte[]>;
