using System;
using MediatR;
using SmartFleet.Customer.Domain.Common.Dtos;

namespace SmartFleet.Customer.Domain.Queries.Vehicles
{
    public class GetVehicleByMobileUnitImeiQuery : IRequest<VehicleDto>
    {
        public string Imei { get; set; }
    }
}
