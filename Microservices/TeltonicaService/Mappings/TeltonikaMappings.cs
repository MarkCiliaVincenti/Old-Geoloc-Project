using AutoMapper;
using SmartFleet.Core.Contracts.Commands;

namespace TeltonicaService.Mappings
{
    public class TeltonikaMappings : Profile
    {
        public TeltonikaMappings()
        {
            CreateMap<CreateTeltonikaGps, TLGpsDataEvent>();
        }
    }
}
