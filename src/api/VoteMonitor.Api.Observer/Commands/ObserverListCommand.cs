using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Queries;

namespace VoteMonitor.Api.Observer.Commands {
    public class ObserverListCommand : IRequest<ApiListResponse<ObserverModel>> {
        public int IdNgo { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ObserverListCommandProfile : Profile {
        public ObserverListCommandProfile() {
            CreateMap<ObserverListQuery, ObserverListCommand>()
                .ForMember(dest => dest.Number, c => c.MapFrom(src => src.Number))
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.Page, c => c.MapFrom(src => src.Page))
                .ForMember(dest => dest.PageSize, c => c.MapFrom(src => src.PageSize))
               ;
        }
    }
}
