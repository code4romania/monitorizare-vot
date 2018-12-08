using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using VotingIrregularities.Domain.SectionAggregate;

namespace VotingIrregularities.Api.Models
{
    public class SectionDataUpdateModel
    {
        [Required(AllowEmptyStrings = false)]
        public string CountyCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int SectionNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        public DateTime? LeaveTime { get; set; }
    }

    public class UpdateSectionDataModelProfile : Profile
    {
        public UpdateSectionDataModelProfile()
        {
            CreateMap<SectionDataUpdateModel, SectionModelQuery>();
            CreateMap<SectionDataUpdateModel, SectionUpdateCommand>();
        }
    }
}
