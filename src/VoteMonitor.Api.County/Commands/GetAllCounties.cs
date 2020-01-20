using System.Collections.Generic;
using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Commands
{
    public class GetAllCounties : IRequest<Result<List<CountyModel>>>
    {
    }
}