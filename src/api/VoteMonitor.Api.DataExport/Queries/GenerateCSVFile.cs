using MediatR;
using System.Collections.Generic;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.DataExport.Queries
{
    public class GenerateCSVFile : IRequest<byte[]>
    {
        public IEnumerable<ExportModel> Data { get; }

        public GenerateCSVFile(IEnumerable<ExportModel> data)
        {
            Data = data;
        }
    }
}