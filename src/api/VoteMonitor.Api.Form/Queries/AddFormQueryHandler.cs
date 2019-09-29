using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
    public class AddFormQueryHandler :
        AsyncRequestHandler<AddFormQuery, FormDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public AddFormQueryHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected override async Task<FormDTO> HandleCore(AddFormQuery message)
        {
            var newForm = _mapper.Map<Entities.Form>(message.Form);
            _context.Forms.Add(newForm);
            await _context.SaveChangesAsync();
            message.Form.Id = newForm.Id;
            return message.Form;
        }
    }
}