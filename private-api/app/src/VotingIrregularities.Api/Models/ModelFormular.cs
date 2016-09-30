using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace VotingIrregularities.Api.Models
{
    public class ModelFormular
    {
        public class VersiuneQuery : IAsyncRequest<Dictionary<string,int>>
        {
        }
    }
}
