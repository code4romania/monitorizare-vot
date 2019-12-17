using MediatR;
using System.Collections.Generic;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.DataExport.Queries
{
    public class GenerateExcelFile : IRequest<byte[]>
    {
        public List<ExportModel> Data { get; }

        public GenerateExcelFile(List<ExportModel> data)
        {
            Data = data;
        }
    }
}
