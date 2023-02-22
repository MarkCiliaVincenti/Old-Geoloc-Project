using System.Collections.Generic;
using MediatR;
using SmartFleet.MobileUnit.Domain.MobileUnit.Dtos;

namespace SmartFleet.MobileUnit.Domain.MobileUnit.Queries
{
    public class GetMobileUnitsListQuery : IRequest<List<MobileUnitDto>>
    {
    }
}
