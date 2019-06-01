using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.SectieAggregate;

namespace VotingIrregularities.Api.Models
{

    public class PollingStationProfile : Profile
    {
        public PollingStationProfile()
        {
            CreateMap<PollingStationInfo, RegisterPollingStationCommand>();
        }
    }
}
