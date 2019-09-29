using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries {
    public class AddFormQuery  : IRequest<FormDTO> {
        public FormDTO Form { get; set; }
    }
}
