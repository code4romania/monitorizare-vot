using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VoteMonitor.Api.DataExport.Controller;
using VoteMonitor.Api.DataExport.Queries;

namespace VoteMonitor.Api.DataExport.Handlers
{
    public class ExcelGeneratorQueryHandler: IRequestHandler<GenerateExcelFile, byte[]>
    {
        private readonly IExcelGenerator _excelGenerator;

        public ExcelGeneratorQueryHandler()
        {
            _excelGenerator = new ExcelGenerator();
        }

        public Task<byte[]> Handle(GenerateExcelFile request, CancellationToken cancellationToken)
        {
            var fileContents = _excelGenerator.Export(request.Data, "myData");

            return Task.FromResult(fileContents);
        }
    }
}