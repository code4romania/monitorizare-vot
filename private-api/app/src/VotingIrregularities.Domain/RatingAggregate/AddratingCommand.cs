using System;
using AutoMapper;
using MediatR;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.RatingAggregate
{
    public class AddRatingCommand : IRequest<int>
    {
        public int ObserverId { get; set; }
        public int VotingSectionId { get; set; }
        public int? QuestionId { get; set; }
        public string RatingText { get; set; }
        public string AttachedFilePath { get; set; }
    }

    public class RatingProfile : Profile
    {
        public RatingProfile()
        {
            CreateMap<AddRatingCommand, Rating>()
                .ForMember(src => src.QuestionId, c => c.MapFrom(src => 
                    !src.QuestionId.HasValue || src.QuestionId.Value <= 0 ? null : src.QuestionId)
                 )
                .ForMember(src => src.LastUpdateDate, c => c.MapFrom(src => DateTime.UtcNow));
        }
    }
}
