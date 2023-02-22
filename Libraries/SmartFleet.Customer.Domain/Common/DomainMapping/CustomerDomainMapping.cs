using System.Linq;
using AutoMapper;
using SmartFleet.Core.Contracts.Commands;
using SmartFleet.Core.Domain.Gpsdevices;
using SmartFleet.Core.Domain.Vehicles;
using SmartFleet.Customer.Domain.Commands.Vehicles;
using SmartFleet.Customer.Domain.Common.Dtos;

namespace SmartFleet.Customer.Domain.Common.DomainMapping
{
    public class CustomerDomainMapping : Profile
    {
        public CustomerDomainMapping()
        {
            CreateMap<CreateVehicleCommand, Vehicle>()
                .ForMember(x => x.Id, o => o.MapFrom(p => p.CmdId))
                .ForMember(x => x.VehicleType, o => o.MapFrom(co =>(VehicleType) co.VehicleType))
                .ForMember(x => x.Box_Id, o => o.MapFrom(co => co.BoxId))
                .ForMember(x => x.Brand_Id, o => o.MapFrom(co => co.BrandId))
                .ForMember(x => x.Model_Id, o => o.MapFrom(co => co.ModelId))

                .ReverseMap();
            CreateMap<CreateBoxCommand, Box>().ReverseMap();
            CreateMap<Vehicle, VehicleDto>()
                .ForMember(x => x.Customer, o => o.MapFrom(v => v.Customer.Name))
                .ForMember(x => x.CustomerId, o => o.MapFrom(v => v.CustomerId.ToString()))
                .ForMember(x => x.Brand, o => o.MapFrom(v => v.Brand.Name))
                .ForMember(x => x.Model, o => o.MapFrom(v => v.Model.Name))
                .ForMember(x => x.CreationDate,
                    o => o.MapFrom(v => v.CreationDate.HasValue ? v.CreationDate.Value.ToShortDateString() : ""))
                .ForMember(x => x.InitServiceDate,
                    o => o.MapFrom(v => v.InitServiceDate.HasValue ? v.InitServiceDate.Value.ToShortDateString() : ""))
                .ForMember(x => x.Imei, y => y.MapFrom(s => s.Boxes.Any() ? s.Boxes.FirstOrDefault().Imei : ""))
                .ForMember(x => x.VehicleStatus, o => o.MapFrom(x => x.VehicleStatus.ToString()));

        }
    }
}
