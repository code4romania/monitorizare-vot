using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using VotingIrregularities.Domain.SectionAggregate;

namespace VotingIrregularities.Api.Models
{
    public class SectionDataModel
    {
        [Required(AllowEmptyStrings = false)]
        public string CountyCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int SectionNumber { get; set; }

        public DateTime? ArivalTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public bool? IsUrbanArea { get; set; }
        public bool? BesvPresidentIsWoman { get; set; }
    }



    public class SectionDataModelProfile : Profile
    {
        public SectionDataModelProfile()
        {
            CreateMap<SectionDataModel, RegisterSectionCommand>();
        }
    }
}
