using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
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
			var bringAllForms = request.Diaspora == null || request.Diaspora == true;

            var result = await _context.Forms
                .AsNoTracking()
                .Where(x => bringAllForms || x.Diaspora == false)
                .Where(x => x.Draft == false)
                .ToListAsync();

			var sortedForms = result
					.OrderBy(x=>x.Order)
                    .Select(x=>new FormDetailsModel() { 
                       Id = x.Id,
                       Description = x.Description,
                       Code = x.Code,
                       CurrentVersion = x.CurrentVersion,
                       Diaspora = x.Diaspora,
					   Order = x.Order
                    })
                    .ToList();

			return sortedForms;
		}
    }
}
