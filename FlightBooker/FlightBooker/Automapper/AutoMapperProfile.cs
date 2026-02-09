using NuGet.Protocol.Plugins;
using FlightBooker.Domains.DataDB;
using FlightBooker.Domains.EntitiesDB;
using FlightBooker.ViewModels;
using AutoMapper;

namespace FlightBooker.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<City, CityVM>();

            CreateMap<Flight, FlightVM>()
        .ForMember(dest => dest.DepartCityName, opt => opt.MapFrom(src => src.DepartCityNavigation.FullName))
        .ForMember(dest => dest.ArriveCityName, opt => opt.MapFrom(src => src.ArriveCityNavigation.FullName))
        .ForMember(dest => dest.LocalDateTimeDepart, opt => opt.MapFrom(src =>
            src.DateTimeDepart.HasValue
                ? src.DateTimeDepart.Value.AddHours((double)src.DepartCityNavigation.Utcoffset)
                : (DateTime?)null))
        .ForMember(dest => dest.LocalDateTimeArrive, opt => opt.MapFrom(src =>
            src.DateTimeArrive.HasValue
                ? src.DateTimeArrive.Value.AddHours((double)src.ArriveCityNavigation.Utcoffset)
                : (DateTime?)null));

            CreateMap<Booking, BookingVM>();


        }

    }
}
