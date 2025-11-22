using SearchService.Application.DTOs;
using SearchService.Domain.Entities;
using AutoMapper;

namespace SearchService.Application.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<SearchItem, SearchItemDto>()
                .ForMember(d => d.ViewCount, o => o.MapFrom(s => s.Metadata != null ? s.Metadata.ViewCount : 0))
                .ForMember(d => d.Relevance, o => o.MapFrom(s => s.Metadata != null ? s.Metadata.Relevance : 0))
                .ForMember(d => d.LastIndexed, o => o.MapFrom(s => s.Metadata != null ? s.Metadata.LastIndexed : DateTimeOffset.MinValue));

            CreateMap<CreateSearchItemDto, SearchItem>()
                .ForMember(d => d.Metadata, o => o.Ignore());

            CreateMap<SearchItemDto, SearchItem>()
                .ForMember(d => d.Metadata, o => o.Ignore());
        }
    }
}