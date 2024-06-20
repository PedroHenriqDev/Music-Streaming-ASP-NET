using ApplicationLayer.ViewModels;
using AutoMapper;
using DomainLayer.Entities;
using UtilitiesLayer.Helpers;

namespace ApplicationLayer.Profiles;

public class DomainMappingProfile : Profile
{
    public DomainMappingProfile() 
    {
        CreateMap<Listener, DescriptionViewModel>()
            .ForMember(dest => dest.GeneratedDescription, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Artist, DescriptionViewModel>()
            .ForMember(dest => dest.GeneratedDescription, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Music, MusicViewModel>()
            .ForMember(dest => dest.DurationText, opt => opt.MapFrom(src => MusicHelper.FormatMusicDuration(src.Duration)))
            .ReverseMap();

        CreateMap<Playlist, PlaylistViewModel>()
            .ForMember(dest => dest.Musics, opt => opt.Ignore())
            .ReverseMap();
    }
}
