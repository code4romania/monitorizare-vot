using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Models
{
    public class ModelFormular
    {
        public class VersiuneQuery : IAsyncRequest<Dictionary<string,int>>
        {
        }

        public class IntrebariQuery : IAsyncRequest<IEnumerable<ModelSectiune>>
        {
            public string CodFormular { get; set; }
        }
    }
}
