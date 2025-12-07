using AutoMapper;
using Roboto.Models.Dto;

namespace Roboto.Models.Profiles
{
    public class CalendarEventCreateProfile : Profile
    {
        public CalendarEventCreateProfile ()
        {
            CreateMap<CalendarEventCreateDto, CalendarEvent>()
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
                );
        }
    }
}