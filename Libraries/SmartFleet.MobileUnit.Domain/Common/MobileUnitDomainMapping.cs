using AutoMapper;
using SmartFleet.Core.Domain.Gpsdevices;
using SmartFleet.MobileUnit.Domain.MobileUnit.Dtos;

namespace SmartFleet.MobileUnit.Domain.Common
{
    public class MobileUnitDomainMapping :Profile
    {
        public MobileUnitDomainMapping()
        {
            CreateMap<Box, MobileUnitDto>().ReverseMap();
        }
    }
}
