using AutoMapper;
using System.ComponentModel.DataAnnotations;
using VotingIrregularities.Domain.RatingAggregate;

namespace VotingIrregularities.Api.Models
{
    public class RatingModel
    {
        [Required(AllowEmptyStrings = false)]
        public string CountyCode { get; set; }
        [Required]
        public int SectionNumber { get; set; }
        public int? QuestionId { get; set; }
        public string RatingText { get; set; }
    }

    public class RatingModelProfile : Profile
    {
        public RatingModelProfile()
        {
            CreateMap<RatingModel, SectionModelQuery>();
            CreateMap<RatingModel, AddRatingCommand>();
        }
    }
}
