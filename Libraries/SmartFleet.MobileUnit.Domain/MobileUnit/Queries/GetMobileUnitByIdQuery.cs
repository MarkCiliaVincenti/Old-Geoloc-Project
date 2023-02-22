using System;
using MediatR;
using SmartFleet.MobileUnit.Domain.MobileUnit.Dtos;

namespace SmartFleet.MobileUnit.Domain.MobileUnit.Queries
{
    public class GetMobileUnitByIdQuery :IRequest<MobileUnitDto>
    {
        public Guid Id { get; set; }
    }
}
