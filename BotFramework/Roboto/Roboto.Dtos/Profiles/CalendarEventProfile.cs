using AutoMapper;
using Roboto.Models;

namespace Roboto.Dtos.Profiles
{
    public class CalendarEventProfile : Profile
    {
        public CalendarEventProfile()
        {
            CreateMap<CalendarEvent, CalendarEventDto>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id)
                )
                .ForMember(
                    dest => dest.Title,
                    opt => opt.MapFrom(src => src.Title)
                )
                .ForMember(
                    dest => dest.StartDateTime,
                    opt => opt.MapFrom(src => src.StartDateTime)
                )
                .ForMember(
                    dest => dest.Duration,
                    opt => opt.MapFrom(src => src.Duration)
                )
                .ForMember(
                    dest => dest.EndDateTime,
                    opt => opt.MapFrom(src => src.EndDateTime)
                )
                .ForMember(
                    dest => dest.BlockCalendar,
                    opt => opt.MapFrom(src => src.BlockCalendar)
                )
                .ForMember(
                    dest => dest.IsAllDay,
                    opt => opt.MapFrom(src => src.IsAllDay)
                )
                .ForMember(
                    dest => dest.Details,
                    opt => opt.MapFrom(src => src.Details)
                )
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.UserId)
                )
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt)
                )
                .ForMember(
                    dest => dest.ModifiedAt,
                    opt => opt.MapFrom(src => src.ModifiedAt)
                )
                .ReverseMap();
        }
    }
}