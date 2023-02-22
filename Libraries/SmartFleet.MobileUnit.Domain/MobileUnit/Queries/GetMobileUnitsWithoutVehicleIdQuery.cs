using System.Collections.Generic;
using MediatR;
using SmartFleet.MobileUnit.Domain.MobileUnit.Dtos;

namespace SmartFleet.MobileUnit.Domain.MobileUnit.Queries
{
    public class GetMobileUnitsWithoutVehicleIdQuery : IRequest<IEnumerable<MobileUnitSelectListDto>> 
    {
    }
}
