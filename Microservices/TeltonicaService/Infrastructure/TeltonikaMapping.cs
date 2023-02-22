using AutoMapper;
using SmartFleet.Core.Contracts.Commands;

namespace TeltonicaService.Infrastucture
{
    public class TeltonikaMapping : Profile
    {
        public TeltonikaMapping()
        {
            CreateMap< CreateTeltonikaGps, TLGpsDataEvent>()
                .ForMember(x => x.DateTimeUtc, o => o.MapFrom(v => v.DateTimeUtc))
                .ReverseMap();
        }
    }
}