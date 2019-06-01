using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using VotingIrregularities.Domain.SectieAggregate;

namespace VotingIrregularities.Api.Models
{
    public class UpdatePollingStationInfo {
        [Required(AllowEmptyStrings = false)]
        public string CountyCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int IdPollingStation { get; set; }

        [Required(AllowEmptyStrings = false)]
        public DateTime? LeaveTime { get; set; }
    }

    public class UpdatePollingStationInfoProfile : Profile
    {
        public UpdatePollingStationInfoProfile()
        {
            CreateMap<UpdatePollingStationInfo, PollingStationQuery>();
            CreateMap<UpdatePollingStationInfo, UpdatePollingSectionCommand>();
        }
    }
}
