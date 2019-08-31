﻿using AutoMapper;

namespace VoteMonitor.Api.Observer.Models
{
    public class GeneratedObserver
    {
        public string Id { get; set; }
        public string Pin { get; set; }
    }
    public class ObserverProfile : Profile
    {
        public ObserverProfile()
        {
            _ = CreateMap<VoteMonitor.Entities.Observer, GeneratedObserver>()
                .ForMember(dest => dest.Id, c => c.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Pin, c => c.MapFrom(src => src.Pin));
        }
    }
}