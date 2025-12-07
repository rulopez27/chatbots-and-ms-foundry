using AutoMapper;
using Roboto.Models.Dto;

namespace Roboto.Models.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User,UserDto>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id)
                )
                .ForMember(
                    dest => dest.Username,
                    opt => opt.MapFrom(src => src.Username)
                )
                .ForMember(
                    dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email)
                )
                .ForMember(
                    dest => dest.FirstName,
                    opt => opt.MapFrom(src => src.FirstName)
                )
                .ForMember(
                    dest => dest.LastName,
                    opt => opt.MapFrom(src => src.LastName)
                )
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt)
                )
                .ForMember(
                    dest => dest.CalendarEvents,
                    opt => opt.MapFrom(src => src.CalendarEvents)
                )
                .ForMember(
                    dest => dest.ModifiedAt,
                    opt => opt.MapFrom(src => src.ModifiedAt)
                )
                .ReverseMap();
        }
        
    }
}