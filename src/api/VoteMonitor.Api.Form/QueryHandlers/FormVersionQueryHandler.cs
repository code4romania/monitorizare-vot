using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers
{
    public class FormVersionQueryHandler : IRequestHandler<FormVersionQuery, List<FormDetailsModel>>
    {
        private readonly VoteMonitorContext _context;

        public FormVersionQueryHandler(VoteMonitorContext context)
        {
            _context = context;
        }

        public async Task<List<FormDetailsModel>> Handle(FormVersionQuery request, CancellationToken cancellationToken)
        {
            // Diaspora gets all forms
            var bringAllForms = request.Diaspora == null || request.Diaspora == true;

            var result = await _context.Forms
                .AsNoTracking()
                .Where(x => bringAllForms || x.Diaspora == false)
                .Where(x => request.Draft == null || x.Draft == request.Draft)
                .Include(x=>x.FormSections)
                .ThenInclude(x=>x.Questions)
                .ToListAsync(cancellationToken);

            var sortedForms = result
                    .OrderBy(x => x.Order)
                    .Select(x => new FormDetailsModel
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Code = x.Code,
                        CurrentVersion = x.CurrentVersion,
                        Diaspora = x.Diaspora,
                        Order = x.Order,
                        Draft = x.Draft,
                        NumberOfQuestions = x.FormSections.Select(fs => fs.Questions.Count).Sum(),
                        NumberOfSections = x.FormSections.Count,
                        LastUpdatedOn = x.LastUpdatedOn
                    })
                    .ToList();

            return sortedForms;
        }
    }
}
